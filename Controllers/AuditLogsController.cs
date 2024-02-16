using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
using OntrackDb.Model;
using OntrackDb.Service;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Controllers;

[Authorize]
[Route("api/audit-logs")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiController]
public class AuditLogsController : ControllerBase
{
    readonly IAuditLogService auditLogService;
    readonly IJwtUtils _jwtUtils;

    public AuditLogsController(IAuditLogService auditLogService, IJwtUtils jwtUtils)
    {
        this.auditLogService = auditLogService;
        _jwtUtils = jwtUtils;
    }

    [HttpPost]
    public async Task<IActionResult> AddAuditLogAsync([Required] AuditLogModel auditLogModel, [FromHeader] string authorization)
    {
        var userId = "";

        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            var parameter = headerValue.Parameter;
            userId =  _jwtUtils.ValidateToken(parameter);
        }

        return Ok(await auditLogService.AddAuditLogAsync(auditLogModel, userId));
    }

    [HttpPost]
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("search")]
    public async Task<IActionResult> GetAuditLogsAsync(AuditLogSearchModel searchModel)
    {
        return Ok(await auditLogService.GetAuditLogsAsync(searchModel));
    }

    [HttpGet]
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("employees")]
    public async Task<IActionResult> GetAuditEmployeesAsync([Required][FromQuery(Name = "q")]string searchText)
    {
        return Ok(await auditLogService.GetAuditEmployeesAsync(searchText));
    }

    [HttpGet]
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("patients")]
    public async Task<IActionResult> GetAuditPatientsAsync([Required][FromQuery(Name = "q")] string searchText)
    {
        return Ok(await auditLogService.GetAuditPatientsAsync(searchText));
    }

    [HttpPost]
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("download")]

    public async Task<IActionResult> GetAuditLogsCsvAsync(AuditLogSearchModel searchModel)
    {
        var response = await auditLogService.GetContentForCsvAsync(searchModel);

        if (!response.Success) return Ok(response);

        return File(Encoding.ASCII.GetBytes(response.Data), "text/csv", "AuditLogs.csv");
    }
}
