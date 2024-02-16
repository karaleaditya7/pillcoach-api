using Microsoft.AspNetCore.Mvc;
using OntrackDb.Model;
using System.Threading.Tasks;
using System;

namespace OntrackDb.Controllers
{
    [System.Web.Mvc.Route("api/[controller]")]
    [ApiController]
    public class TwilioWebhookController : ControllerBase
    {
        [HttpPost]
        //[Consumes("application/json")]
        [Route("call-event")]
        public async Task<IActionResult> HandleCallEvent()//[FromBody] TwilioRequestModel twilioRequest)
        {
            //Console.WriteLine("Received Twilio Request:");
            //Console.WriteLine($"Call SID: {twilioRequest.CallSid}");
            //Console.WriteLine($"From: {twilioRequest.From}");
            //Console.WriteLine($"To: {twilioRequest.To}");
            //Console.WriteLine($"Call Status: {twilioRequest.CallStatus}");
            return NoContent();
        }
    }
}
