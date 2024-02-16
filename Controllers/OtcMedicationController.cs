using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtcMedicationController : Controller
    {
        private readonly IOtcMedicationService _otcMedicationService;
        public OtcMedicationController(IOtcMedicationService otcMedicationService)
        {
            _otcMedicationService = otcMedicationService;
        }
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> AddOtcMedication([FromBody] OtcMedicationModel model)
        {
            var response = await _otcMedicationService.AddOtcMedication(model);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("{patientId}/OtcMedications")]
        public async Task<IActionResult> GetOtcMedicationsByPatientId([FromQuery] int recordNumber, [FromQuery] int pageLimit, int patientId)
        {
            var response = await _otcMedicationService.GetOtcMedicationsByPatientId(recordNumber, pageLimit,patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpPut]
        public async Task<IActionResult> UpdateOtcMedication([FromBody] OtcMedicationModel model)
        {
            var response = await _otcMedicationService.UpdateOtcMedication(model);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("doctor/{patientId}/all")]
        public async Task<IActionResult> GetAlldoctorsforOtcMedication(int patientId)
        {
            var response = await _otcMedicationService.GetAlldoctorsforOtcMedication(patientId);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("condition/{patientId}/all")]
        public async Task<IActionResult> GetAllConditionsforOtcMedication(int patientId)
        {
            var response = await _otcMedicationService.GetAllConditionsforOtcMedication(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOtcMedicationsById(int id)
        {
            var response = await _otcMedicationService.DeleteOtcMedicationById(id);
            return Ok(response);
        }
    }
}
