using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class AllergyData :IAllergyData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public AllergyData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;   
        }
        public async Task<Allergy> AddPatientAllergy(Allergy allergy,bool commitChanges)
        {
            var result = await _applicationDbcontext.Allergies.AddAsync(allergy);
            if(commitChanges) await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbcontext.SaveChangesAsync();
        }
        public async Task<Allergy> GetAllergyByAllergyId(int allergyid)
        {
            var allergy = await _applicationDbcontext.Allergies.Where(a => a.Id == allergyid).FirstOrDefaultAsync();

            return allergy;
        }
        public async Task<List<Allergy>> GetAllergyByAllergyId(int patientid, int medicationsubstanceid)
        {
            var allergies = await _applicationDbcontext.Allergies.Where(a => a.Patient.Id == patientid && a.MedicationSubstance.Id == medicationsubstanceid)
                .ToListAsync();
            return allergies;
        }
        public async Task<List<int>> GetReactionyByAllergyId(int allergyid)
        {
          var reactions = await _applicationDbcontext.Allergies.Where(a => a.Id == allergyid).
                Select(a => a.Reaction.Id).ToListAsync();
            return reactions;
        }

       

        public async Task<Reaction> GetReactionByReactionId(int reactionId)
        {
            var reaction = await _applicationDbcontext.Reactions.Where(a => a.Id == reactionId).FirstOrDefaultAsync();

            return reaction;
        }

        public async Task<Allergy> UpdateReactionByAllergy(Allergy allergy)
        {
            var result = _applicationDbcontext.Allergies.Update(allergy);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<Reaction>> GetAllReactionsByPatientId(int id)
        {
            var reactions = await _applicationDbcontext.Allergies.Where(m => m.Patient.Id == id).
                                             Select(a=>a.Reaction).
                                            ToListAsync();
            return reactions;
        }

        public async Task<List<MedicationSubstance>>GetMedicationSubstancesByPatientIdForAllergy (int id)
        {
            var MedicationSubstances = await _applicationDbcontext.Allergies.Where(m => m.Patient.Id == id).Select(m=>m.MedicationSubstance).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return MedicationSubstances;
        }

        public async Task<List<Allergy>> GetAllAllergyReactionsByMedicationSubstanceById(int id , int patientId)
        {
            var Reactions = await _applicationDbcontext.Allergies.Where(m => m.MedicationSubstance.Id == id && m.Patient.Id == patientId).Include(m => m.Reaction).
                                            ToListAsync();
            return Reactions;
        }

        public async Task<List<Allergy>> GetAllergyById(int medicationSubstanceId, int patientId)
        {
            return await _applicationDbcontext.Allergies.Where(a => a.MedicationSubstance.Id == medicationSubstanceId && a.Patient.Id == patientId).ToListAsync();
        }

        public async Task<List<Allergy>> GetAllergyByPatientId( int patientId)
        {
            return await _applicationDbcontext.Allergies.Include(a => a.MedicationSubstance).Where( a => a.Patient.Id == patientId).ToListAsync();
        }

        public async Task<MedicationSubstance> GetAllergyMedicationSubstanceByAllergyId(int medicationSubstanceId, int patientId)
        {
            var medicationSubstance= await _applicationDbcontext.Allergies.Where(a => a.MedicationSubstance.Id == medicationSubstanceId && a.Patient.Id == patientId).Select(a=>a.MedicationSubstance).FirstOrDefaultAsync();
            return medicationSubstance;
        }

        public async Task DeleteAllergyById(Allergy allergy)
        {
             _applicationDbcontext.Allergies.Remove(allergy);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public void  DeleteAllergyByServiceTakeAway(Allergy allergy)
        {
            _applicationDbcontext.Allergies.Remove(allergy);
          
        }


        public void PatientDeleteForAllergy(Allergy allergy)
        {
            _applicationDbcontext.Allergies.Remove(allergy);
            

        }

        public void DeleteReactionById(Reaction reaction)
        {
            _applicationDbcontext.Reactions.Remove(reaction);

        }

        public void DeleteMedicationSubstanceById(MedicationSubstance medicationSubstance)
        {
            _applicationDbcontext.MedicationSubstances.Remove(medicationSubstance);

        }
    

        public async Task<List<Reaction>> GetAllAllergyReactionsByMedicationSubstanceByAndPatientById(int medicationSubstanceId,int patientId)
        {
            var Reactions = await _applicationDbcontext.Allergies.Where(m => m.MedicationSubstance.Id == medicationSubstanceId && m.Patient.Id ==patientId).Select(m => m.Reaction).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return Reactions;
        }

      
        public async Task<List<Allergy>> GetAllergyMedicationSubstanceId(string medicationSubstance , int patientId)
        {
            var Allergy = await _applicationDbcontext.Allergies.Where( a => a.MedicationSubstance.Name == medicationSubstance && a.Patient.Id == patientId).ToListAsync();
            return Allergy;
        }

        public async Task<List<Allergy>> GetAllAllergyByPatientId( int patientId)
        {
            var Allergy = await _applicationDbcontext.Allergies.Where(a => a.Patient.Id == patientId).ToListAsync();
            return Allergy;
        }

        public void DeleteMedicationSubstanceForServiceTakeAwayId(MedicationSubstance medicationSubstance)
        {
            _applicationDbcontext.MedicationSubstances.Remove(medicationSubstance);
        }
    } 
}
