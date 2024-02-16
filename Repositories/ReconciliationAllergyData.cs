using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class ReconciliationAllergyData : IReconciliationAllergyData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public ReconciliationAllergyData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<List<ReconciliationAllergy>> GetRecAllergyMedicationSubstanceId(string medicationSubstance, int patientId)
        {
            var ReconciliationAllergy = await _applicationDbcontext.ReconciliationAllergies.Where(a => a.MedicationSubstance.Name == medicationSubstance && a.Patient.Id == patientId).ToListAsync();
            return ReconciliationAllergy;
        }

        public async Task<ReconciliationAllergy> AddPatientReconciliationAllergy(ReconciliationAllergy reconciliation, bool commitChanges)
        {
            var result = await _applicationDbcontext.ReconciliationAllergies.AddAsync(reconciliation);
            if (commitChanges) await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task SaveChangesAsync()
        {
            await _applicationDbcontext.SaveChangesAsync();
        }

        public async Task<List<ReconciliationAllergy>> GetReconciliationAllergyByAllergyId(int patientid, int medicationsubstanceid)
        {
            var reconciliationAllergies = await _applicationDbcontext.ReconciliationAllergies.Where(a => a.Patient.Id == patientid && a.MedicationSubstance.Id == medicationsubstanceid)
                .ToListAsync();
            return reconciliationAllergies;
        }

        public async Task DeleteReconciliationAllergyById(ReconciliationAllergy reconciliationAllergy)
        {
            _applicationDbcontext.ReconciliationAllergies.Remove(reconciliationAllergy);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public void DeleteRecAllergyByServiceTakeAwayMedReconciliation(ReconciliationAllergy reconciliationAllergy)
        {
            _applicationDbcontext.ReconciliationAllergies.Remove(reconciliationAllergy);

        }

        public async Task<List<Reaction>> GetAllRecAllergyReactionsByPatientId(int id)
        {
            var reactions = await _applicationDbcontext.ReconciliationAllergies.Where(m => m.Patient.Id == id).
                                             Select(a => a.Reaction).
                                            ToListAsync();
            return reactions;
        }

        public async Task<List<MedicationSubstance>> GetMedSubstancesByPatientIdForRecAllergy(int id)
        {
            var MedicationSubstances = await _applicationDbcontext.ReconciliationAllergies.Where(m => m.Patient.Id == id).Select(m => m.MedicationSubstance).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return MedicationSubstances;
        }

        public async Task<List<MedicationSubstance>> GetRecAllergyMedSubstancesByPatientIdForAllergy(int id)
        {
            var MedicationSubstances = await _applicationDbcontext.ReconciliationAllergies.Where(m => m.Patient.Id == id).Select(m => m.MedicationSubstance).GroupBy(m => m.Id).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
            return MedicationSubstances;
        }

        public async Task<List<ReconciliationAllergy>> GetAllRecAllergyReactionsByMedSubstanceById(int id, int patientId)
        {
            var Reactions = await _applicationDbcontext.ReconciliationAllergies.Where(m => m.MedicationSubstance.Id == id && m.Patient.Id == patientId).Include(m => m.Reaction).
                                            ToListAsync();
            return Reactions;
        }

        public async Task<List<ReconciliationAllergy>> GetReconciliationAllergyById(int medicationSubstanceId, int patientId)
        {
            return await _applicationDbcontext.ReconciliationAllergies.Where(a => a.MedicationSubstance.Id == medicationSubstanceId && a.Patient.Id == patientId).ToListAsync();
        }

        public async Task<List<ReconciliationAllergy>> GetReconciliationAllergyByPatientId(int patientId)
        {
            return await _applicationDbcontext.ReconciliationAllergies.Include(a => a.MedicationSubstance).Where(a => a.Patient.Id == patientId).ToListAsync();
        }


        public void PatientDeleteForReconciliationAllergy(ReconciliationAllergy reconciliationAllergy)
        {
            _applicationDbcontext.ReconciliationAllergies.Remove(reconciliationAllergy);

        }


    }
}
