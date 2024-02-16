using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
using OntrackDb.Dto;
using OntrackDb.Enums;
using OntrackDb.Service;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Controllers;

[Authorize(Roles = "SuperAdmin")]
[Route("api/import-logs")]
[ApiController]
public class ImportLogController : ControllerBase
{
    private readonly IImportLogService importLogsService;
    private readonly IJwtUtils jwtUtils;

    public ImportLogController(IImportLogService importLogsService,IJwtUtils jwtUtils)
    {
        this.importLogsService = importLogsService;
        this.jwtUtils = jwtUtils;
    }
    [Route("getImportLogsbyUserId")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetLatestImportHistory([FromHeader] string authorization, [FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery]DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] bool importEnabled)
    {
        var parameter = "";
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            parameter = headerValue.Parameter;
        }
        var role = jwtUtils.GetRole(parameter);
        var userID = jwtUtils.ValidateToken(parameter);

        if (role == Roles.SuperAdmin.ToString() && importEnabled)
        {
            var response = await importLogsService.GetLatestImportHistory(recordNumber, pageLimit, filterType, filterValue,startDate,endDate,keywords,sortDirection);
            return Ok(response);
        }
        return BadRequest();
    }

    [Produces("application/json")]
    [Route("Download/{importId}")]
    [HttpGet]
    public async Task<IActionResult> DownloadFile(int importId)
    {
        var response = await importLogsService.DownloadFile(importId);
        return Ok(response);
    }

    [Route("getErrorDetails")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetErrorDetails([FromHeader] string authorization, [FromQuery] int importId, [FromQuery] int rowNumber)
    {
        var parameter = "";
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            parameter = headerValue.Parameter;
        }
        var role = jwtUtils.GetRole(parameter);
        var userID = jwtUtils.ValidateToken(parameter);

        if (role == Roles.SuperAdmin.ToString())
        {
            var response = await importLogsService.GetErrorDetails(importId, rowNumber);
            return Ok(response);
        }
        return BadRequest();
    }
    /// <summary>
    ///  For update the Row. Role[SuperAdmin]
    /// </summary>
    /// <remarks>
    ///  This API Will be used For Update the Row of staging table.
    /// </remarks>
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut]
    public async Task<IActionResult> UpdateStagingErrorRow([FromHeader] string authorization,[FromBody] StagingRowErrorDto stagingRow)
    {
        var parameter = "";
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            parameter = headerValue.Parameter;
        }
        var role = jwtUtils.GetRole(parameter);
        var userID = jwtUtils.ValidateToken(parameter);

        if (role == Roles.SuperAdmin.ToString())
        {
            var response = await importLogsService.UpdateStagingErrorRow(stagingRow);
            return Ok(response);
        }
        return BadRequest();
    }

    /// <summary>
    ///  For update the ImportSourceFiles table row. Role[SuperAdmin]
    /// </summary>
    /// <remarks>
    ///  This API Will be used For Update the Row of ImportSourceFiles table.
    /// </remarks>
    [Route("UpdateImportSourceFileRow")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut]
    public async Task<IActionResult> UpdateImportSourceFileRow([FromHeader] string authorization, int importId)
    {
        var parameter = "";
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            parameter = headerValue.Parameter;
        }
        var role = jwtUtils.GetRole(parameter);

        if (role == Roles.SuperAdmin.ToString())
        {
            var response = await importLogsService.UpdateImportSourceFileRow(importId);
            return Ok(response);
        }
        return BadRequest();
    }

    /// <summary>
    ///  For update the ImportSourceFiles table status field. Role[SuperAdmin]
    /// </summary>
    /// <remarks>
    ///  This API Will be used For Update the status field as rejected of ImportSourceFiles table.
    /// </remarks>
    [Route("updateRejectedStatus")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut]
    public async Task<IActionResult> UpdateImportSourceFileStatusRejected([FromHeader] string authorization, int importId)
    {
        var parameter = "";
        if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
        {
            parameter = headerValue.Parameter;
        }
        var role = jwtUtils.GetRole(parameter);

        if (role == Roles.SuperAdmin.ToString())
        {
            var response = await importLogsService.UpdateImportSourceFileStatusRejected(importId);
            return Ok(response);
        }
        return BadRequest();
    }
}



