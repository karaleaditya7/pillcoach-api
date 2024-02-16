
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IMedicationService _medicationService;
        private readonly IJwtUtils _jwtUtils;

        public MedicationController(IPatientService patientService, IMedicationService medicationService, IJwtUtils jwtUtils)
        {
            _patientService = patientService;
            _medicationService = medicationService;
            _jwtUtils = jwtUtils;
        }

        /// <summary>
        /// Update Medication Condition. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for updating medication condition by medication id.
        /// </remarks>
        [Authorize]
        [HttpPut]
        [Route("{medicationId}/condition")]
        public async Task<IActionResult> UpdateMedicationByCondition(int medicationId, [FromBody] MedicationModel model)
        {
            var response = await _patientService.UpdateMedicationByCondition(medicationId, model);
            return Ok(response);
        }


        /// <summary>
        /// Get All Medication list. Role[Admin]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting list of all medication.
        /// </remarks>

        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetUniqueNameMedicationWithGenericName([FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromHeader] string authorization, [FromQuery] string keywords, [FromQuery] string sortDirection)
        {
            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;

            var role =  _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            var response =await _medicationService.GetUniqueNameMedicationWithGenericName(recordNumber,pageLimit, role, userId,keywords,sortDirection);
            return Ok(response);
        }


        [Authorize]
        [HttpGet]
        [Route("condition/all")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetAllMedicationCondition()
        {
            var response = await _medicationService.GetAllMedicationCondition();
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        [Route("condition/{text}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> SearchForMedicationCondition(string text)
        {
            var response = await _medicationService.SerachForMedicationCondition(text);
            return Ok(response);
        }


        [Produces("application/json")]
        [Authorize]
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> GetCountForPatientsAndPharmacies([FromQuery]string genericName, [FromQuery] string sbdcName, [FromHeader] string authorization)
        {
            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;
            
            var role = _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            var response = await _medicationService.GetPatientPharmacyCountForMedication(genericName, sbdcName, role, userId);
            return Ok(response);
        }


        /// <summary>
        /// Get patient with NDCNumber related medication. Role[Admin]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting Patient of all medication of NdcNumber.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("patient")]
        public async Task<IActionResult> GetPatientByNDCNumber([FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] int Month,[FromQuery]string genericName, [FromQuery] string sbdcName, [FromHeader] string authorization, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, Month);
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;

            var role = _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            if (Roles.SuperAdmin.ToString().Equals(role, StringComparison.OrdinalIgnoreCase))
                userId = null;

            var response = await _medicationService.GetPatientByUniqueMedication(recordNumber, pageLimit, Month, startDate,endDate, genericName,sbdcName, userId,keywords,sortDirection,filterType,filterValue,filterCategory);
            return Ok(response);
        }


        /// <summary>
        /// Get All patient list with userId . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting list of all Patient for a userId.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{userId}/patients")]
        public async Task<IActionResult> GetPatientsUserId(string userId)
        {
            var response = await _medicationService.GetPatientsByUserId(userId);
            
            return Ok(response);
        }



        [Authorize]
        [HttpGet]
        [Route("details/{rxNumber}")]
        public async Task<IActionResult> GetMedicationDetails(string rxNumber)
        {
            var response = await _medicationService.GetMedicationdetails(rxNumber);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("{patientId:int}/history")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetMedicationHistory(int patientId, [FromBody] string drugName)
        {
            var response = await _medicationService.GetPatientMedicationHistoryByDrugNameAsync(patientId, drugName);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("{medicationId:int}/usage-status")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> SetMedicationInUseStatus(int medicationId, [FromBody] bool status)
        {
            if (medicationId <= 0) return BadRequest();

            var response = await _medicationService.SetMedicationUsageStatusAsync(medicationId, status);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        [Route("{medicationId:int}/refill-due")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> SetMedicationRefillDueStatus(int medicationId, [FromBody] bool status)
        {
            if (medicationId <= 0) return BadRequest();

            var response = await _medicationService.SetMedicationRefillDueStatusAsync(medicationId, status);

            return Ok(response);
        }
    }
}
