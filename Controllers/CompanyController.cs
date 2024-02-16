using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Twilio.AspNet.Common;
using Twilio.Jwt;
using Twilio.Jwt.Client;
using Twilio;
using Twilio.Security;
using OntrackDb.Filter;

namespace OntrackDb.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>
        /// Get auth token for sms . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting Auth token for sms.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("auth/{identity}")]
        public  IActionResult GenerateAuthTokenForSms(string identity)
        {
            var response = _companyService.GenerateAuthTokenForSms(identity);
            return Ok(response);
        }

        /// <summary>
        /// Get auth token for voice calling . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting Auth token for voice calling feature.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("voice/token/{identity}")]
        public IActionResult GenerateAuthTokenForVoice(string identity)
        {
            var response = _companyService.GenerateAuthTokenForVoice(identity);
            return Content(response, "application/jwt");
        }

        /// <summary>
        /// Get auth token for video calling . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting Auth token for video calling.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("video/{identity}")]
        public IActionResult GenerateAuthTokenForVideo(string identity)
        {
            var response = _companyService.GenerateAuthTokenForVideo(identity);
            return Ok(response);
        }

        /// <summary>
        /// Get auth token for video for patient video call . Role[Everyone can access]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting Auth token for patient video call.
        /// </remarks>
        [AllowAnonymous]
        [HttpGet]
        [Route("patient/video/{identity}")]
        public IActionResult GenerateAuthTokenpatientVideoCall(int identity)
        {
            var response = _companyService.GenerateAuthTokenForPatientVideoCall(identity);
            return Ok(response);
        }

        /// <summary>
        /// To Create Voice call . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for generating a vocie calling feature.
        /// </remarks>
        [ValidateTwilioRequest]
        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost]
        [Route("voice")]
        public IActionResult VoiceCall([FromForm] VoiceRequest request)
        {
            
            var response = _companyService.VoiceCall(request.To, request.CallSid);
            return Ok(XElement.Parse(response.ToString()));
        }
    }
}
