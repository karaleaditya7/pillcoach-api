using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Service;
using System.Threading.Tasks;

namespace OntrackDb.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class PatientCallsController : ControllerBase
{
    readonly IPatientCallInfoService callInfoService;

    public PatientCallsController(IPatientCallInfoService callInfoService)
    {
        this.callInfoService = callInfoService;
    }

    [HttpGet]
    [Route("list/{patientId:int}")]
    public async Task<IActionResult> GetCallListAsync(int patientId)
    {
        var result = await callInfoService.GetPatientCallListAsync(patientId);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SaveCallInfoAsync(PatientCallInfoDto callInfo)
    {
        var result = await callInfoService.SavePatientCallInfoAsync(callInfo);

        return Ok(result);
    }
}
