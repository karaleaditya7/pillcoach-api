using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
   
    public class SideEffectData :ISideEffectData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public SideEffectData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<SideEffect> AddPatientSideEffect(SideEffect sideEffect,bool commitChanges)
        {
            var result = await _applicationDbcontext.SideEffects.AddAsync(sideEffect);
            if(commitChanges) await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<List<SideEffect>> GetSideEffectByPatientIdAndMedicationSubstanceId(int patientid, int medicationsubstanceid)
        {
            var sideEffects = await _applicationDbcontext.SideEffects.Where(a => a.Patient.Id == patientid && a.MedicationSubstance.Id == medicationsubstanceid)
                  .ToListAsync();
            return sideEffects;
        }
        public async Task SaveChangesAsync()
        {
            await _applicationDbcontext.SaveChangesAsync();
        }

        public async Task<List<int>> GetReactionyBySideEffectId(int sideeffectId)
        {
            var reactions = await _applicationDbcontext.SideEffects.Where(a => a.Id == sideeffectId).
                  Select(a => a.Reaction.Id).ToListAsync();
            return reactions;
        }
        public async Task<List<string>> GetReactionyBySideEffectId(int patientid, int medicationsubstanceid)
        {
            return await _applicationDbcontext.SideEffects.Include(a => a.MedicationSubstance).Where(a => a.Patient.Id == patientid && a.MedicationSubstance.Id == medicationsubstanceid).Select(a => a.MedicationSubstance.Name).ToListAsync();
        }

        public async Task<SideEffect> GetSideEffectBySideEffectId(int sideeffectId)
        {
            var sideeffect = await _applicationDbcontext.SideEffects.Where(a => a.Id == sideeffectId).FirstOrDefaultAsync();

            return sideeffect;
        }

        public async Task<Reaction> GetReactionByReactionId(int reactionId)
        {
            var reaction = await _applicationDbcontext.Reactions.Where(a => a.Id == reactionId).FirstOrDefaultAsync();

            return reaction;
        }

        public async Task<SideEffect> UpdateReactionBySideEffect(SideEffect sideEffect)
        {
            var result = _applicationDbcontext.SideEffects.Update(sideEffect);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<Reaction>> GetAllSideEffectsReactionsByPatientId(int id)
        {
            var reactions = await _applicationDbcontext.SideEffects.Where(m => m.Patient.Id == id).
                                             Select(a => a.Reaction).
                                            ToListAsync();
            return reactions;
        }
        public async Task<List<MedicationSubstance>> GetMedicationSubstancesByPatientIdForSideEffect(int id)
        {
            var MedicationSubstances = await _applicationDbcontext.SideEffects.Where(m => m.Patient.Id == id)
                                             .Select(m => m.MedicationSubstance).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return MedicationSubstances;
        }

        public async Task<List<SideEffect>> GetAllSideEffectReactionsByMedicationSubstanceById(int id,int patientId)
        {
            var sideEffects = await _applicationDbcontext.SideEffects.Where(m => m.MedicationSubstance.Id == id && m.Patient.Id == patientId).Include(m => m.Reaction).GroupBy(m => m.Reaction.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return sideEffects;
        }
        public async Task<SideEffect> GetSideEffectById(int sideEffectId)
        {
            return await _applicationDbcontext.SideEffects.Where(a => a.Id == sideEffectId).FirstAsync();
        }

        public async Task<List<SideEffect>> GetSideEffectByPatientId(int patientId)
        {
            
            return await _applicationDbcontext.SideEffects.Include(a => a.MedicationSubstance).Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public async Task DeleteSideEffect(SideEffect sideEffect)
        {
         
            _applicationDbcontext.SideEffects.Remove(sideEffect);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public void DeleteSideEffectBySideEffectForServiceTakeAway(SideEffect sideEffect)
        {

            _applicationDbcontext.SideEffects.Remove(sideEffect);
          

        }

        public void DeleteMedicationSubstanceForSideEffect(MedicationSubstance medicationSubstance)
        {

            _applicationDbcontext.MedicationSubstances.Remove(medicationSubstance);


        }

        public async Task<List<SideEffect>> GetSideEffectByMedicationSubstanceAndPatientId(int medicationSubstanceId, int patientId)
        {
            return await _applicationDbcontext.SideEffects.Where(a => a.MedicationSubstance.Id == medicationSubstanceId && a.Patient.Id == patientId).ToListAsync();
        }
        public async Task<List<SideEffect>> GetSideEffectMedicationSubstanceId(string medicationSubstance, int patientId)
        {
           var SideEffect = await _applicationDbcontext.SideEffects.Where(a =>a.MedicationSubstance.Name == medicationSubstance && a.Patient.Id==patientId).ToListAsync();
           return SideEffect;
        }
        public async Task<List<SideEffect>> GetAllSideEffectByPatientId(int patientId)
        {
            var sideEffects = await _applicationDbcontext.SideEffects.Where(a => a.Patient.Id == patientId).ToListAsync();
            return sideEffects;
        }
        public void PatientDeleteForSideEffect(SideEffect sideEffect)
        {
            _applicationDbcontext.SideEffects.Remove(sideEffect);


        }
    }
}
