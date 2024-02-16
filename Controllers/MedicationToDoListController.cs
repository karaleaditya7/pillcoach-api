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
    public class MedicationToDoListController : ControllerBase
    {
        private readonly IMedicationToDoListService _medicationToDoListService;
        public MedicationToDoListController(IMedicationToDoListService medicationToDoListService)
        {
            _medicationToDoListService = medicationToDoListService;
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientMedicationToDoList([FromBody] MedicationToDoListModel model)
        {
            var response = await _medicationToDoListService.AddMedicationToDoList(model);

            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientMedicationToDoList([FromBody] MedicationToDoListModel model)
        {
            var response = await _medicationToDoListService.UpdateMedicationToDoList(model);

            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [HttpPut]
        [Route("NonMedicationRelated")]
        public async Task<IActionResult> UpdatePatientNonMedicationToDoList([FromBody] MedicationToDoListNonRelatedModel model)
        {
            var response = await _medicationToDoListService.UpdateNonMedicationToDoList(model);

            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [HttpPost]
        [Route("NonMedicationRelated")]
        public async Task<IActionResult> AddPatientNonRelatedMedicationToDoList([FromBody] MedicationToDoListNonRelatedModel model)
        {
            var response = await _medicationToDoListService.AddMedicationToDoListNonRelated(model);

            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("cmrMedication/{patientId}")]
        public async Task<IActionResult> GetAllCmrMedicationRelated(int patientId)
        {
            var response = await _medicationToDoListService.getAllCmrMedicationRelated(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("NonMedicationRelated/{patientId}")]
        public async Task<IActionResult> GetCmrNonMedicationRelated(int patientId)
        {           
            var response = await _medicationToDoListService.getCmrNonMedicationRelated(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("actionItemToDo/{patientId}")]
        public async Task<IActionResult> GetAllActionItemToDo(int patientId)
        {
            var response = await _medicationToDoListService.getAllActionItemToDo(patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete]
        [Route("cmrMedication/{cmrMedicationId}")]
        public async Task<IActionResult> DeleteMedicationToDoRelated(int cmrMedicationId)
        {
            var response = await _medicationToDoListService.DeleteMedicationToDoRelated(cmrMedicationId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete]
        [Route("actionItemToDo/{actionitemToDoId}")]
        public async Task<IActionResult> DeleteActionitemToDoId(int actionitemToDoId)
        {
            var response = await _medicationToDoListService.DeleteActionitemToDoId(actionitemToDoId);
            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationToDoListRelated/{cmrMedicationId}")]
        public async Task<IActionResult> GetMedicationToDoListRelatedByCmrMedicationId(int cmrMedicationId , int patientId)
        {
            var response = await _medicationToDoListService.getMedicationToDoListRelatedByCmrMedicationIdByPatientId(cmrMedicationId, patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("NonMedicationToDoListRelated/{actionitemToDoId}")]
        public async Task<IActionResult> GetNonMedicationToDoListRelatedByActionitemToDoId(int actionitemToDoId , int patientId)
        {
            var response = await _medicationToDoListService.getNonMedicationToDoListRelatedByCmrMedicationIdByPatientId(actionitemToDoId , patientId);
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllMedicationToDoListRelatedByPatientId( int patientId)
        {
            var response = await _medicationToDoListService.getAllMedicationToDoList(patientId);
            return Ok(response);
        }


    }
}
