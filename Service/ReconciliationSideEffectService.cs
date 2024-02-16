using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ReconciliationSideEffectService : IReconciliationSideEffectService
    {

        private readonly IMedicationSubstanceData _medicationSubstanceData;
        private readonly IReactionData _reactionData;
        private readonly IPatientData _patientData;
        private readonly IReconciliationSideEffectData _reconciliationSideEffectData;

        public ReconciliationSideEffectService(IMedicationSubstanceData medicationSubstanceData, IReactionData reactionData, IPatientData patientData, IReconciliationSideEffectData reconciliationSideEffectData)
        {
            _medicationSubstanceData = medicationSubstanceData;
            _reactionData = reactionData;
            _patientData = patientData;
            _reconciliationSideEffectData = reconciliationSideEffectData; 
        }


        public async Task<Response<ReconciliationSideeffect>> AddPatientReconciliationSideEffect(ReconciliationSideEffectModel model)
        {
            Response<ReconciliationSideeffect> response = new Response<ReconciliationSideeffect>();
            var reconciliationSideeffectdb = await _reconciliationSideEffectData.GetRecSideeffectMedSubstanceId(model.MedicationSubstance, model.PatientId);
            if (reconciliationSideeffectdb.Count > 0)
            {
                response.Message = "reconciliationSideeffect Already exist";
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
                    ReconciliationSideeffect reconciliationSideeffect = new ReconciliationSideeffect();
                    response.Success = false;

                    if (medicationSubstance == null)
                    {
                        MedicationSubstance medicationSubstance1 = new MedicationSubstance()
                        {
                            Name = model.MedicationSubstance
                        };

                        var medicationSubstanceResult = await _medicationSubstanceData.AddMedicationSubstance(medicationSubstance1);
                        reconciliationSideeffect.MedicationSubstance = medicationSubstanceResult;

                        medicationSubstance = medicationSubstanceResult; // ON-469
                    }
                    else
                    {
                        reconciliationSideeffect.MedicationSubstance = medicationSubstance;
                    }
                    var reaction = await _reactionData.GetReactionByName(model.Reactions[i]);
                    if (reaction == null)
                    {
                        Reaction reaction1 = new Reaction()
                        {
                            Name = model.Reactions[i]
                        };
                        var reactionresult = await _reactionData.AddReaction(reaction1, false);
                        reconciliationSideeffect.Reaction = reactionresult;

                    }
                    else
                    {
                        reconciliationSideeffect.Reaction = reaction;
                    }

                    reconciliationSideeffect.Patient = patient;
                    var result = await _reconciliationSideEffectData.AddPatientReconciliationSideEffect(reconciliationSideeffect, false);
                }
                await _reconciliationSideEffectData.SaveChangesAsync();
                response.Success = true;
                response.Message = "Added Patient reconciliationSideeffect Successfully!";
                return response;
            }

        }


        public async Task<Response<ReconciliationSideeffect>> UpdatePatientReconciliationSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            Response<ReconciliationSideeffect> response = new Response<ReconciliationSideeffect>();

            var reconciliationSideeffects = await _reconciliationSideEffectData.GetRecSideEffectByPatientIdAndMedSubstanceId(patientid, medicationsubstanceid);

            foreach (var reconciliationSideeffect in reconciliationSideeffects)
            {
                await _reconciliationSideEffectData.DeleteReconciliationSideEffect(reconciliationSideeffect);
            }

            foreach (var reaction in reactions)
            {
                var patient = await _patientData.GetPatientById(patientid);
                var medicationSubstance = await _medicationSubstanceData.GetMedicationSubstanceById(medicationsubstanceid);

                ReconciliationSideeffect reconciliationSideeffect = new ReconciliationSideeffect();

                var reactionDb = await _reactionData.GetReactionByName(reaction.Name);
                if (reactionDb == null)
                {
                    Reaction reaction1 = new Reaction()
                    {
                        Name = reaction.Name,
                    };

                    var reactionresult = await _reactionData.AddReaction(reaction1, false);
                    reconciliationSideeffect.Reaction = reactionresult;


                }
                else
                {
                    reconciliationSideeffect.Reaction = reaction;
                }

                reconciliationSideeffect.Patient = patient;
                reconciliationSideeffect.MedicationSubstance = medicationSubstance;
                var result = await _reconciliationSideEffectData.AddPatientReconciliationSideEffect(reconciliationSideeffect, false);
            }
            await _reconciliationSideEffectData.SaveChangesAsync();
            response.Success = true;
            response.Message = "SideEffect Update successfully";
            return response;
        }

        public async Task<Response<Reaction>> GetAllReconciliationSideEffectReactions(int id)
        {

            Response<Reaction> response = new Response<Reaction>();
            var reactions = await _reconciliationSideEffectData.GetAllRecSideEffectsReactionsByPatientId(id);

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

        public async Task<Response<MedicationSubstance>> GetRecMedsubstancesByPatientIdForSideEffect(int id)
        {

            Response<MedicationSubstance> response = new Response<MedicationSubstance>();
            var medicationSubstances = await _reconciliationSideEffectData.GetMedSubstancesByPatientIdForSideEffect(id);

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

        public async Task<Response<ReconciliationSideeffect>> GetAllRecSideEffectReactionsByMedSubstanceById(int id, int patientId)
        {

            Response<ReconciliationSideeffect> response = new Response<ReconciliationSideeffect>();
            var reconciliationSideeffects = await _reconciliationSideEffectData.GetAllSideEffectReactionsByMedSubstanceById(id, patientId);

            if (reconciliationSideeffects == null)
            {
                response.Success = false;
                response.Message = "SideEffects Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "SideEffects retrived successfully";
            response.DataList = reconciliationSideeffects;
            return response;
        }

        public async Task<Response<ReconciliationSideeffect>> DeleteRecSideEffectByRecSideEffectId(int medicationSubstanceId, int patientId)
        {
            Response<ReconciliationSideeffect> response = new Response<ReconciliationSideeffect>();
            var reconciliationSideeffectDbs = await _reconciliationSideEffectData.GetRecSideEffectByMedSubstanceAndPatientId(medicationSubstanceId, patientId);
            if (reconciliationSideeffectDbs == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            foreach (ReconciliationSideeffect reconciliationSideeffect in reconciliationSideeffectDbs)
            {
                await _reconciliationSideEffectData.DeleteReconciliationSideEffect(reconciliationSideeffect);
            }

            response.Message = "ReconciliationSideeffectDbs Deleted Succesfully ";
            response.Success = true;
            return response;
        }

    }


}
