using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IMedicationToDoListService
    {
        Task<Response<MedicationToDoRelated>> AddMedicationToDoList(MedicationToDoListModel model);
        Task<Response<MedicationToDoRelated>> UpdateMedicationToDoList(MedicationToDoListModel model);
        Task<Response<NonRelatedMedicationToDo>> UpdateNonMedicationToDoList(MedicationToDoListNonRelatedModel model);
        Task<Response<NonRelatedMedicationToDo>> AddMedicationToDoListNonRelated(MedicationToDoListNonRelatedModel model);

        Task<Response<CmrMedication>> getAllCmrMedicationRelated(int patientId);

        Task<Response<ActionItemToDo>> getAllActionItemToDo(int patientId);

        Task<Response<MedicationToDoRelated>> DeleteMedicationToDoRelated(int cmrMedicationId);

        Task<Response<NonRelatedMedicationToDo>> DeleteActionitemToDoId(int ActionitemToDoId);

        Task<Response<MedicationToDoRelated>> getMedicationToDoListRelatedByCmrMedicationId(int cmrMedicationId );

        Task<Response<NonRelatedMedicationToDo>> getNonMedicationToDoListRelatedByActionitemToDoId(int ActionitemToDoId);

        Task<Response<NonRelatedMedicationToDo>> getNonMedicationToDoListRelatedByCmrMedicationIdByPatientId(int actionitemToDoId, int patientId);
        Task<Response<MedicationToDoRelated>> getMedicationToDoListRelatedByCmrMedicationIdByPatientId(int cmrMedicationId, int patientId);

        Task<Response<NonRelatedMedicationToDo>> getCmrNonMedicationRelated(int patientId);

        Task<Response<MedicationToDoRelated>> getAllMedicationToDoList(int patientId);
    }
}
