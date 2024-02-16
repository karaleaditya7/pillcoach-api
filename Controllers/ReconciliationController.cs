using iTextSharp.text;
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
    public class ReconciliationController : Controller
    {
        private readonly IReconciliationService _reconciliationService;

        public ReconciliationController(IReconciliationService reconciliationService)
        {
                _reconciliationService = reconciliationService;
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddReconciliationMedication([FromBody] MedicationRecocilationModel model)
        {
            var response = await _reconciliationService.AddReconciliationMedication(model);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{patientId}/Reconciliations")]
        public async Task<IActionResult> GetReconciliationMedicationsByPatientId(int patientId)
        {
            var response = await _reconciliationService.GetReconciliationMedicationsByPatientId(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [Route("Reconciliations")]
        public async Task<IActionResult> GetAllRxNavMedicationsBy()
        {
            var response = await _reconciliationService.SaveAllRxNavMedications();
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{id}/detail")]
        public async Task<IActionResult> GetMedicationsById(int id)
        {
            var response = await _reconciliationService.GetRecMedicationById(id);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteReconciliationMedicationsById(int id)
        {
            var response = await _reconciliationService.DeleteReconciliationMedicationsById(id);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{search}")]

        public IActionResult SearchFunctionalityForMedication(string search)
        {
            var response =  _reconciliationService.SearchForMedication(search);
            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("doctor/{id}/all")]
        public async Task<IActionResult> GetAlldoctors(int id)
        {
            var response = await _reconciliationService.GetAllDoctors(id);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("condition/{patientId}/all")]
        public async Task<IActionResult> GetAllConditionForMedRec(int patientId)
        {
            var response = await _reconciliationService.GetAllConditionsforMedRe(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Admin, Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdateReconciliationMedication([FromBody] MedicationRecocilationModel model)
        {
            var response = await _reconciliationService.UpdateReconciliationMedication(model);
            return Ok(response);
        }
    }
}
