using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class MedicationToDoListService : IMedicationToDoListService
    {
        private readonly IPatientData _patientData;
        private readonly ICmrMedicationData _cmrMedicationData;
        private readonly IMedicationToDoListData _medicationToDoListData;
       
        public MedicationToDoListService(IPatientData patientData, ICmrMedicationData cmrMedicationData, IMedicationToDoListData medicationToDoListData)
        {
            _patientData = patientData;
            _cmrMedicationData = cmrMedicationData;
            _medicationToDoListData = medicationToDoListData;
        }

        public async Task<Response<MedicationToDoRelated>> AddMedicationToDoList(MedicationToDoListModel model)
        {
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var patient = await _patientData.GetPatientById(model.PatientId);
            var cmrMedication = await _cmrMedicationData.GetCmrMedicationsById(model.CmrMedicationId);

            var medicationToDoList = await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationId(model.CmrMedicationId);


            if(medicationToDoList == null && cmrMedication!= null)
            {
                MedicationToDoRelated medicationToDoRelated = new MedicationToDoRelated
                {
                    Patient = patient,
                    CmrMedication = cmrMedication,
                    PatientToDo = model.PatientToDo,
                };


                var result = await _medicationToDoListData.AddMedicationToDoListRelated(medicationToDoRelated);
               
                response.Success = true;
                response.Message = "Added MedicationToDoList Successfully!";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = false;
                response.Message = " MedicationToDoList already present for that cmrMedication";
                return response;
            }

           
        }

        public async Task<Response<MedicationToDoRelated>> UpdateMedicationToDoList(MedicationToDoListModel model)
        {
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var medicationToDoList = await _medicationToDoListData.getMedicationToDoListRelatedById(model.Id);


            if (medicationToDoList != null)
            {

                medicationToDoList.PatientToDo = model.PatientToDo;

                var result = await _medicationToDoListData.UpdateMedicationToDoListRelated(medicationToDoList);

                response.Success = true;
                response.Message = "Updated MedicationToDoList Successfully!";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = false;
                response.Message = " MedicationToDoList not found";
                return response;
            }


        }

        public async Task<Response<NonRelatedMedicationToDo>> UpdateNonMedicationToDoList(MedicationToDoListNonRelatedModel model)
        {
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var nonmedicationToDoList = await _medicationToDoListData.getNonMedicationToDoListRelatedById(model.Id);


            if (nonmedicationToDoList != null)
            {

                nonmedicationToDoList.PatientToDo = model.PatientToDo;

                var result = await _medicationToDoListData.UpdateNonMedicationToDoListRelated(nonmedicationToDoList);

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


        public async Task<Response<NonRelatedMedicationToDo>> AddMedicationToDoListNonRelated(MedicationToDoListNonRelatedModel model)
        {
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            NonRelatedMedicationToDo nonRelatedMedicationTo = new NonRelatedMedicationToDo();
            response.Success = false;
            if (string.IsNullOrEmpty(model.PatientToDo))
            {
                response.Message = "PatientToDo is Missing";
                return response;
            }

            var patient = await _patientData.GetPatientById(model.PatientId);

            var actionItemToDo = await _medicationToDoListData.GetActionitemByPatientId(model.ActionItem,model.PatientId);
            if (actionItemToDo == null)
            {
                ActionItemToDo actionItemTo = new ActionItemToDo
                {
                    Description = model.ActionItem
                };

                var actionItemResult = await _medicationToDoListData.AddActionItemToDo(actionItemTo);
                nonRelatedMedicationTo.ActionItemToDo = actionItemResult;
            }
            else
            {
                response.Success = false;
                response.Message = "ActionItem already present";
                return response;

            }

            nonRelatedMedicationTo.Patient = patient;
            nonRelatedMedicationTo.PatientToDo = model.PatientToDo;



            var result = await _medicationToDoListData.AddNonMedicationRelatedToDo(nonRelatedMedicationTo);

            response.Success = true;
            response.Message = "Added NonRelatedMedicationToDoList Successfully!";
            response.Data = result;
            return response;
        }

         public async Task<Response<CmrMedication>> getAllCmrMedicationRelated(int patientId)
         {
            Response<CmrMedication> response = new Response<CmrMedication>();
            var result = await _medicationToDoListData.getAllCmrMedicationRelated(patientId);
            if(result == null)
            {
                response.Success=false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "CmrMedications retrived successfully";
            response.DataList = result;
            return response;
         }

       public async Task<Response<NonRelatedMedicationToDo>> getCmrNonMedicationRelated(int patientId)
        {
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            var result = await _medicationToDoListData.getCmrNonMedicationRelated(patientId);
            if(result == null)
            {
                response.Success = false;
                response.Message = "Not Found";               
                return response;
            }
            response.Success = true;
            response.Message = "Non-Medication retrived successfully";
            response.DataList = result;
            return response;
           
        }

        public async Task<Response<ActionItemToDo>> getAllActionItemToDo(int patientId)
         {
            Response<ActionItemToDo> response = new Response<ActionItemToDo>();
            var result = await _medicationToDoListData.getAllActionItemToDo(patientId);

            if (result == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "ActionItemToDos retrived successfully";
            response.DataList = result;
            return response;
        }

        public async Task<Response<MedicationToDoRelated>> DeleteMedicationToDoRelated(int cmrMedicationId)
        {
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();
            var medicationToDoRelated = await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationId(cmrMedicationId);

            if (medicationToDoRelated == null)
            {
                response.Message = "cmrMedication Not Found";
                response.Success = false;              
                return response;
            }
            
          await _medicationToDoListData.DeleteMedicationToDoRelated(medicationToDoRelated);
            response.Success = true;
            response.Message = "MedicationToDoRelated list deleted successfully";
            return response;
        
        }

        public async Task<Response<NonRelatedMedicationToDo>> DeleteActionitemToDoId(int ActionitemToDoId)
        {
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            var nonRelatedMedicationToDo = await _medicationToDoListData.getNonMedicationToDoListRelatedByActionitemToDoId(ActionitemToDoId);
            if (nonRelatedMedicationToDo == null)
            {
                response.Message = "ActionitemToDoId Not Found";
                response.Success = false;            
                return response;
            }
           
            await _medicationToDoListData.DeleteNonMedicationToDoListRelated(nonRelatedMedicationToDo);
            response.Success = true;
            response.Message = "NonRelatedMedicationToDo list deleted successfully";
            
            return response;

        }

        public async Task<Response<MedicationToDoRelated>> getMedicationToDoListRelatedByCmrMedicationId(int cmrMedicationId)
        {
            // Response<CmrMedication> response = new Response<CmrMedication>();
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();
            var medicationToDoRelated = await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationId(cmrMedicationId);
            if (medicationToDoRelated == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "medicationToDoRelated retrived successfully";
            //response.DataList = result;
            response.Data = medicationToDoRelated;
            return response;
        }


        public async Task<Response<MedicationToDoRelated>> getMedicationToDoListRelatedByCmrMedicationIdByPatientId(int cmrMedicationId , int patientId)
        {
            // Response<CmrMedication> response = new Response<CmrMedication>();
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();

            var medicationToDoRelated = await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationIdByPatientId(cmrMedicationId , patientId);
            if (medicationToDoRelated == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "medicationToDoRelated retrived successfully";
            //response.DataList = result;
            response.Data = medicationToDoRelated;
            return response;
        }

        public async Task<Response<NonRelatedMedicationToDo>> getNonMedicationToDoListRelatedByActionitemToDoId(int actionitemToDoId)
        {
            //Response<ActionItemToDo> response = new Response<ActionItemToDo>();
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            var nonRelatedMedicationToDo = await _medicationToDoListData.getNonMedicationToDoListRelatedByActionitemToDoId(actionitemToDoId);
            if (nonRelatedMedicationToDo == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "nonRelatedMedicationToDo retrived successfully";
            response.Data = nonRelatedMedicationToDo;
            return response;
        }

        public async Task<Response<NonRelatedMedicationToDo>> getNonMedicationToDoListRelatedByCmrMedicationIdByPatientId(int actionitemToDoId, int patientId)
        {
            Response<NonRelatedMedicationToDo> response = new Response<NonRelatedMedicationToDo>();
            var nonRelatedMedicationToDo = await _medicationToDoListData.getNonMedicationToDoListRelatedByActionitemToDoIdPatientId(actionitemToDoId, patientId);
            if (nonRelatedMedicationToDo == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "nonRelatedMedicationToDo retrived successfully";
            response.Data = nonRelatedMedicationToDo;
            return response;
        }


        public async Task<Response<MedicationToDoRelated>> getAllMedicationToDoList( int patientId)
        {
            Response<MedicationToDoRelated> response = new Response<MedicationToDoRelated>();
            var medicationToDoRelateds = await _medicationToDoListData.getMedicationToDoRelatedsbyPatientId(patientId);
            if (medicationToDoRelateds == null)
            {
                response.Success = false;
                response.Message = "Not found";
                return response;
            }

            response.Success = true;
            response.Message = "medicationToDoRelateds retrived successfully";
            response.DataList = medicationToDoRelateds;
            return response;
        }

    }
}
