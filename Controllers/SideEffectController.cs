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
    public class SideEffectController : ControllerBase
    {
        private readonly ISideEffectService _sideEffectService;
        public SideEffectController(ISideEffectService sideEffectService)
        {
            _sideEffectService = sideEffectService; 
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientSideEffect([FromBody] SideEffectModel model)
        {
            var response = await _sideEffectService.AddPatientSideEffects(model);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            var response = await _sideEffectService.UpdatePatientSideEffect(patientid, medicationsubstanceid, reactions);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllSideEffectReactions(int id)
        {
            var response = await _sideEffectService.GetSideEffectReactionsByPatientId(id);
            return Ok(response);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/Reaction/{patientId}")]
        public async Task<IActionResult> GetAllMedicationSubstanceAndReactionsBypatientId(int patientId)
        {
            var response = await _sideEffectService.GetMedicationsubstancesByPatientIdForSideEffect(patientId);
            Dictionary<string, List<SideEffect>> Pairs = new Dictionary<string, List<SideEffect>>();
            foreach (MedicationSubstance medicationSubstance in response.DataList)
            {
                Pairs.Add(medicationSubstance.Name, _sideEffectService.GetAllSideEffectReactionsByMedicationSubstanceById(medicationSubstance.Id, patientId).Result.DataList);
            }

            return Ok(Pairs);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/all/{id}")]
        public async Task<IActionResult> GetAllMedicationSubstancesBypatientIdForSideEffect(int id)
        {
            var response = await _sideEffectService.GetMedicationsubstancesByPatientIdForSideEffect(id);
            return Ok(response);

        }


        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("Reaction/all/{id}/{patientId}")]
        public async Task<IActionResult> GetAllSideEffectReactionsByMedicationSubstancesId(int id, int patientId)
        {
            var response = await _sideEffectService.GetAllSideEffectReactionsByMedicationSubstanceById(id,patientId);
            return Ok(response);

        }
        [Authorize]
        [HttpDelete]
        [Route("{medicationSubstanceId}/{patientId}")]
        public async Task<IActionResult> DeleteSideEffectBySideEffectId(int medicationSubstanceId,int patientId)
        {
            var response = await _sideEffectService.DeleteSideEffectByMedicationSubstanceIdAndPatientId(medicationSubstanceId, patientId);
            return Ok(response);
        }


    }
}
