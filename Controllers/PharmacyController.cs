using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authorization;
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

    [Route("api/[controller]")]
    [ApiController]
    public class PharmacyController : ControllerBase
    {
        private readonly IPharmacyService _pharmacyService;
        private readonly IJwtUtils _jwtUtils;
        private readonly IUserService _userService;
        private readonly IPatientService _patientService;
        readonly IPatientPdcService _patientPdcService;

        public PharmacyController(IPharmacyService pharmacyService, IJwtUtils jwtUtils, IUserService userService, IPatientService patientService, IPatientPdcService patientPdcService)
        {
            _pharmacyService = pharmacyService;
            _jwtUtils = jwtUtils;
            _userService = userService;
            _patientService = patientService;
            _patientPdcService = patientPdcService;
        }

        /// <summary>
        /// Get All pharamcy list. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting list of all pharamacy
        /// for Admin, it will return all pharmacy list.
        /// for Employee, it will return all pharmacy which are assigned to him.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllPharmacies([FromQuery] int recordNumber, [FromQuery] int pageLimit,[FromHeader] string authorization, [FromQuery] int month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {

            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role =  _jwtUtils.GetRole(parameter);
            var userID = _jwtUtils.ValidateToken(parameter);

            if (role == Roles.SuperAdmin.ToString())
            {
                var response = await _pharmacyService.GetPharmacies(recordNumber,pageLimit, startDate,endDate,month,keywords,sortDirection, filterType, filterValue, filterCategory);
                return Ok(response);
            }
            else
            {
                var response = await _pharmacyService.GetPharmaciesByUserIDWithPagination(recordNumber, pageLimit,userID, startDate, endDate, month,keywords,sortDirection, filterType, filterValue, filterCategory);
                return Ok(response);
            }
        }

        /// <summary>
        /// Get All pharamcy list. Role[SuperAdmin]
        /// This API Will be used for getting list of all pharmacy
        /// </summary> 
        [Authorize]
        [HttpGet]
        [Route("all/pharmacyNames")]
        public async Task<IActionResult> GetAllPharmacyNames([FromHeader] string authorization)
        {
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role = _jwtUtils.GetRole(parameter);
            var userID = _jwtUtils.ValidateToken(parameter);

            if (role == Roles.SuperAdmin.ToString())
            {
                var response = await _pharmacyService.GetAllPharmacyNames();
                return Ok(response);
            }

            return Ok("Not a SuperAdmin");
        }
        /// <summary>
        /// Delete pharmacy. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will soft delete pharamacy by ID
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [Produces("application/json")]
        [HttpDelete]
        [Route("{pharmacyId}")]
        public async Task<IActionResult> DeletePharmacy(int pharmacyId)
        {
            var response = await _pharmacyService.DeletePharmacy(pharmacyId);
            return Ok(response);
        }

        /// <summary>
        /// Get pharmacy by pharmacy id. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will soft delete pharamacy by ID
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetPharmacyById(int id,[FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate,month);
            var response = await _pharmacyService.GetPharmacyById(id,startDate,endDate,month);
            return Ok(response);
        }

        /// <summary>
        /// Get pharmacy disease count by pharmacy id.
        /// </summary>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet]
        [Route("{id}/count")]
        public async Task<IActionResult> GetPatientDiseaseCountById(int id, [FromQuery] int month, [FromQuery] string condition)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var patientCount =  await _patientService.GetPatientDiseaseCountById(id, startDate, endDate, month, condition);
            return Ok(patientCount);
        }

        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet]
        [Route("{id}/PDC")]
        public async Task<IActionResult> GetPharmacyByIdForPDC(int id, [FromQuery] int month)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            var response = await _pharmacyService.GetPharmacyByIdForPDCForDto(id, startDate, endDate, month);
            return Ok(response);
        }


        /// <summary>
        /// Get list of patient by Pharamcy ID. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will return list of patients belongs to given pharamcy.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{pharmacyId}/patients")]
        public async Task<IActionResult> GetPatientsByPharmacyId(int pharmacyId)
        {
            var response = await _patientService.GetPatientsByPharmacyId(pharmacyId);
            return Ok(response);
        }




        /// <summary>
        /// Get patient by pharmacyid and condition. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will return list of patients belongs to given pharamcy with condition. ex: RASA, Diabitis, Cholestrol.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{pharmacyId}/patients/{condition}")]
        public async Task<IActionResult> GetPharmacyPatientByCondition([FromQuery] int recordNumber, [FromQuery] int pageLimit, int pharmacyId, string condition,[FromQuery] int Month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, Month);
            var response = await _patientService.GetPharmacyPatientsByCondition(recordNumber, pageLimit,startDate,endDate, pharmacyId, condition,Month,keywords,sortDirection,filterType,filterValue,filterCategory);
            return Ok(response);
        }


        /// <summary>
        /// Get allPatient by pharmacyid and condition.
        /// </summary>
        /// <remarks>
        /// This API Will return all patient of given pharmacy with condition. ex: RASA, Diabetes, Cholesterol.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{pharmacyId}/{condition}/count")]
        public async Task<IActionResult> GetAllPharmacyPatientByCondition([FromQuery] int recordNumber, [FromQuery] int pageLimit, int pharmacyId, string condition, [FromQuery] int Month, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, Month);
            var response = await _patientService.GetAllPharmacyPatientsByCondition(recordNumber, pageLimit, startDate, endDate, pharmacyId, condition, Month, keywords, sortDirection, filterType, filterValue, filterCategory);
            return Ok(response);
        }



        /// <summary>
        /// For Adding a pharmacy. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Adding a pharmacy.
        /// </remarks>
        [Authorize(Roles = "Admin, SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddPharmacy([FromHeader] string authorization, [FromBody] PharmacyModel model)
        {
            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
                parameter = headerValue.Parameter;

            var role = _jwtUtils.GetRole(parameter);
            var userId =  _jwtUtils.ValidateToken(parameter);

            var response = await _pharmacyService.AddPharmacy(model, Roles.SuperAdmin.ToString().Equals(role, StringComparison.OrdinalIgnoreCase) ? null : userId);

            return Ok(response);
        }

        /// <summary>
        /// For Update a pharmacy. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Update a pharmacy.
        /// </remarks>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePharmacy([FromBody] PharmacyModel model)
        {
            var response = await _pharmacyService.UpdatePharmacy(model);
            return Ok(response);
        }

        /// <summary>
        /// For Adding a note for pharmacy. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Adding a note for pharmacy.
        /// </remarks>
        [Authorize]
        [HttpPost]
        [Route("note/{pharmacyId}")]
        public async Task<IActionResult> AddPharmacyNote([FromBody] NoteModel noteModel, int pharmacyId)
        {
            var response = await _pharmacyService.AddPharmacyNote(noteModel, pharmacyId);
            return Ok(response);
        }

        /// <summary>
        /// For graph of pharmacy performance. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for graph of pharmacy performance.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [Route("{pharmacyId}/graph")]
        public async Task<IActionResult> Graph([FromQuery] int month, int pharmacyId)
        {
            var cholesterolPdcList = new List<PdcModel>();
            var daibetesPdcList = new List<PdcModel>();
            var rasaPdcList = new List<PdcModel>();

            var range = _patientPdcService.GetMedicationPeriodForGraph(month);
            var startDate = range.Item1;
            var endDate = range.Item2;

            while (startDate <= endDate)
            {
                cholesterolPdcList.Add(await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.Cholesterol.ToString(), startDate, month, PdcQueryType.ByEndDate));
                daibetesPdcList.Add(await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.Diabetes.ToString(), startDate, month, PdcQueryType.ByEndDate));
                rasaPdcList.Add(await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.RASA.ToString(), startDate, month, PdcQueryType.ByEndDate));

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
    }
}
