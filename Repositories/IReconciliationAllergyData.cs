using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IReconciliationAllergyData
    {
        Task<List<ReconciliationAllergy>> GetRecAllergyMedicationSubstanceId(string medicationSubstance, int patientId);

        Task<ReconciliationAllergy> AddPatientReconciliationAllergy(ReconciliationAllergy reconciliation, bool commitChanges);

        Task SaveChangesAsync();

        Task<List<ReconciliationAllergy>> GetReconciliationAllergyByAllergyId(int patientid, int medicationsubstanceid);

        Task DeleteReconciliationAllergyById(ReconciliationAllergy reconciliationAllergy);

        Task<List<Reaction>> GetAllRecAllergyReactionsByPatientId(int id);

        Task<List<MedicationSubstance>> GetMedSubstancesByPatientIdForRecAllergy(int id);

        Task<List<MedicationSubstance>> GetRecAllergyMedSubstancesByPatientIdForAllergy(int id);

        Task<List<ReconciliationAllergy>> GetAllRecAllergyReactionsByMedSubstanceById(int id, int patientId);

        Task<List<ReconciliationAllergy>> GetReconciliationAllergyById(int medicationSubstanceId, int patientId);

        Task<List<ReconciliationAllergy>> GetReconciliationAllergyByPatientId(int patientId);

        void DeleteRecAllergyByServiceTakeAwayMedReconciliation(ReconciliationAllergy reconciliationAllergy);

        void PatientDeleteForReconciliationAllergy(ReconciliationAllergy reconciliationAllergy);
    }
}
