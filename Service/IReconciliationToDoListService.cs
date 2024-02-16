using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace OntrackDb.Service
{
    public interface IReconciliationToDoListService
    {

        Task<Response<ReconciliationToDoRelated>> AddReconciliationToDoList(ReconciliationToDoListModel model);

        Task<Response<ReconciliationToDoRelated>> UpdateReconciliationToDoList(ReconciliationToDoListModel model);
        Task<Response<NonRelatedRecocilationToDo>> UpdateReconciliationNonMedicationToDoList(NonRelatedRecocilationToDoModel model);
        Task<Response<NonRelatedRecocilationToDo>> AddPatientRecNonMedicationToDoList(NonRelatedRecocilationToDoModel model);

        Task<Response<MedicationReconciliation>> GetAllMedicationReconciliation(int patientId);

        Task<Response<NonRelatedRecocilationToDo>> GetMedRecNonRelatedRecocilationToDo(int patientId);

        Task<Response<ActionItemReconciliationToDo>> GetAllActionItemReconciliationToDo(int patientId);

        Task<Response<ReconciliationToDoRelated>> DeleteReconciliationToDoRelated(int medicationReconciliationId);

        Task<Response<NonRelatedRecocilationToDo>> DeleteActionitemReconciliationToDoId(int ActionitemReconciliationToDoId);

        Task<Response<ReconciliationToDoRelated>> GetRecToDoListRelatedByMedReconciliationId(int medicationReconciliationId, int patientId);

        Task<Response<NonRelatedRecocilationToDo>> GetNonRelatedRecToDoByActionitemRecToDoId(int actionitemReconciliationToDoId, int patientId);

        Task<Response<ReconciliationToDoRelated>> GetAllReconciliationToDoRelatedByPatientId(int patientId);
    }
}
