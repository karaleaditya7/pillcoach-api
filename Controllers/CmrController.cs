using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CmrController : ControllerBase
    {
        private readonly ICmrMedicationService _cmrMedicationService;
        public CmrController(ICmrMedicationService cmrMedicationService)
        {
            _cmrMedicationService = cmrMedicationService;
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddCMRMedication([FromBody] CmrMedicationModel model)
        {
            var response = await _cmrMedicationService.AddCmrMedication(model);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{patientId}/cmrMedications")]
        public async Task<IActionResult> GetCmrMedicationsByPatientId(int patientId)
        {
            var response = await _cmrMedicationService.GetUniqueCmrMedicationsByPatientId(patientId);
            return Ok(response);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [Route("Medications")]
        public async Task<IActionResult> GetAllRxNavMedicationsBy()
        {
            var response = await _cmrMedicationService.SaveAllRxNavMedications();
            return Ok(response);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetCmrMedicationsById(int id)
        {
            var response = await _cmrMedicationService.GetCmrMedicationById(id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCmrMedicationsById(int id)
        {
            var response = await _cmrMedicationService.DeleteCmrMedicationById(id);
            return Ok(response);
        }


        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("{search}")]
        
        public async Task<IActionResult> SearchFunctionalityForMedication( string search)
        {
            var response =  await _cmrMedicationService.SearchForMedication(search);
            return Ok(response);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("doctor/{id}/all")]
        public async Task<IActionResult> GetAlldoctors(int id)
        {
            var response = await _cmrMedicationService.GetAllDoctors(id);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("conditions/{patientId}/all")]
        public async Task<IActionResult> GetAllConditionsForCmr(int patientId)
        {
            var response = await _cmrMedicationService.GetAllConditionsforCmrMedication(patientId);
            return Ok(response);
        }


        [Authorize(Roles = "Admin, Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateCmrMedication([FromBody] CmrMedicationModel model)
        {
            var response = await _cmrMedicationService.UpdateCmrMedication(model);
            return Ok(response);
        }
    }
}
