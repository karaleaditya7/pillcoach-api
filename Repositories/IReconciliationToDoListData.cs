using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IReconciliationToDoListData
    {
        Task<ReconciliationToDoRelated> GetReconciliationToDoListRelatedByCmrMedicationId(int medicationRecocilationId);

        Task<ReconciliationToDoRelated> AddReconciliationMedicationToDoListRelated(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<ReconciliationToDoRelated> UpdateReconciliationToDoListRelated(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<NonRelatedRecocilationToDo> GetNonReconciliationToDoListRelatedById(int nonRelatedRecocilationToDoId);

        Task<NonRelatedRecocilationToDo> UpdateNonReconciliationToDoListRelated(NonRelatedRecocilationToDo nonRelatedRecocilationToDo);

        Task<ActionItemReconciliationToDo> GetActionitemReconciliationByPatientId(string description, int patientId);

        Task<ActionItemReconciliationToDo> AddActionItemReconciliationToDo(ActionItemReconciliationToDo actionItemReconciliationToDo);

        Task<NonRelatedRecocilationToDo> AddNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo);

        Task<List<MedicationReconciliation>> GetAllMedicationReconciliation(int patientId);

        Task<List<NonRelatedRecocilationToDo>> GetMedicationReconciliationNonRelatedRecocilationToDo(int patientId);

        Task<List<ActionItemReconciliationToDo>> GetAllActionItemReconciliationToDo(int patientId);

        Task DeleteReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<NonRelatedRecocilationToDo> GetNonRelatedRecocilationToDoByActionitemRecocilationToDoId(int actionitemRecocilationToDoId);

        Task DeleteNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo);

        Task<ReconciliationToDoRelated> GetReconciliationToDoListRelatedByMedicationReconciliationId(int medicationReconciliationId, int patientId);

        Task<NonRelatedRecocilationToDo> GetNonRelatedRecocilationToDoByActionitemReconciliationToDoId(int actionitemReconciliationToDoId, int patientId);

        Task<List<ReconciliationToDoRelated>> GetAllReconciliationToDoRelatedByPatientId(int patientId);

        Task<List<ReconciliationToDoRelated>> GetReconciliationToDoByPatientId(int patientId);

        void DeleteReconciliationToDoRelatedForServiceTakeAwayMedRec(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<List<NonRelatedRecocilationToDo>> GetNonReconciliationToDoByPatientId(int patientId);

        void DeleteNonRecocilationToDoForServiceTakeAwayMedRec(NonRelatedRecocilationToDo nonRelatedRecocilationToDo);

        Task<List<ReconciliationToDoRelated>> GetReconciliationToDoRelatedByPatientId(int patientId);

        void PatientDeleteForReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated);

        Task<List<NonRelatedRecocilationToDo>> GetNonRelatedRecocilationToDoRelatedByPatientId(int patientId);

        void PatientDeleteForNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo);
    }
}
