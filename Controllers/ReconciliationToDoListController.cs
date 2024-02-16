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
    public class ReconciliationToDoListController : ControllerBase
    {
        private readonly IReconciliationToDoListService _reconciliationToDoListService;
        public ReconciliationToDoListController(IReconciliationToDoListService reconciliationToDoListService)
        {
            _reconciliationToDoListService = reconciliationToDoListService;
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientReconciliationToDoList([FromBody] ReconciliationToDoListModel model)
        {
            var response = await _reconciliationToDoListService.AddReconciliationToDoList(model);

            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientMedicationToDoList([FromBody] ReconciliationToDoListModel model)
        {
            var response = await _reconciliationToDoListService.UpdateReconciliationToDoList(model);

            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [HttpPut]
        [Route("NonRelatedReconciliationToDo")]
        public async Task<IActionResult> UpdateReconciliationNonMedicationToDoList([FromBody] NonRelatedRecocilationToDoModel model)
        {
            var response = await _reconciliationToDoListService.UpdateReconciliationNonMedicationToDoList(model);

            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("NonRelatedReconciliationToDo")]
        public async Task<IActionResult> AddPatientReconciliationNonMedicationToDoList([FromBody] NonRelatedRecocilationToDoModel model)
        {
            var response = await _reconciliationToDoListService.AddPatientRecNonMedicationToDoList(model);

            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationReconciliation/{patientId}")]
        public async Task<IActionResult> GetAllMedicationReconciliation(int patientId)
        {
            var response = await _reconciliationToDoListService.GetAllMedicationReconciliation(patientId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("NonRelatedReconciliationToDos/{patientId}")]
        public async Task<IActionResult> GetMedReconciliationNonRelatedRecToDo(int patientId)
        {
            var response = await _reconciliationToDoListService.GetMedRecNonRelatedRecocilationToDo(patientId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("actionItemReconciliationToDo/{patientId}")]
        public async Task<IActionResult> GetAllActionItemReconciliationToDo(int patientId)
        {
            var response = await _reconciliationToDoListService.GetAllActionItemReconciliationToDo(patientId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [HttpDelete]
        [Route("MedicationReconciliation/{medicationReconciliationId}")]
        public async Task<IActionResult> DeleteReconciliationToDoRelated(int medicationReconciliationId)
        {
            var response = await _reconciliationToDoListService.DeleteReconciliationToDoRelated(medicationReconciliationId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [HttpDelete]
        [Route("actionItemReconciliationToDo/{actionitemReconciliationToDoId}")]
        public async Task<IActionResult> DeleteActionitemReconciliationToDoId(int actionitemReconciliationToDoId)
        {
            var response = await _reconciliationToDoListService.DeleteActionitemReconciliationToDoId(actionitemReconciliationToDoId);
            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("ReconciliationToDoRelated/{medicationReconciliationId}")]
        public async Task<IActionResult> GetRecToDoListRelatedByMedReconciliationId(int medicationReconciliationId, int patientId)
        {
            var response = await _reconciliationToDoListService.GetRecToDoListRelatedByMedReconciliationId(medicationReconciliationId, patientId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("NonRelatedReconciliationToDo/{actionitemReconciliationToDoId}")]
        public async Task<IActionResult> GetNonRelatedRecToDoByActionitemRecToDoId(int actionitemReconciliationToDoId, int patientId)
        {
            var response = await _reconciliationToDoListService.GetNonRelatedRecToDoByActionitemRecToDoId(actionitemReconciliationToDoId, patientId);
            return Ok(response);
        }



        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllReconciliationToDoRelatedByPatientId(int patientId)
        {
            var response = await _reconciliationToDoListService.GetAllReconciliationToDoRelatedByPatientId(patientId);
            return Ok(response);
        }

    }
}
