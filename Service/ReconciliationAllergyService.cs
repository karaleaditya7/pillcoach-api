using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OntrackDb.Service
{
    public class ReconciliationAllergyService : IReconciliationAllergyService
    {
        private readonly IMedicationSubstanceData _medicationSubstanceData;
        private readonly IReactionData _reactionData;
        private readonly IPatientData _patientData;
        private readonly IReconciliationAllergyData _reconciliationAllergyData;
        

        public ReconciliationAllergyService(IMedicationSubstanceData medicationSubstanceData, IReactionData reactionData, IPatientData patientData , IReconciliationAllergyData reconciliationAllergyData)
        {
            _medicationSubstanceData = medicationSubstanceData;
            _reactionData = reactionData;          
            _patientData = patientData;
            _reconciliationAllergyData = reconciliationAllergyData;
        }

        public async Task<Response<ReconciliationAllergy>> AddPatientReconciliationAllergy(ReconciliationAllergyModel model)
        {

            Response<ReconciliationAllergy> response = new Response<ReconciliationAllergy>();
            var reconciliationAllergydb = await _reconciliationAllergyData.GetRecAllergyMedicationSubstanceId(model.MedicationSubstance, model.PatientId);
            if (reconciliationAllergydb.Count > 0)
            {
                response.Message = "ReconciliationAllergydb already exist";
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
                foreach (var reaction in model.Reactions)
                {
                    ReconciliationAllergy reconciliation = new ReconciliationAllergy();
                    response.Success = false;

                    if (medicationSubstance == null)
                    {
                        MedicationSubstance medicationSubstance1 = new MedicationSubstance()
                        {
                            Name = model.MedicationSubstance
                        };

                        var medicationSubstanceResult = await _medicationSubstanceData.AddMedicationSubstance(medicationSubstance1);
                        reconciliation.MedicationSubstance = medicationSubstanceResult;

                        medicationSubstance = medicationSubstanceResult; // ON-468
                    }
                    else
                    {
                        reconciliation.MedicationSubstance = medicationSubstance;
                    }

                    var reactiondb = await _reactionData.GetReactionByName(reaction);
                    if (reactiondb == null)
                    {
                        Reaction reaction1 = new Reaction()
                        {
                            Name = reaction
                        };

                        var reactionresult = await _reactionData.AddReaction(reaction1, false);
                        reconciliation.Reaction = reactionresult;


                    }
                    else
                    {
                        reconciliation.Reaction = reactiondb;
                    }

                    reconciliation.Patient = patient;
                    var result = await _reconciliationAllergyData.AddPatientReconciliationAllergy(reconciliation, false);
                }
                await _reconciliationAllergyData.SaveChangesAsync();
                response.Success = true;
                response.Message = "Added ReconciliationAllergy Successfully!";
                //response.Data = result;
                return response;
            }

        }

        public async Task<Response<ReconciliationAllergy>> UpdatePatientReconciliationAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            Response<ReconciliationAllergy> response = new Response<ReconciliationAllergy>();
            var reconciliationAllergies = await _reconciliationAllergyData.GetReconciliationAllergyByAllergyId(patientid, medicationsubstanceid);

            foreach (var reconciliationAllergy in reconciliationAllergies)
            {
                await _reconciliationAllergyData.DeleteReconciliationAllergyById(reconciliationAllergy);
            }

            foreach (var reaction in reactions)
            {
                var patient = await _patientData.GetPatientById(patientid);
                var medicationSubstance = await _medicationSubstanceData.GetMedicationSubstanceById(medicationsubstanceid);

                ReconciliationAllergy reconciliationAllergy = new ReconciliationAllergy();

                var reactionDb = await _reactionData.GetReactionByName(reaction.Name);
                if (reactionDb == null)
                {
                    Reaction reaction1 = new Reaction()
                    {
                        Name = reaction.Name,
                    };

                    var reactionresult = await _reactionData.AddReaction(reaction1, false);
                    reconciliationAllergy.Reaction = reactionresult;


                }
                else
                {
                    reconciliationAllergy.Reaction = reaction;
                }

                reconciliationAllergy.Patient = patient;
                reconciliationAllergy.MedicationSubstance = medicationSubstance;
                var result = await _reconciliationAllergyData.AddPatientReconciliationAllergy(reconciliationAllergy, false);
            }
            await _reconciliationAllergyData.SaveChangesAsync();
            response.Success = true;
            response.Message = "Allergy Update successfully";
            return response;
        }

        public async Task<Response<Reaction>> GetAllReconciliationAllergyReactions(int id)
        {

            Response<Reaction> response = new Response<Reaction>();
            var reactions = await _reconciliationAllergyData.GetAllRecAllergyReactionsByPatientId(id);

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

        public async Task<Response<MedicationSubstance>> GetAllRecAllergyMedSubstancesBypatientId(int id)
        {

            Response<MedicationSubstance> response = new Response<MedicationSubstance>();
            var medicationSubstances = await _reconciliationAllergyData.GetMedSubstancesByPatientIdForRecAllergy(id);

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

        public async Task<Response<MedicationSubstance>> GetAllRecAllergyMedSubstanceAndRecAllergyReactionsBypatientId(int id)
        {

            Response<MedicationSubstance> response = new Response<MedicationSubstance>();
            var medicationSubstances = await _reconciliationAllergyData.GetRecAllergyMedSubstancesByPatientIdForAllergy(id);

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

        public async Task<Response<ReconciliationAllergy>> GetRecAllergyReactionsByMedSubstanceById(int id, int patientId)
        {

            Response<ReconciliationAllergy> response = new Response<ReconciliationAllergy>();
            var reconciliationAllergies = await _reconciliationAllergyData.GetAllRecAllergyReactionsByMedSubstanceById(id, patientId);

            if (reconciliationAllergies == null)
            {
                response.Success = false;
                response.Message = "Allergy Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "Allergy retrived successfully";
            response.DataList = reconciliationAllergies;
            return response;
        }


        public async Task<Response<ReconciliationAllergy>> GetAllRecAllergyReactionByMedSubstancesId(int id, int patientId)
        {

            Response<ReconciliationAllergy> response = new Response<ReconciliationAllergy>();
            var reconciliationAllergies = await _reconciliationAllergyData.GetAllRecAllergyReactionsByMedSubstanceById(id, patientId);

            if (reconciliationAllergies == null)
            {
                response.Success = false;
                response.Message = "Allergy Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "Allergy retrived successfully";
            response.DataList = reconciliationAllergies;
            return response;
        }

        public async Task<Response<ReconciliationAllergy>> DeleteReconciliationAllergyMedicationSubstanceId(int medicationSubstanceId, int patientId)
        {
            Response<ReconciliationAllergy> response = new Response<ReconciliationAllergy>();
            var reconciliationAllergiedb = await _reconciliationAllergyData.GetReconciliationAllergyById(medicationSubstanceId, patientId);

            if (reconciliationAllergiedb == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            foreach (ReconciliationAllergy reconciliationAllergy in reconciliationAllergiedb)
            {
                await _reconciliationAllergyData.DeleteReconciliationAllergyById(reconciliationAllergy);
            }

            response.Message = "ReconciliationAllergy Deleted Succesfully ";
            response.Success = true;
            return response;
        }
    }
}
