using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using OntrackDb.Service;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTakeawayInformationController : ControllerBase
    {
        private readonly IServiceTakeawayInformationService _serviceTakeawayInformationService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserData _userData;

        public ServiceTakeawayInformationController(IServiceTakeawayInformationService serviceTakeawayInformationService , IJwtUtils jwtUtils , IUserData userData)
        {
            _serviceTakeawayInformationService = serviceTakeawayInformationService;
            _jwtUtils = jwtUtils;
            _userData = userData;
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddServiceTakeawayInformation([FromBody] ServiceTakeawayInformationModel model)
        {
            var response = await _serviceTakeawayInformationService.AddServiceTakeawayInformation(model);

            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("{serviceTakeawayInformationId}")]
        public async Task<IActionResult> GetServiceTakeawayInformationById(int serviceTakeawayInformationId)
        {
            var response = await _serviceTakeawayInformationService.GetServiceTakeawayInformationById(serviceTakeawayInformationId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut]
        public async Task<IActionResult> EditServiceTakeawayInformation([FromBody] ServiceTakeawayInformationForUpdateModel model)
        {
            var response = await _serviceTakeawayInformationService.EditServiceTakeawayInformation(model);
            return Ok(response);
        }


        [HttpDelete]
        [Route("{serviceTakeawayInformationId}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteServiceTakawayInformationById(int serviceTakeawayInformationId)
        {
           var response = await _serviceTakeawayInformationService.DeleteServiceTakawayInformationById(serviceTakeawayInformationId);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("details/{patientId}")]
        public async Task<IActionResult> GetServiceTakewayInformationByPatientId(int patientId)
        {
            var response = await _serviceTakeawayInformationService.GetServiceTakewayInformationByPatientId(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("sendpdf")]
        public async Task<IActionResult> SendCmrPdfEmail( string email,int patientId )
        {
           
            var response = await _serviceTakeawayInformationService.SendCmrPdfEmail(email ,patientId);
            return Ok(response);
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet]
        [Route("{patientId}/uuid")]
        public async Task<IActionResult> CheckLinkExpired(int patientId, string uuid)
        {

            var response = await _serviceTakeawayInformationService.CheckLinkExpired(patientId, uuid);
            return Ok(response);
            
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost]
        [Route("{patientId}/verification")]
        public async Task<IActionResult> PatientVerificationForPdf(string lastname , DateTime dob , int patientId,string uuid)
        {  

            var response = await _serviceTakeawayInformationService.PatientVerificationForPdf(lastname, dob , patientId,uuid);
            if(response.Success)
            {
                //return File(response.Data, "application/pdf","cmr.pdf");
                return Ok(Convert.ToBase64String(response.Data));
            }
            else
            {
                return Ok(response);
            }
            
        }

    }
}
