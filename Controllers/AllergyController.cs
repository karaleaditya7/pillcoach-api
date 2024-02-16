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
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyService _allergyService;
        public AllergyController(IAllergyService allergyService)
        {
            _allergyService = allergyService;
        }
        
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientAllergy([FromBody] AllergyModel model)
        {
            var response = await _allergyService.AddPatientAllergy(model);

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
        public async Task<IActionResult> UpdatePatientAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            var response = await _allergyService.UpdatePatientAllergy(patientid, medicationsubstanceid, reactions);

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllReactions(int id)
        {
            var response = await _allergyService.GetReactionsByPatientId(id);
            return Ok(response);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/all/{id}")]
        public async Task<IActionResult> GetAllMedicationSubstancesBypatientId(int id)
        {
            var response = await _allergyService.GetMedicationsubstancesByPatientIdForAllergy(id);
            return Ok(response);

        }
        



        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("MedicationSubstance/Reaction/{patientId}")]
        public async Task<IActionResult> GetAllMedicationSubstanceAndReactionsBypatientId(int patientId)
        {
            var response = await _allergyService.GetMedicationsubstancesByPatientIdForAllergy(patientId);
            Dictionary<string, List<Allergy>> Pairs = new Dictionary<string, List<Allergy>>();
            foreach (MedicationSubstance medicationSubstance in response.DataList)
            {
                Pairs.Add(medicationSubstance.Name,  _allergyService.GetAllergyReactionsByMedicationSubstanceById(medicationSubstance.Id, patientId).Result.DataList);
            }

            return Ok(Pairs);

        }

        [Authorize(Roles = "Admin,Employee")]
        [Produces("application/json")]
        [HttpGet]
        [Route("Reaction/all/{id}/{patientId}")]
        public async Task<IActionResult> GetAllAllergyReactionByMedicationSubstancesId(int id,int patientId)
        {
            var response = await _allergyService.GetAllergyReactionsByMedicationSubstanceById(id ,patientId);
            return Ok(response);

        }
        [Authorize]
        [HttpDelete]
        [Route("{medicationSubstanceId}/{patientId}")]
        public async Task<IActionResult> DeleteMedicationSubstanceId(int medicationSubstanceId,int patientId)
        {
            var response = await _allergyService.DeleteMedicationSubstanceAndReactionsIdForAllergies(medicationSubstanceId, patientId);
            return Ok(response);
        }


    }
}
