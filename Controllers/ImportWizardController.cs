using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto.ImportWizard;
using OntrackDb.Service;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Controllers;

[Authorize(Roles = "SuperAdmin")]
[Route("api/import-wizard")]
[ApiController]
public class ImportWizardController : ControllerBase
{
    readonly IImportWizardService importWizardService;

    public ImportWizardController(IImportWizardService importWizardService)
    {
        this.importWizardService = importWizardService;
    }

    [Route("{pharmacyId:int}/upload")]
    [HttpPost]
    [Consumes("multipart/form-data")]
   // [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
   // [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadAsync(int pharmacyId,IFormFile formFile)
    {
        if (Request.Form.Files.Count == 0) return BadRequest();

        IFormFile file = Request.Form.Files[0];

        var result = await importWizardService.UploadSourceFileAsync(pharmacyId, file);

        return Ok(result);
    }

    [Route("column-mappings")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut]
    public async Task<IActionResult> UpdateColumMappings(ColumnMappingRequestDTO request)
    {
        var result = await importWizardService.UpdateColumnMappingsAsync(request);

        return Ok(result);
    }

    [Route("history/{pharmacyId:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetImportHistory(int pharmacyId, [FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] string keywords)
    {
        if (pharmacyId <= 0) return BadRequest();

        var result = await importWizardService.GetImportHistoryAsync(pharmacyId, recordNumber, pageLimit, filterType, filterValue, startDate, endDate, keywords);

        return Ok(result);
    }

    [Route("last-mappings/{pharmacyId:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetLastColumnMappings(int pharmacyId)
    {
        if (pharmacyId <= 0) return BadRequest();

        var result = await importWizardService.GetLastColumnMappingsAsync(pharmacyId);

        return Ok(result);
    }

    [Route("errors/{sourceFileId:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetFileProcessingErrors(int sourceFileId, [FromQuery] int recordNumber, [FromQuery] int pageLimit)
    {
        if (sourceFileId <= 0) return BadRequest();

        var result = await importWizardService.GetFileErrorAsync(sourceFileId, recordNumber, pageLimit);

        return Ok(result);
    }

    [Route("re-run/{sourceFileId:int}")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [HttpPut]
    public async Task<IActionResult> RerunImportProcess(int sourceFileId)
    {
        var result = await importWizardService.RerunImportProcessAsync(sourceFileId);

        return Ok(result);
    }
}
