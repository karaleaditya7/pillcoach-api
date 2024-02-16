using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IReconciliationData
    {

        Task<MedicationReconciliation> AddReconciliationMedication(MedicationReconciliation medicationReconciliation);

        Task<List<MedicationReconciliation>> GetUniqueReconciliationMedicationsByPatientId(int patientId);

        Task<MedicationReconciliation> UpdateReconciliationMedication(MedicationReconciliation medicationReconciliation);

        Task<MedicationReconciliation> GetReconciliationMedicationsById(int id);

        Task DeleteReconciliationMedication(MedicationReconciliation medicationReconciliation);

        List<string> SearchForRxNavMedicationSBDC(string text);
            
        List<string> SearchForRxNavMedicationGPCK(string text);

        Task<List<Doctor>> GetAllDoctors(int id);

        void DeleteReconciliationMedicationForServiceTakeAwayMedReconciliation(MedicationReconciliation medicationReconciliation);

        Task<ReconciliationToDoRelated> GetMedicationToDoListRelatedByMedicationReconciliationId(int medicationReconciliationId);

        void PatientDeleteReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<List<string>> GetUniqueConditionForMedRecByPatientId(int patientId);

        Task<List<MedicationReconciliation>> GetMedReconciliationByPatientId(int patientId);

        void PatientDeleteForMedicationReconciliation(MedicationReconciliation medicationReconciliation);

        void DeleteMedRecForPatient(MedicationReconciliation medicationReconciliation);
    }
}
