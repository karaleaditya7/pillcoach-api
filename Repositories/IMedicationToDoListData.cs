using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IMedicationToDoListData
    {
        Task<MedicationToDoRelated> AddMedicationToDoListRelated(MedicationToDoRelated medicationToDoRelated);
        Task<MedicationToDoRelated> UpdateMedicationToDoListRelated(MedicationToDoRelated medicationToDoRelated);
        Task<NonRelatedMedicationToDo> UpdateNonMedicationToDoListRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo);
        Task<ActionItemToDo> AddActionItemToDo(ActionItemToDo actionItemToDo);

        Task<ActionItemToDo> GetActionitem(string description);

        Task<NonRelatedMedicationToDo> AddNonMedicationRelatedToDo(NonRelatedMedicationToDo nonRelatedMedicationToDo);

        Task<List<CmrMedication>> getAllCmrMedicationRelated(int patientId);

        Task<List<ActionItemToDo>> getAllActionItemToDo(int patientId);

        Task DeleteMedicationToDoRelated(MedicationToDoRelated medicationToDoRelated);

        Task<MedicationToDoRelated> getMedicationToDoListRelatedByCmrMedicationId(int cmrMedicationId);
        Task<MedicationToDoRelated> getMedicationToDoListRelatedById(int medicationToDoRelatedId);

        Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedById(int medicationToDoListNonRelatedId);

        Task DeleteNonMedicationToDoListRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo);

        Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedByActionitemToDoId(int actionitemToDoId);

        Task<MedicationToDoRelated> getMedicationToDoListRelatedByCmrMedicationIdByPatientId(int cmrMedicationId, int patientId);

        Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedByActionitemToDoIdPatientId(int actionitemToDoId, int patientId);
        Task<ActionItemToDo> GetActionitemByPatientId(string description, int patientId);

        Task<List<NonRelatedMedicationToDo>> getCmrNonMedicationRelated(int patientId);
        Task<List<MedicationToDoRelated>> getMedicationToDoRelatedsbyPatientId(int patientId);
        void PatientDeleteMedicationToDoRelated(MedicationToDoRelated medicationToDoRelated);
        Task<List<NonRelatedMedicationToDo>> getNonMedicationToDoRelatedsbyPatientId(int patientId);
        void PatientDeleteNonMedicationToDoRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo);

        Task<List<MedicationToDoRelated>> GetMedicationByPatientId(int patientId);

        Task<List<NonRelatedMedicationToDo>> GetNonMedicationByPatientId(int patientId);

        void DeleteNonMedicationToDoListRelatedForServiceTakeAwayInformaction(NonRelatedMedicationToDo nonRelatedMedicationToDo);

        void DeleteMedicationToDoRelatedForServiceTakeAwayInformaction(MedicationToDoRelated medicationToDoRelated);
    }
}
