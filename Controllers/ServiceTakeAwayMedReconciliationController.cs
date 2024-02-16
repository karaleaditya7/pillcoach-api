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
    public class ServiceTakeAwayMedReconciliationController : Controller
    {
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserData _userData;
        private readonly IServiceTakeAwayMedReconciliationService _serviceTakeAwayMedReconciliationService;

        public ServiceTakeAwayMedReconciliationController(IJwtUtils jwtUtils, IUserData userData , IServiceTakeAwayMedReconciliationService serviceTakeAwayMedReconciliationService)
        {
            _jwtUtils = jwtUtils;
            _userData = userData;
            _serviceTakeAwayMedReconciliationService = serviceTakeAwayMedReconciliationService;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddServiceTakeawayMedReconciliation([FromBody] ServiceTakeAwayMedReconciliationModel model)
        {
            var response = await _serviceTakeAwayMedReconciliationService.AddServiceTakeawayMedReconciliation(model);

            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("{serviceTakeawaymedreconciliationId}")]
        public async Task<IActionResult> GetServiceTakeawayMedReconciliationById(int serviceTakeawaymedreconciliationId)
        {
            var response = await _serviceTakeAwayMedReconciliationService.GetServiceTakeawayMedReconciliationById(serviceTakeawaymedreconciliationId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut]
        public async Task<IActionResult> EditServiceTakeawayMedReconciliation([FromBody] ServiceTakeAwayMedReconciliationUpdateModel model)
        {
            var response = await _serviceTakeAwayMedReconciliationService.EditServiceTakeawayMedReconciliation(model);
            return Ok(response);
        }

        [HttpDelete]
        [Route("{serviceTakeawaymedreconciliationId}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteServiceTakeAwayMedReconciliationById(int serviceTakeawaymedreconciliationId)
        {
            var response = await _serviceTakeAwayMedReconciliationService.DeleteServiceTakeAwayMedReconciliationById(serviceTakeawaymedreconciliationId);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Employee")]
        [HttpGet]
        [Route("details/{patientId}")]
        public async Task<IActionResult> GetServiceTakewayMedReconciliationByPatientId(int patientId)
        {
            var response = await _serviceTakeAwayMedReconciliationService.GetServiceTakewayMedReconciliationByPatientId(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("sendpdf")]
        public async Task<IActionResult> SendCmrPdfEmailByMedReconciliation(string email, int patientId)
        {

            var response = await _serviceTakeAwayMedReconciliationService.SendCmrPdfEmailByMedReconciliation(email, patientId);
            return Ok(response);
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpGet]
        [Route("{patientId}/uuid")]
        public async Task<IActionResult> CheckLinkExpiredByMedReconciliation(int patientId, string uuid)
        {

            var response = await _serviceTakeAwayMedReconciliationService.CheckLinkExpiredByMedReconciliation(patientId, uuid);
            return Ok(response);

        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost]
        [Route("{patientId}/verification")]
        public async Task<IActionResult> PatientVerificationForPdfMedReconciliation(string lastname, DateTime dob, int patientId, string uuid)
        {

            var response = await _serviceTakeAwayMedReconciliationService.PatientVerificationForPdfMedReconciliation(lastname, dob, patientId, uuid);
            if (response.Success)
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
