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
    public class ReconciliationAllergyController : ControllerBase
    {
        private readonly IReconciliationAllergyService _reconciliationAllergyService;

        public ReconciliationAllergyController(IReconciliationAllergyService reconciliationAllergyService)
        {
            _reconciliationAllergyService = reconciliationAllergyService;
        }
        
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientReconciliationAllergy([FromBody] ReconciliationAllergyModel model)
        {
            var response = await _reconciliationAllergyService.AddPatientReconciliationAllergy(model);

            return Ok(response);
        }

        /* [Authorize(Roles = "Admin,Employee")]
         [HttpPut]
         public async Task<IActionResult> UpdatePatientAllergy([FromBody] AllergyModel model)
         {
             var response = await _allergyService.UpdatePatientAllergy(model);

             return Ok(response);
         }
 */
        [Authorize(Roles = "Admin,Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientReconciliationAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            var response = await _reconciliationAllergyService.UpdatePatientReconciliationAllergy(patientid, medicationsubstanceid, reactions);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllReconciliationAllergyReactions(int id)
        {
            var response = await _reconciliationAllergyService.GetAllReconciliationAllergyReactions(id);
            return Ok(response);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/all/{id}")]
        public async Task<IActionResult> GetAllReconciliationAllergyMedicationSubstancesBypatientId(int id)
        {
            var response = await _reconciliationAllergyService.GetAllRecAllergyMedSubstancesBypatientId(id);
            return Ok(response);

        }




        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/Reaction/{patientId}")]
        public async Task<IActionResult> GetAllRecAllergyMedSubstanceAndRecAllergyReactionsBypatientId(int patientId)
        {
            var response = await _reconciliationAllergyService.GetAllRecAllergyMedSubstanceAndRecAllergyReactionsBypatientId(patientId);
            Dictionary<string, List<ReconciliationAllergy>> Pairs = new Dictionary<string, List<ReconciliationAllergy>>();
            foreach (MedicationSubstance medicationSubstance in response.DataList)
            {
                Pairs.Add(medicationSubstance.Name, _reconciliationAllergyService.GetRecAllergyReactionsByMedSubstanceById(medicationSubstance.Id, patientId).Result.DataList);
            }

            return Ok(Pairs);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("Reaction/all/{id}/{patientId}")]
        public async Task<IActionResult> GetAllRecAllergyReactionByMedSubstancesId(int id, int patientId)
        {
            var response = await _reconciliationAllergyService.GetAllRecAllergyReactionByMedSubstancesId(id, patientId);
            return Ok(response);

        }
        [Authorize]
        [HttpDelete]
        [Route("{medicationSubstanceId}/{patientId}")]
        public async Task<IActionResult> DeleteReconciliationAllergyMedicationSubstanceId(int medicationSubstanceId, int patientId)
        {
            var response = await _reconciliationAllergyService.DeleteReconciliationAllergyMedicationSubstanceId(medicationSubstanceId, patientId);
            return Ok(response);
        }


    }
}
