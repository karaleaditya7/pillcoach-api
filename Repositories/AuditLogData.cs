using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OntrackDb.Repositories;

public interface IAuditLogData
{
    Task<AuditLog> AddAuditLogAsync(AuditLog auditLog);
    Task<PagedResult<AuditLogDetailModel>> GetAuditLogsAsync(string userId, AuditLogSearchModel searchModel);
    Task<List<AuditEmployeeModel>> GetAuditEmployeesAsync(string userId, string searchText);
    Task<List<AuditPatientModel>> GetAuditPatientsAsync(string userId, string searchText);
    Task<List<AuditLog>> GetAuditLogsBypatientId(int patientId);

    void DeleteAuditLogForPatient(AuditLog auditLog);
}

public class AuditLogData : IAuditLogData
{
    readonly ApplicationDbContext dbContext;

    public AuditLogData(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<AuditLog> AddAuditLogAsync(AuditLog auditLog)
    {
        var result = await dbContext.AuditLogs.AddAsync(auditLog);

        await dbContext.SaveChangesAsync();

        return result.Entity;
    }

    public async Task<PagedResult<AuditLogDetailModel>> GetAuditLogsAsync(string userId, AuditLogSearchModel searchModel)
    {
        var query = dbContext.AuditLogs
            .Where(a => a.LogDateUTC.AddMinutes(-searchModel.TimezoneOffset).Date >= searchModel.StartDate.Date && a.LogDateUTC.AddMinutes(-searchModel.TimezoneOffset).Date <= searchModel.EndDate.Date);

        if (userId != null)
        {
            // match patients/users from the same pharmacies as the current user
            query = query.Where(a => a.Patient.Pharmacy.PharmacyUsers.Any(u => u.UserId == userId) ||
                    dbContext.PharmacyUsers.Where(p => p.UserId == userId).SelectMany(p => p.Pharmacy.PharmacyUsers).Any(pu => pu.UserId == a.UserId));
        }

        // append additional where clauses, depending on search criteria

        if (searchModel.EmployeeIds.Any())
            query = query.Where(a => searchModel.EmployeeIds.Any(u => u.Equals(a.UserId)));

        if (searchModel.PatientIds.Any())
            query = query.Where(a => searchModel.PatientIds.Any(p => p.Equals(a.PatientId)));

        // execute the query for total records

        var totalRecordCount = await query.CountAsync();

        // calculate the page offset and total pages

        var pageSize = searchModel.PageSize;
        var offset = 0;
        var totalPageCount = 1;

        if (searchModel.PageSize > 0)
        {
            pageSize = searchModel.PageSize;
            offset = (searchModel.PageNumber - 1) * pageSize;
            totalPageCount = (int)Math.Ceiling(totalRecordCount / (double)pageSize);
        }
        else
        {
            pageSize = totalRecordCount;
        }

        var records = new List<AuditLogDetailModel>();

        if (searchModel.PageNumber <= totalPageCount)
        {
            // prepare query to execute for actual records
            // set the sort order for the query

            var sortExpr = GetSortOrderExpression(searchModel.SortField);

            if (searchModel.SortDirection == Enums.SortOrder.Ascending)
            {
                query = sortExpr.Count() == 1 ? query.OrderBy(sortExpr.First()) :
                    query.OrderBy(sortExpr.First()).ThenBy(sortExpr.Last());
            }
            else
            {
                query = sortExpr.Count() == 1 ? query.OrderByDescending(sortExpr.First()) :
                    query.OrderByDescending(sortExpr.First()).ThenByDescending(sortExpr.Last());
            }

            var rg = new Regex("[A-Z]");

            var q2 = query
                .Skip(offset)
                .Take(pageSize)
                .Select(a => new AuditLogDetailModel
                {
                    LogId = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient != null ? $"{a.Patient.Contact.FirstName} {a.Patient.Contact.LastName}" : null,
                    PatientDOB = a.Patient.Contact.DoB,
                    EmployeeName = $"{a.User.FirstName} {a.User.LastName}",
                    EmployeeEmail = a.User.Email,
                    LogDateUTC = a.LogDateUTC.AddMinutes(-searchModel.TimezoneOffset),
                    ActionSourceId = a.ActionSourceId,
                    ActionSource = rg.Replace(a.SourceType.ActionSourceType, " $0").Trim(),
                    ActionTypeId = a.ActionTypeId,
                    ActionType = rg.Replace(a.ActionType.ActionType, " $0").Trim()
                });

            records = await q2.ToListAsync();
        }

        return new PagedResult<AuditLogDetailModel>()
        {
            PageNumber = searchModel.PageNumber,
            PageSize = pageSize,
            TotalRecordCount = totalRecordCount,
            TotalPageCount = totalPageCount,
            Records = records
        };
    }

    public async Task<List<AuditEmployeeModel>> GetAuditEmployeesAsync(string userId, string searchText)
    {
        var query = dbContext.PharmacyUsers
            .Where(u => u.User.FirstName.Contains(searchText) || u.User.LastName.Contains(searchText) || u.User.Email.Contains(searchText))
            .Join(dbContext.PharmacyUsers.Where(p => p.UserId == userId), p1 => p1.PharmacyId, p2 => p2.PharmacyId, (p1, p2) => new
            {
                p1.User.Id,
                p1.User.FirstName,
                p1.User.LastName,
                p1.User.Email
            }).Distinct();
            
        return await query
            .Select(a => new AuditEmployeeModel
            {
                UserId = a.Id,
                EmployeeName = $"{a.FirstName} {a.LastName}",
                EmployeeEmail = a.Email
            }).ToListAsync();
    }

    public async Task<List<AuditLog>> GetAuditLogsBypatientId(int patientId)
    {
      var auditLogs = await dbContext.AuditLogs.Where(p=>p.Patient.Id == patientId).ToListAsync();
        return auditLogs;   
    }

    public void DeleteAuditLogForPatient(AuditLog auditLog)
    {
        dbContext.AuditLogs.Remove(auditLog);
        
    }
    public async Task<List<AuditPatientModel>> GetAuditPatientsAsync(string userId, string searchText)
    {
        var query = dbContext.Patients
            .Where(p => p.Pharmacy.PharmacyUsers.Any(u => u.UserId == userId) &&
                (p.Contact.FirstName.Contains(searchText) || p.Contact.LastName.Contains(searchText)))
            .Distinct();

        return await query
            .Select(a => new AuditPatientModel
            {
                PatientId = a.Id,
                PatientName = $"{a.Contact.FirstName} {a.Contact.LastName}",
                PatientDOB = a.Contact.DoB
            }).ToListAsync();
    }

    IEnumerable<Expression<Func<AuditLog, object>>> GetSortOrderExpression(string sortBy)
    {
        List<Expression<Func<AuditLog, object>>> expr = new();

        switch(sortBy)
        {
            case nameof(AuditLogDetailModel.LogDateUTC):
                expr.Add((u) => u.LogDateUTC);
                break;

            case nameof(AuditLogDetailModel.ActionType):
                expr.Add((u) => u.ActionType.ActionType);
                break;

            case nameof(AuditLogDetailModel.ActionSource):
                expr.Add((u) => u.SourceType.ActionSourceType);
                break;

            case nameof(AuditLogDetailModel.EmployeeName):
                expr.Add((u) => u.User.FirstName);
                expr.Add((u) => u.User.LastName);
                break;

            case nameof(AuditLogDetailModel.EmployeeEmail):
                expr.Add((u) => u.User.Email);
                break;

            case nameof(AuditLogDetailModel.PatientName):
                expr.Add((u) => u.Patient.Contact.FirstName);
                expr.Add((u) => u.Patient.Contact.LastName);
                break;

            case nameof(AuditLogDetailModel.PatientDOB):
                expr.Add((u) => u.Patient.Contact.DoB);
                break;
        }

        if (!expr.Any()) expr.Add((u) => u.Id);

        return expr.Take(2);
    }
}
