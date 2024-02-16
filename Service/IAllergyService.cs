using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IAllergyService
    {
        Task<Response<Allergy>> AddPatientAllergy(AllergyModel model);
        Task<Response<Allergy>> UpdatePatientAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions);
        Task<Response<Reaction>> GetReactionsByPatientId(int id);
        Task<Response<MedicationSubstance>> GetMedicationsubstancesByPatientIdForAllergy(int id);
        Task<Response<Allergy>> GetAllergyReactionsByMedicationSubstanceById(int id, int patientId);

        Task<Response<Allergy>> DeleteMedicationSubstanceAndReactionsId(int medicationSubstanceId, int patientId);

        Task<Response<Allergy>> DeleteMedicationSubstanceAndReactionsIdForAllergies(int medicationSubstanceId, int patientId);
    }
}
