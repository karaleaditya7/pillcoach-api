using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ReconciliationToDoListService : IReconciliationToDoListService
    {
        private readonly IPatientData _patientData;
        private readonly IReconciliationData _cmrReconciliationData;
        private readonly IReconciliationToDoListData _reconciliationToDoListData;
        public ReconciliationToDoListService(IReconciliationToDoListData reconciliationToDoListData, IPatientData patientData, IReconciliationData cmrReconciliationData)
        {
            _reconciliationToDoListData = reconciliationToDoListData; 
            _patientData = patientData; 
            _cmrReconciliationData = cmrReconciliationData;     
        }
        public async Task<Response<ReconciliationToDoRelated>> AddReconciliationToDoList(ReconciliationToDoListModel model)
        {
            Response<ReconciliationToDoRelated> response = new Response<ReconciliationToDoRelated>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var patient = await _patientData.GetPatientById(model.PatientId);
            var medicationReconciliation = await _cmrReconciliationData.GetReconciliationMedicationsById(model.MedicationReconciliationId);

            var reconciliationToDoRelated = await _reconciliationToDoListData.GetReconciliationToDoListRelatedByCmrMedicationId(model.MedicationReconciliationId);


            if (reconciliationToDoRelated == null && medicationReconciliation != null)
            {
                ReconciliationToDoRelated reconciliationToDoRelated1 = new ReconciliationToDoRelated
                {
                    Patient = patient,
                    MedicationReconciliation = medicationReconciliation,
                    PatientToDo = model.PatientToDo,
                };


                var result = await _reconciliationToDoListData.AddReconciliationMedicationToDoListRelated(reconciliationToDoRelated1);

                response.Success = true;
                response.Message = "Added ReconciliationToDoRelated Successfully!";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = false;
                response.Message = " ReconciliationToDoRelated already present for that MedicationReconciliation";
                return response;
            }
        }


        public async Task<Response<ReconciliationToDoRelated>> UpdateReconciliationToDoList(ReconciliationToDoListModel model)
        {
            Response<ReconciliationToDoRelated> response = new Response<ReconciliationToDoRelated>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var reconciliationToDoRelated = await _reconciliationToDoListData.GetReconciliationToDoListRelatedByCmrMedicationId(model.MedicationReconciliationId);


            if (reconciliationToDoRelated != null)
            {

                reconciliationToDoRelated.PatientToDo = model.PatientToDo;

                var result = await _reconciliationToDoListData.UpdateReconciliationToDoListRelated(reconciliationToDoRelated);

                response.Success = true;
                response.Message = "Updated ReconciliationToDoRelated Successfully!";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = false;
                response.Message = " ReconciliationToDoRelated not found";
                return response;
            }


        }

        public async Task<Response<NonRelatedRecocilationToDo>> UpdateReconciliationNonMedicationToDoList(NonRelatedRecocilationToDoModel model)
        {
            Response<NonRelatedRecocilationToDo> response = new Response<NonRelatedRecocilationToDo>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var nonRelatedRecocilationToDo = await _reconciliationToDoListData.GetNonReconciliationToDoListRelatedById(model.Id);


            if (nonRelatedRecocilationToDo != null)
            {

                nonRelatedRecocilationToDo.PatientToDo = model.PatientToDo;

                var result = await _reconciliationToDoListData.UpdateNonReconciliationToDoListRelated(nonRelatedRecocilationToDo);

                response.Success = true;
                response.Message = "Updated NonMedicationToDoList Successfully!";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = false;
                response.Message = " NonMedicationToDoList not found";
                return response;
            }
        }

        public async Task<Response<NonRelatedRecocilationToDo>> AddPatientRecNonMedicationToDoList(NonRelatedRecocilationToDoModel model)
        {
            Response<NonRelatedRecocilationToDo> response = new Response<NonRelatedRecocilationToDo>();
            NonRelatedRecocilationToDo nonRelatedRecocilationToDo = new NonRelatedRecocilationToDo();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var patient = await _patientData.GetPatientById(model.PatientId);

            var actionItemReconciliationToDo = await _reconciliationToDoListData.GetActionitemReconciliationByPatientId(model.ActionItem, model.PatientId);
            if (actionItemReconciliationToDo == null)
            {
                ActionItemReconciliationToDo actionItemReconciliationToDo1 = new ActionItemReconciliationToDo
                {
                    Description = model.ActionItem
                };

                var actionItemResult = await _reconciliationToDoListData.AddActionItemReconciliationToDo(actionItemReconciliationToDo1);
                nonRelatedRecocilationToDo.ActionItemReconciliationToDo = actionItemResult;
            }
            else
            {
                response.Success = false;
                response.Message = "ActionItem already present";
                return response;

            }

            nonRelatedRecocilationToDo.Patient = patient;
            nonRelatedRecocilationToDo.PatientToDo = model.PatientToDo;



            var result = await _reconciliationToDoListData.AddNonRelatedRecocilationToDo(nonRelatedRecocilationToDo);

            response.Success = true;
            response.Message = "Added NonRelatedRecocilationToDo Successfully!";
            response.Data = result;
            return response;
        }

        public async Task<Response<MedicationReconciliation>> GetAllMedicationReconciliation(int patientId)
        {
            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            var result = await _reconciliationToDoListData.GetAllMedicationReconciliation(patientId);
            if (result == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "MedicationReconciliations retrived successfully";
            response.DataList = result;
            return response;
        }

        public async Task<Response<NonRelatedRecocilationToDo>> GetMedRecNonRelatedRecocilationToDo(int patientId)
        {
            Response<NonRelatedRecocilationToDo> response = new Response<NonRelatedRecocilationToDo>();
            var result = await _reconciliationToDoListData.GetMedicationReconciliationNonRelatedRecocilationToDo(patientId);
            if (result == null)
            {
                response.Success = false;
                response.Message = "Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "NonRelatedRecocilationToDo retrived successfully";
            response.DataList = result;
            return response;

        }

        public async Task<Response<ActionItemReconciliationToDo>> GetAllActionItemReconciliationToDo(int patientId)
        {
            Response<ActionItemReconciliationToDo> response = new Response<ActionItemReconciliationToDo>();
            var result = await _reconciliationToDoListData.GetAllActionItemReconciliationToDo(patientId);

            if (result == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "ActionItemReconciliationToDos retrived successfully";
            response.DataList = result;
            return response;
        }

        public async Task<Response<ReconciliationToDoRelated>> DeleteReconciliationToDoRelated(int medicationReconciliationId)
        {
            Response<ReconciliationToDoRelated> response = new Response<ReconciliationToDoRelated>();
            var reconciliationToDoRelated = await _reconciliationToDoListData.GetReconciliationToDoListRelatedByCmrMedicationId(medicationReconciliationId);

            if (reconciliationToDoRelated == null)
            {
                response.Message = "reconciliationToDoRelated Not Found";
                response.Success = false;
                return response;
            }

            await _reconciliationToDoListData.DeleteReconciliationToDoRelated(reconciliationToDoRelated);
            response.Success = true;
            response.Message = "ReconciliationToDoRelated deleted successfully";
            return response;

        }

        public async Task<Response<NonRelatedRecocilationToDo>> DeleteActionitemReconciliationToDoId(int ActionitemReconciliationToDoId)
        {
            Response<NonRelatedRecocilationToDo> response = new Response<NonRelatedRecocilationToDo>();
            var nonRelatedRecocilationToDo = await _reconciliationToDoListData.GetNonRelatedRecocilationToDoByActionitemRecocilationToDoId(ActionitemReconciliationToDoId);
            if (nonRelatedRecocilationToDo == null)
            {
                response.Message = "ActionitemRecocilationToDoId Not Found";
                response.Success = false;
                return response;
            }

            await _reconciliationToDoListData.DeleteNonRelatedRecocilationToDo(nonRelatedRecocilationToDo);
            response.Success = true;
            response.Message = "NonRelatedRecocilationToDo list deleted successfully";

            return response;

        }

        public async Task<Response<ReconciliationToDoRelated>> GetRecToDoListRelatedByMedReconciliationId(int medicationReconciliationId, int patientId)
        {
            // Response<CmrMedication> response = new Response<CmrMedication>();
            Response<ReconciliationToDoRelated> response = new Response<ReconciliationToDoRelated>();

            var reconciliationToDoRelated = await _reconciliationToDoListData.GetReconciliationToDoListRelatedByMedicationReconciliationId(medicationReconciliationId, patientId);
            if (reconciliationToDoRelated == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "ReconciliationToDoRelated retrived successfully";
            //response.DataList = result;
            response.Data = reconciliationToDoRelated;
            return response;
        }

        public async Task<Response<NonRelatedRecocilationToDo>> GetNonRelatedRecToDoByActionitemRecToDoId(int actionitemReconciliationToDoId, int patientId)
        {
            Response<NonRelatedRecocilationToDo> response = new Response<NonRelatedRecocilationToDo>();
            var nonRelatedRecocilationToDo = await _reconciliationToDoListData.GetNonRelatedRecocilationToDoByActionitemReconciliationToDoId(actionitemReconciliationToDoId, patientId);
            if (nonRelatedRecocilationToDo == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "NonRelatedRecocilationToDo retrived successfully";
            response.Data = nonRelatedRecocilationToDo;
            return response;
        }

        public async Task<Response<ReconciliationToDoRelated>> GetAllReconciliationToDoRelatedByPatientId(int patientId)
        {
            Response<ReconciliationToDoRelated> response = new Response<ReconciliationToDoRelated>();
            var reconciliationToDoRelateds = await _reconciliationToDoListData.GetAllReconciliationToDoRelatedByPatientId(patientId);
            if (reconciliationToDoRelateds == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "reconciliationToDoRelateds retrived successfully";
            response.DataList = reconciliationToDoRelateds;
            return response;
        }
    }
}
