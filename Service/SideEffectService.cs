using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class SideEffectService : ISideEffectService
    {
        private readonly IMedicationSubstanceData _medicationSubstanceData;
        private readonly IReactionData _reactionData;
        private readonly IPatientData _patientData;
        private readonly ISideEffectData _sideEffectData;
        public SideEffectService(IMedicationSubstanceData medicationSubstanceData, IReactionData reactionData, IPatientData patientData, ISideEffectData sideEffectData)
        {
            _medicationSubstanceData = medicationSubstanceData;
            _reactionData = reactionData;   
            _patientData = patientData; 
            _sideEffectData = sideEffectData;   
        }
        public async Task<Response<SideEffect>> AddPatientSideEffects(SideEffectModel model)
        {
            Response<SideEffect> response = new Response<SideEffect>();
            var sideeffectdb = await _sideEffectData.GetSideEffectMedicationSubstanceId(model.MedicationSubstance, model.PatientId);
            if (sideeffectdb.Count > 0)
            {
                response.Message = "sideeffect Already exist";
                return response;
            }
            else
            {
                if (string.IsNullOrEmpty(model.MedicationSubstance))
                {
                    response.Message = "Medication/Substance is Missing";
                    return response;
                }
                if (model.Reactions == null)
                {
                    response.Message = "Reactions are Missing";
                    return response;
                }
                var patient = await _patientData.GetPatientById(model.PatientId);
                var medicationSubstance = await _medicationSubstanceData.GetMedicationSubstanceByName(model.MedicationSubstance);
                for (int i = 0; i < model.Reactions.Count; i++)
                {
                    SideEffect sideEffect = new SideEffect();
                    response.Success = false;
   
                    if (medicationSubstance == null)
                    {
                        MedicationSubstance medicationSubstance1 = new MedicationSubstance()
                        {
                            Name = model.MedicationSubstance
                        };

                        var medicationSubstanceResult = await _medicationSubstanceData.AddMedicationSubstance(medicationSubstance1);
                        sideEffect.MedicationSubstance = medicationSubstanceResult;

                        medicationSubstance = medicationSubstanceResult; // ON-469
                    }
                    else
                    {
                        sideEffect.MedicationSubstance = medicationSubstance;
                    }
                    var reaction = await _reactionData.GetReactionByName(model.Reactions[i]);
                    if (reaction == null)
                    {
                        Reaction reaction1 = new Reaction()
                        {
                            Name = model.Reactions[i]
                        };
                        var reactionresult = await _reactionData.AddReaction(reaction1, false);
                        sideEffect.Reaction = reactionresult;

                    }
                    else
                    {
                        sideEffect.Reaction = reaction;
                    }

                    sideEffect.Patient = patient;
                    var result = await _sideEffectData.AddPatientSideEffect(sideEffect,false);
                }
                await _sideEffectData.SaveChangesAsync();
                response.Success = true;
                response.Message = "Added Patient Side Effects Successfully!";
                return response;
            }
           
        }

        public async Task<Response<SideEffect>> UpdatePatientSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            Response<SideEffect> response = new Response<SideEffect>();

            var sideEffects = await _sideEffectData.GetSideEffectByPatientIdAndMedicationSubstanceId(patientid, medicationsubstanceid);

            foreach (var sideEffect in sideEffects)
            {
                await _sideEffectData.DeleteSideEffect(sideEffect);
            }

            foreach (var reaction in reactions)
            {
                var patient = await _patientData.GetPatientById(patientid);
                var medicationSubstance = await _medicationSubstanceData.GetMedicationSubstanceById(medicationsubstanceid);

                SideEffect sideEffect = new SideEffect();

                var reactionDb = await _reactionData.GetReactionByName(reaction.Name);
                if (reactionDb == null)
                {
                    Reaction reaction1 = new Reaction()
                    {
                        Name = reaction.Name,
                    };

                    var reactionresult = await _reactionData.AddReaction(reaction1, false);
                    sideEffect.Reaction = reactionresult;


                }
                else
                {
                    sideEffect.Reaction = reaction;
                }

                sideEffect.Patient = patient;
                sideEffect.MedicationSubstance = medicationSubstance;
                var result = await _sideEffectData.AddPatientSideEffect(sideEffect, false);
            }
            await _sideEffectData.SaveChangesAsync();
            response.Success = true;
            response.Message = "SideEffect Update successfully";
            return response;
        }
        public async Task<Response<Reaction>> GetSideEffectReactionsByPatientId(int id)
        {

            Response<Reaction> response = new Response<Reaction>();
            var reactions = await _sideEffectData.GetAllSideEffectsReactionsByPatientId(id);

            if (reactions == null)
            {
                response.Success = false;
                response.Message = "Reactions Not Found";
                return response;
            }


            response.Success = true;
            response.Message = "Reactions retrived successfully";
            response.DataList = reactions;
            return response;
        }


        public async Task<Response<MedicationSubstance>> GetMedicationsubstancesByPatientIdForSideEffect(int id)
        {

            Response<MedicationSubstance> response = new Response<MedicationSubstance>();
            var medicationSubstances = await _sideEffectData.GetMedicationSubstancesByPatientIdForSideEffect(id);

            if (medicationSubstances == null)
            {
                response.Success = false;
                response.Message = "medicationSubstances Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "medicationSubstances retrived successfully";
            response.DataList = medicationSubstances;
            return response;
        }

        public async Task<Response<SideEffect>> GetAllSideEffectReactionsByMedicationSubstanceById(int id ,int patientId)
        {

            Response<SideEffect> response = new Response<SideEffect>();
            var sideEffects = await _sideEffectData.GetAllSideEffectReactionsByMedicationSubstanceById(id,patientId);

            if (sideEffects == null)
            {
                response.Success = false;
                response.Message = "SideEffects Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "SideEffects retrived successfully";
            response.DataList = sideEffects;
            return response;
        }
        public async Task<Response<SideEffect>> DeleteSideEffectBySideEffectId(int medicationSubstanceId,int patientId)
        {
            Response<SideEffect> response = new Response<SideEffect>();
            var sideeffectDbs = await _sideEffectData.GetSideEffectByMedicationSubstanceAndPatientId(medicationSubstanceId,patientId);
            if (sideeffectDbs == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
           foreach(SideEffect sideEffect in sideeffectDbs)
            {
                 _sideEffectData.DeleteSideEffectBySideEffectForServiceTakeAway(sideEffect);
            }
            
            response.Message = "SideEffect Deleted Succesfully ";
            response.Success = true;
            return response;
        }

        public async Task<Response<SideEffect>> DeleteSideEffectByMedicationSubstanceIdAndPatientId(int medicationSubstanceId, int patientId)
        {
            Response<SideEffect> response = new Response<SideEffect>();
            var sideeffectDbs = await _sideEffectData.GetSideEffectByMedicationSubstanceAndPatientId(medicationSubstanceId, patientId);
            if (sideeffectDbs == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            foreach (SideEffect sideEffect in sideeffectDbs)
            {
                await _sideEffectData.DeleteSideEffect(sideEffect);
            }

            response.Message = "SideEffect Deleted Succesfully ";
            response.Success = true;
            return response;
        }

    }
}
