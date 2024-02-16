using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReconciliationSideEffectController : ControllerBase
    {

        private readonly IReconciliationSideEffectService  _reconciliationSideEffectService;
        public ReconciliationSideEffectController(IReconciliationSideEffectService reconciliationSideEffectService)
        {
            _reconciliationSideEffectService = reconciliationSideEffectService;
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientReconciliationSideEffect([FromBody] ReconciliationSideEffectModel model)
        {
            var response = await _reconciliationSideEffectService.AddPatientReconciliationSideEffect(model);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientReconciliationSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            var response = await _reconciliationSideEffectService.UpdatePatientReconciliationSideEffect(patientid, medicationsubstanceid, reactions);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllReconciliationSideEffectReactions(int id)
        {
            var response = await _reconciliationSideEffectService.GetAllReconciliationSideEffectReactions(id);
            return Ok(response);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/Reaction/{patientId}")]
        public async Task<IActionResult> GetAllMedSubstanceAndReactionsBypatientId(int patientId)
        {
            var response = await _reconciliationSideEffectService.GetRecMedsubstancesByPatientIdForSideEffect(patientId);
            Dictionary<string, List<ReconciliationSideeffect>> Pairs = new Dictionary<string, List<ReconciliationSideeffect>>();
            foreach (MedicationSubstance medicationSubstance in response.DataList)
            {
                Pairs.Add(medicationSubstance.Name, _reconciliationSideEffectService.GetAllRecSideEffectReactionsByMedSubstanceById(medicationSubstance.Id, patientId).Result.DataList);
            }

            return Ok(Pairs);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/all/{id}")]
        public async Task<IActionResult> GetAllRecMedSubstancesBypatientIdForSideEffect(int id)
        {
            var response = await _reconciliationSideEffectService.GetRecMedsubstancesByPatientIdForSideEffect(id);
            return Ok(response);

        }


        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("Reaction/all/{id}/{patientId}")]
        public async Task<IActionResult> GetAllRecSideEffectReactionsByMedSubstancesId(int id, int patientId)
        {
            var response = await _reconciliationSideEffectService.GetAllRecSideEffectReactionsByMedSubstanceById(id, patientId);
            return Ok(response);

        }

        [Authorize]
        [HttpDelete]
        [Route("{medicationSubstanceId}/{patientId}")]
        public async Task<IActionResult> DeleteRecSideEffectByRecSideEffectId(int medicationSubstanceId, int patientId)
        {
            var response = await _reconciliationSideEffectService.DeleteRecSideEffectByRecSideEffectId(medicationSubstanceId, patientId);
            return Ok(response);
        }
    }
}
