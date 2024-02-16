using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Controllers;

[Authorize]
[Route("api/twilio")]
[ApiController]
public class TwilioRecordingsController : ControllerBase
{
    private IConfiguration _configuration;

    public TwilioRecordingsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    [Route("recordings/{pathSid}")]
    public async Task<IActionResult> GetRecording(string pathSid)
    {
        if (string.IsNullOrWhiteSpace(pathSid))
            return BadRequest();

        var accountSid = _configuration["twilioAccountSid"];
        var authToken  = _configuration["twimlAuthToken"];
        var filename = $"{pathSid}.mp3";

        var url = $"https://api.twilio.com/2010-04-01/Accounts/{accountSid}/Recordings/{filename}";

        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountSid}:{authToken}")));
            var array = await httpClient.GetByteArrayAsync(url);
            return File(array, "audio/mpeg", $"{filename}");

        }
        catch
        {
            return StatusCode(500);
        }
    }
}
