using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Service;

public interface IAuditLogService
{
    Task<Response<int>> AddAuditLogAsync(AuditLogModel model, string userId);
    Task<Response<PagedResult<AuditLogDetailModel>>> GetAuditLogsAsync(AuditLogSearchModel searchModel);
    Task<Response<AuditEmployeeModel>> GetAuditEmployeesAsync(string searchText);
    Task<Response<AuditPatientModel>> GetAuditPatientsAsync(string searchText);
    Task<Response<string>> GetContentForCsvAsync(AuditLogSearchModel searchModel);
}

internal class AuditLogService : IAuditLogService
{
    readonly IAuditLogData auditLodData;
    readonly IPatientData patientData;
    readonly IUserData userData;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly UserManager<User> userManager;

    public AuditLogService(IAuditLogData auditLodData, IPatientData patientData, IUserData userData, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        this.auditLodData = auditLodData;
        this.patientData = patientData;
        this.userData = userData;
        this.httpContextAccessor = httpContextAccessor;
        this.userManager = userManager;
    }

    public async Task<Response<int>> AddAuditLogAsync(AuditLogModel model, string userId)
    {
        var result = new Response<int>();

        if (!Enum.IsDefined(typeof(AuditActionType), model.ActionTypeId))
        {
            result.Message = $"Invalid Action Type Id: {model.ActionTypeId}";
            return result;
        }

        if (!Enum.IsDefined(typeof(AuditActionSourceType), model.ActionSourceId))
        {
            result.Message = $"Invalid Action Source Type Id: {model.ActionSourceId}";
            return result;
        }

        var user = !string.IsNullOrWhiteSpace(userId) ? await userData.GetUserByUserId(userId) : null;

        if (user == null)
        {
            result.Message = "Invalid/Unknown user";
            return result;
        }

        if (model.PatientId != null && !await patientData.IsValidPatientForUser(user.Id, model.PatientId.Value))
        {
            result.Message = $"Invalid Patient Id: {model.PatientId}";
            return result;
        }

        var entity = await auditLodData.AddAuditLogAsync(new Entities.AuditLog
        {
            PatientId = model.PatientId,
            UserId = user.Id,
            ActionTypeId = model.ActionTypeId,
            ActionSourceId = model.ActionSourceId,
            LogDateUTC = DateTime.UtcNow
        });

        result.Data = entity.Id;
        result.Success = true;

        return result;
    }

    public async Task<Response<AuditEmployeeModel>> GetAuditEmployeesAsync(string searchText)
    {
        var result = new Response<AuditEmployeeModel>();

        var user = httpContextAccessor.HttpContext.User;

        if (user == null)
        {
            result.Message = "Invalid/Unknown user";
            return result;
        }

        var userId = user.FindFirst("id")?.Value;

        var records = await auditLodData.GetAuditEmployeesAsync(userId, searchText);

        if ((records ?? new()).Any())
            records.Sort((x, y) => x.EmployeeName.CompareTo(y.EmployeeName));

        result.DataList = records;
        result.Success = true;

        return result;
    }

    public async Task<Response<AuditPatientModel>> GetAuditPatientsAsync(string searchText)
    {
        var result = new Response<AuditPatientModel>();

        var user = httpContextAccessor.HttpContext.User;

        if (user == null)
        {
            result.Message = "Invalid/Unknown user";
            return result;
        }

        var userId = user.FindFirst("id")?.Value;

        var records = await auditLodData.GetAuditPatientsAsync(userId, searchText);

        if ((records ?? new()).Any())
            records.Sort((x,y) => x.PatientName.CompareTo(y.PatientName));

        result.DataList = records;
        result.Success = true;

        return result;
    }

    public async Task<Response<PagedResult<AuditLogDetailModel>>> GetAuditLogsAsync(AuditLogSearchModel searchModel)
    {
        var result = new Response<PagedResult<AuditLogDetailModel>>();

        if (searchModel.StartDate > searchModel.EndDate)
        {
            result.Message = "Invalid date range";
            return result;
        }

        var user = httpContextAccessor.HttpContext.User;

        if (user == null)
        {
            result.Message = "Invalid/Unknown user";
            return result;
        }

        var userId = user.FindFirst("id")?.Value;

        var appUser = await userData.GetUserById(userId);

        if (await userManager.IsInRoleAsync(appUser, Roles.SuperAdmin.ToString()))
            userId = null;

        // set default page number and page size, if applicable

        if (searchModel.PageNumber <= 0) searchModel.PageNumber = 1;
        if (searchModel.PageSize <= 0) searchModel.PageSize = 15;

        var data = await auditLodData.GetAuditLogsAsync(userId, searchModel);

        result.Success = true;
        result.Data = data;

        return result;
    }

    public async Task<Response<string>> GetContentForCsvAsync(AuditLogSearchModel searchModel)
    {
        var result = new Response<string> { Data = string.Empty };

        if (searchModel.StartDate > searchModel.EndDate)
        {
            result.Message = "Invalid date range";
            return result;
        }

        var user = httpContextAccessor.HttpContext.User;

        if (user == null)
        {
            result.Message = "Invalid/Unknown user";
            return result;
        }

        var userId = user.FindFirst("id")?.Value;

        var appUser = await userData.GetUserById(userId);

        if (await userManager.IsInRoleAsync(appUser, Roles.SuperAdmin.ToString()))
            userId = null;

        searchModel.PageSize = -1; // in order to return all records

        var data = await auditLodData.GetAuditLogsAsync(userId, searchModel);

        if (data.Records != null)
        {
            var str = new StringBuilder();

            str.AppendLine(AuditLogDetailModel.GetHeaderRowForCsv());

            foreach (var record in data.Records)
                str.AppendLine(record.GetRecordForCsv());

            result.Data = str.ToString();
        }

        result.Success = true;

        return result;
    }
}
