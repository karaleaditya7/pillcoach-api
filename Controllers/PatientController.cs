using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
   [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IUserService _userService;
        private readonly IJwtUtils _jwtUtils;
        readonly IPatientPdcService _patientPdcService;

        public PatientController(IPatientService patientService, IUserService userService, IJwtUtils jwtUtils, IPatientPdcService patientPdcService)
        {
            _patientService = patientService;
            _jwtUtils = jwtUtils;
            _userService = userService;
            _patientPdcService = patientPdcService;
        }

        /// <summary>
        /// Get All Patient list. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting list of all patient.
        /// For Employee: This API will be used for getting list of all patient of assighned pharmacy.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllPatients([FromQuery] int recordNumber, [FromQuery] int pageLimit,[FromHeader] string authorization,[FromQuery]int month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);

            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role =  _jwtUtils.GetRole(parameter);
            var userID =  _jwtUtils.ValidateToken(parameter);

            if (role == Roles.SuperAdmin.ToString())
            {
                var response = await _patientService.GetPatients(recordNumber, pageLimit,startDate, endDate, month,keywords,sortDirection,filterType,filterValue,filterCategory);
                return Ok(response);
            }
            else
            {
                var response = await _patientService.GetPatientsByUserIdWithPagination(recordNumber, pageLimit, userID, startDate,endDate, month,keywords,sortDirection, filterType, filterValue, filterCategory);
                return Ok(response);
            }
        }

        /// <summary>
        /// For Delete a patient. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used  For Delete a patient.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpDelete]
        [Route("{patientId}")]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            var response = await _patientService.DeletePatient(patientId);
            return Ok(response);
        }

        /// <summary>
        ///  For Add  a patient. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used For Add  a patient.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] PatientModel model)
        {
            var response = await _patientService.AddPatient(model);
            return Ok(response);
        }

        /// <summary>
        ///  For update  a patient. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used For Update  a patient.
        /// </remarks>
        [Produces("application/json")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatient([FromBody] EditPatientModel model)
        {
            var response = await _patientService.UpdatePatient(model);
            return Ok(response);
        }


        [HttpPut]
        [Route("doctor")]
        public async Task<IActionResult> UpdateDoctor([FromBody] DoctorModel model)
        {
            var response = await _patientService.UpdateDoctor(model);
            return Ok(response);
        }

        /// <summary>
        ///  For Getting Details of a patient. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used For Getting Details of a patient.
        /// </remarks>
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetPatientById([FromQuery] int recordNumber, [FromQuery] int pageLimit, int id , [FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate,month);
            var response = await _patientService.GetPatientById(recordNumber, pageLimit,id, startDate,endDate,month);
            return Ok(response);
        }


        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]

        [Route("{id}/Medications")]
        public async Task<IActionResult> GetPrescribedMedicationsByPatientId([FromQuery] int recordNumber, [FromQuery] int pageLimit, int id)
        {
            var response = await _patientService.GetPrescribedMedicationsBypatientId(recordNumber, pageLimit, id);
            return Ok(response);
        }

        [HttpGet]
        [Route("info")]
        public async Task<IActionResult> GetPatientById([FromQuery] string contactnumber)
        {
            var response = await _patientService.GetPatientByContactNumber(contactnumber);
            return Ok(response);
        }

        [Produces("application/json")]
        [HttpGet]
        [Route("{id}/PDC")]
        public async Task<IActionResult> GetPatientByIdForPDCForDto(int id, [FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _patientService.GetPatientByIdForPDCForDto(id, startDate, endDate, month);
            return Ok(response);
        }


        [HttpGet]
        [Route("{userId}/{patientStatus}")]
        public async Task<IActionResult> GetPatientsByPatientStatus([FromQuery] int month,string userId,string patientStatus)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _patientService.GetPatientsByPatientStatus(userId, startDate, endDate,patientStatus,month);
            return Ok(response);
        }

        [HttpGet]
        [Route("list-by-status/{patientStatus}")]
        public async Task<IActionResult> GetPatientListByStatus([FromHeader] string authorization, string patientStatus, [FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] int month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;
            
            var role = _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            var response = await _patientService.GetPatientListByStatusAsync(Roles.SuperAdmin.ToString().Equals(role, StringComparison.OrdinalIgnoreCase) ? null : userId, patientStatus, recordNumber, pageLimit, month,startDate, endDate, keywords,sortDirection,filterType,filterValue,filterCategory);

            return Ok(response);
        }

        [HttpGet]
        [Route("{userId}/DueforRefills")]
        public async Task<IActionResult> GetPatientsByDueforRefills([FromQuery] int month, string userId)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _patientService.GetPatientsByDueforRefills(userId, startDate, endDate,month);
            return Ok(response);
        }


        [HttpGet]
        [Route("{userId}/NonAdherence/{condition}")]
        public async Task<IActionResult> GetNonAdherencePatientsByUserId([FromQuery] int month, string userId,string condition, [FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _patientService.GetNonAdherencePatientsForUser(userId, condition, startDate, endDate, month,recordNumber,pageLimit,keywords,sortDirection,filterType,filterValue,filterCategory);
            return Ok(response);
        }

        [HttpGet]
        [Route("{userId}/NoRefillRemaining")]
        public async Task<IActionResult> GetPatientsByNoRefillRemaining([FromQuery] int month, string userId)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _patientService.GetPatientsByNoRefillRemaining(userId, startDate, endDate,month);
            return Ok(response);
        }


        /// <summary>
        /// Get All medication list for patient. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting list of all medication given to patient
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{patientId}/Medication")]
        public async Task<IActionResult> GetMedicationsByPatientId(int patientId)
        {
            var response = await _patientService.GetMedicationByPatientId(patientId);
            return Ok(response);
        }

        /// <summary>
        /// Update status of patient. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Updating status of patient, New Patient, In Progress, Completed
        /// </remarks>
        [HttpPut]
        [Route("status")]
        public async Task<IActionResult> UpdatePatientStatus([FromBody] PatientModel patientModel)
        {
            var response = await _patientService.UpdatePatientStatus(patientModel);
            return Ok(response);
        }

        /// <summary>
        /// For Update Medication by condition. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Update Medication by condition.
        /// </remarks>
        [HttpPut]
        [Route("Medication/{medicationId}")]
        public async Task<IActionResult> UpdateMedicationByCondition(int medicationId, [FromBody] MedicationModel model)
        {
            var response = await _patientService.UpdateMedicationByCondition(medicationId, model);
            return Ok(response);
        }

        /// <summary>
        /// For Getting Upcoming Refilles. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Getting Upcoming Refilles.
        /// </remarks>
        [HttpPost]
        [Route("Medication/refills")]
        public async Task<IActionResult> GetUpcomingRefillesByCondtion([FromBody] MedicationModel model)
        {
            var response = await _patientService.GetMedicationByCondition(model.Condition);
            return Ok(response);
        }

        /// <summary>
        /// For Getting List of patients. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Getting List of patients.
        /// </remarks>
        [HttpGet]
        [Route("list/{patientId}")]
        public async Task<IActionResult> GetPatientsByPharmacyId(int patientId)
        {
            var response = await _patientService.GetPatientsByPharmacyId(patientId);
            return Ok(response);
        }


        /// <summary>
        /// For Adding patient Note. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used Adding patient Note.
        /// </remarks>
        [HttpPost]
        [Route("note/{patientId}")]
        public async Task<IActionResult> AddPatientNote([FromBody] NoteModel noteModel, int patientId)
        {
            var response = await _patientService.AddPatientNote(noteModel, patientId);
            return Ok(response);
        }



        /// <summary>
        /// For Getting graph of patient performance. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Getting graph of patient performance.
        /// </remarks>
        [HttpGet]
        [Route("{patientId}/graph")]
        public async Task<IActionResult> Graph([FromQuery] int month, int patientId)
        {
            var cholesterolPdcList = new List<PdcModel>();
            var daibetesPdcList = new List<PdcModel>();
            var rasaPdcList = new List<PdcModel>();

            var range = _patientPdcService.GetMedicationPeriodForGraph(month);
            var startDate = range.Item1;
            var endDate = range.Item2;

            while (startDate <= endDate)
            {
                cholesterolPdcList.Add(await _patientPdcService.GetPdcForPatientAsync(patientId, PDC.Cholesterol.ToString(), startDate, month, PdcQueryType.ByEndDate));
                daibetesPdcList.Add(await _patientPdcService.GetPdcForPatientAsync(patientId, PDC.Diabetes.ToString(), startDate, month, PdcQueryType.ByEndDate));
                rasaPdcList.Add(await _patientPdcService.GetPdcForPatientAsync(patientId, PDC.RASA.ToString(), startDate, month, PdcQueryType.ByEndDate));

                startDate = startDate.AddMonths(1);
            }

            var pairs = new Dictionary<string, List<PdcModel>>
            {
                { "Cholesterol", cholesterolPdcList },
                { "Diabetes", daibetesPdcList },
                { "RASA", rasaPdcList }
            };

            return Ok(pairs);
        }

        [Authorize]
        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> getUserListByPatientId(int patientId)
        {
            var response = await _userService.getUserListByPatientId(patientId);
            return Ok(response);
        }

        [Authorize]
        [HttpPut]
        [Route("{patientId:int}/consent/{consentType}/{flag:int}")]
        public async Task<IActionResult> UpdateConsentAsync(int patientId, string consentType, int flag)
        {
            var result = await _patientService.UpdatePatientConsentAsync(patientId, consentType, flag == 1);

            return Ok(result);
        }
    }
}
