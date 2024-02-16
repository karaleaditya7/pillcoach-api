using Microsoft.AspNetCore.Mvc;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IAllergyData
    {
        Task<Allergy> AddPatientAllergy(Allergy allergy,bool commitChanges);
        Task<List<int>> GetReactionyByAllergyId(int allergyid);
        Task<Allergy> UpdateReactionByAllergy(Allergy allergy);

        Task<Reaction> GetReactionByReactionId(int reactionId);

        Task<Allergy> GetAllergyByAllergyId(int allergyid);
        Task<List<Allergy>> GetAllergyByAllergyId(int patientid, int medicationsubstanceid);

        Task<List<Reaction>> GetAllReactionsByPatientId(int id);
        Task<List<MedicationSubstance>> GetMedicationSubstancesByPatientIdForAllergy(int id);
        Task<List<Allergy>> GetAllAllergyReactionsByMedicationSubstanceById(int id, int patientId);

        Task DeleteAllergyById(Allergy allergy);

        Task<List<Allergy>> GetAllergyById(int medicationSubstanceId, int patientId);

        Task<MedicationSubstance> GetAllergyMedicationSubstanceByAllergyId(int medicationSubstanceId, int patientId);
        Task<List<Reaction>> GetAllAllergyReactionsByMedicationSubstanceByAndPatientById(int medicationSubstanceId, int patientId);

        void DeleteReactionById(Reaction reaction);
        void DeleteMedicationSubstanceById(MedicationSubstance medicationSubstance);

        Task<List<Allergy>> GetAllergyMedicationSubstanceId(string medicationSubstance , int patientId);

        Task<List<Allergy>> GetAllergyByPatientId(int patientId);

        void DeleteAllergyByServiceTakeAway(Allergy allergy);
        Task<List<Allergy>> GetAllAllergyByPatientId(int patientId);
        void PatientDeleteForAllergy(Allergy allergy);

        void DeleteMedicationSubstanceForServiceTakeAwayId(MedicationSubstance medicationSubstance);
       

        Task SaveChangesAsync();
    }
}
