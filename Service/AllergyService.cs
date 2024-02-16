using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class AllergyService :IAllergyService
    {
        private readonly IMedicationSubstanceData _medicationSubstanceData;
        private readonly IReactionData _reactionData;
        private readonly IPatientData _patientData;
        private readonly IAllergyData _allergyData;


        public AllergyService(IMedicationSubstanceData medicationSubstanceData, IReactionData reactionData, IAllergyData allergyData, IPatientData patientData)
        {
            _medicationSubstanceData = medicationSubstanceData;
            _reactionData = reactionData;
            _allergyData = allergyData;
            _patientData = patientData; 
        }
        public async Task<Response<Allergy>> AddPatientAllergy(AllergyModel model)
        {

            Response<Allergy> response = new Response<Allergy>();
            var allergydb = await _allergyData.GetAllergyMedicationSubstanceId(model.MedicationSubstance, model.PatientId);
            if (allergydb.Count > 0)
            {
                response.Message = "Allergy already exist";
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
                    Allergy allergy = new Allergy();
                    response.Success = false;

                    if (medicationSubstance == null)
                    {
                        MedicationSubstance medicationSubstance1 = new MedicationSubstance()
                        {
                            Name = model.MedicationSubstance
                        };

                        var medicationSubstanceResult = await _medicationSubstanceData.AddMedicationSubstance(medicationSubstance1);
                        allergy.MedicationSubstance = medicationSubstanceResult;

                        medicationSubstance = medicationSubstanceResult; // ON-468
                    }
                    else
                    {
                        allergy.MedicationSubstance = medicationSubstance;
                    }

                    var reactiondb = await _reactionData.GetReactionByName(reaction);
                    if (reactiondb == null)
                    {
                        Reaction reaction1 = new Reaction()
                        {
                            Name = reaction
                        };
                       
                        var reactionresult = await _reactionData.AddReaction(reaction1,false);
                        allergy.Reaction = reactionresult;


                    }
                    else
                    {
                        allergy.Reaction = reactiondb;
                    }

                    allergy.Patient = patient;
                    await _allergyData.AddPatientAllergy(allergy,false);
                }
                await _allergyData.SaveChangesAsync();
                response.Success = true;
                response.Message = "Added Allergy Successfully!";
                return response;
            }
           
        }

        public async Task<Response<Allergy>> UpdatePatientAllergy(int patientid, int medicationsubstanceid, List<Reaction> reactions)
        {
            Response<Allergy> response = new Response<Allergy>();
            var allergies = await _allergyData.GetAllergyByAllergyId(patientid, medicationsubstanceid);

            foreach (var allergy in allergies)
            {
                await _allergyData.DeleteAllergyById(allergy);
            }

            foreach (var reaction in reactions)
            {
                var patient = await _patientData.GetPatientById(patientid);
                var medicationSubstance = await _medicationSubstanceData.GetMedicationSubstanceById(medicationsubstanceid);

                Allergy allergy = new Allergy();

                var reactionDb = await _reactionData.GetReactionByName(reaction.Name);
                if (reactionDb == null)
                {
                    Reaction reaction1 = new Reaction()
                    {
                        Name = reaction.Name,
                    };

                    var reactionresult = await _reactionData.AddReaction(reaction1,false);
                    allergy.Reaction = reactionresult;


                }
                else
                {
                    allergy.Reaction = reaction;
                }

                allergy.Patient = patient;
                allergy.MedicationSubstance = medicationSubstance;
                await _allergyData.AddPatientAllergy(allergy, false);
            }
            await _allergyData.SaveChangesAsync();
            response.Success = true;
            response.Message = "Allergy Update successfully";
            return response;
        }
        public async Task<Response<Reaction>> GetReactionsByPatientId(int id)
        {
          
            Response<Reaction> response = new Response<Reaction>();
            var reactions = await _allergyData.GetAllReactionsByPatientId(id);

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


        public async Task<Response<MedicationSubstance>> GetMedicationsubstancesByPatientIdForAllergy(int id)
        {

            Response<MedicationSubstance> response = new Response<MedicationSubstance>();
            var medicationSubstances = await _allergyData.GetMedicationSubstancesByPatientIdForAllergy(id);

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

        public async Task<Response<Allergy>> GetAllergyReactionsByMedicationSubstanceById(int id ,int patientId)
        {

            Response<Allergy> response = new Response<Allergy>();
            var allergies = await _allergyData.GetAllAllergyReactionsByMedicationSubstanceById(id,patientId);

            if (allergies == null)
            {
                response.Success = false;
                response.Message = "Allergy Not Found";
                return response;
            }
            response.Success = true;
            response.Message = "Allergy retrived successfully";
            response.DataList = allergies;
            return response;
        }

        public async Task<Response<Allergy>> DeleteMedicationSubstanceAndReactionsId(int medicationSubstanceId,int patientId)
        {
            Response<Allergy> response = new Response<Allergy>();
            var allergyDbs = await _allergyData.GetAllergyById(medicationSubstanceId, patientId);

            if (allergyDbs == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            foreach(Allergy allegy in allergyDbs)
            {
                 _allergyData.DeleteAllergyByServiceTakeAway(allegy);
            }
           
            response.Message = "Allergy Deleted Succesfully ";
            response.Success = true;
            return response;
        }

        public async Task<Response<Allergy>> DeleteMedicationSubstanceAndReactionsIdForAllergies(int medicationSubstanceId, int patientId)
        {
            Response<Allergy> response = new Response<Allergy>();
            var allergyDbs = await _allergyData.GetAllergyById(medicationSubstanceId, patientId);

            if (allergyDbs == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            foreach (Allergy allegy in allergyDbs)
            {
                await _allergyData.DeleteAllergyById(allegy);
            }

            response.Message = "Allergy Deleted Succesfully ";
            response.Success = true;
            return response;
        }

    }
}
