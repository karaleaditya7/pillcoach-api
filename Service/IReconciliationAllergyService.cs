using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IReconciliationAllergyService
    {
        Task<Response<ReconciliationAllergy>> AddPatientReconciliationAllergy(ReconciliationAllergyModel model);

        Task<Response<ReconciliationAllergy>> UpdatePatientReconciliationAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions);

        Task<Response<Reaction>> GetAllReconciliationAllergyReactions(int id);

        Task<Response<MedicationSubstance>> GetAllRecAllergyMedSubstancesBypatientId(int id);

        Task<Response<MedicationSubstance>> GetAllRecAllergyMedSubstanceAndRecAllergyReactionsBypatientId(int id);

        Task<Response<ReconciliationAllergy>> GetRecAllergyReactionsByMedSubstanceById(int id, int patientId);
        Task<Response<ReconciliationAllergy>> GetAllRecAllergyReactionByMedSubstancesId(int id, int patientId);

        Task<Response<ReconciliationAllergy>> DeleteReconciliationAllergyMedicationSubstanceId(int medicationSubstanceId, int patientId);
    }
}
