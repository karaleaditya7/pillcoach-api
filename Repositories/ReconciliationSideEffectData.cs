using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace OntrackDb.Repositories
{
    public class ReconciliationSideEffectData : IReconciliationSideEffectData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public ReconciliationSideEffectData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<List<ReconciliationSideeffect>> GetRecSideeffectMedSubstanceId(string medicationSubstance, int patientId)
        {
            var ReconciliationSideeffects = await _applicationDbcontext.ReconciliationSideeffects.Where(a => a.MedicationSubstance.Name == medicationSubstance && a.Patient.Id == patientId).ToListAsync();
            return ReconciliationSideeffects;
        }

        public async Task<ReconciliationSideeffect> AddPatientReconciliationSideEffect(ReconciliationSideeffect reconciliationSideeffect, bool commitChanges)
        {
            var result = await _applicationDbcontext.ReconciliationSideeffects.AddAsync(reconciliationSideeffect);
            if (commitChanges) await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbcontext.SaveChangesAsync();
        }

        public async Task<List<ReconciliationSideeffect>> GetRecSideEffectByPatientIdAndMedSubstanceId(int patientid, int medicationsubstanceid)
        {
            var reconciliationSideeffects = await _applicationDbcontext.ReconciliationSideeffects.Where(a => a.Patient.Id == patientid && a.MedicationSubstance.Id == medicationsubstanceid)
                  .ToListAsync();
            return reconciliationSideeffects;
        }
        public async Task DeleteReconciliationSideEffect(ReconciliationSideeffect reconciliationSideeffect)
        {

            _applicationDbcontext.ReconciliationSideeffects.Remove(reconciliationSideeffect);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<List<Reaction>> GetAllRecSideEffectsReactionsByPatientId(int id)
        {
            var reactions = await _applicationDbcontext.ReconciliationSideeffects.Where(m => m.Patient.Id == id).
                                             Select(a => a.Reaction).
                                            ToListAsync();
            return reactions;
        }

        public async Task<List<MedicationSubstance>> GetMedSubstancesByPatientIdForSideEffect(int id)
        {
            var MedicationSubstances = await _applicationDbcontext.ReconciliationSideeffects.Where(m => m.Patient.Id == id)
                                             .Select(m => m.MedicationSubstance).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return MedicationSubstances;
        }

        public async Task<List<ReconciliationSideeffect>> GetAllSideEffectReactionsByMedSubstanceById(int id, int patientId)
        {
            var reconciliationSideeffects = await _applicationDbcontext.ReconciliationSideeffects.Where(m => m.MedicationSubstance.Id == id && m.Patient.Id == patientId).Include(m => m.Reaction).GroupBy(m => m.Reaction.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return reconciliationSideeffects;
        }

        public async Task<List<ReconciliationSideeffect>> GetRecSideEffectByMedSubstanceAndPatientId(int medicationSubstanceId, int patientId)
        {
            return await _applicationDbcontext.ReconciliationSideeffects.Where(a => a.MedicationSubstance.Id == medicationSubstanceId && a.Patient.Id == patientId).ToListAsync();
        }

        public async Task<List<ReconciliationSideeffect>> GetMedRecSideEffectByPatientId(int patientId)
        {

            return await _applicationDbcontext.ReconciliationSideeffects.Include(a => a.MedicationSubstance).Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void DeleteMedRecSideEffectByServiceTakeMedRecId(ReconciliationSideeffect reconciliationSideeffect)
        {

            _applicationDbcontext.ReconciliationSideeffects.Remove(reconciliationSideeffect);

        }

       

    }
}
