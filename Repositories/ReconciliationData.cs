using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class ReconciliationData : IReconciliationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public ReconciliationData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }
        public async Task<MedicationReconciliation> AddReconciliationMedication(MedicationReconciliation medicationReconciliation)
        {
            var result = await _applicationDbcontext.MedicationReconciliations.AddAsync(medicationReconciliation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<MedicationReconciliation>> GetUniqueReconciliationMedicationsByPatientId(int patientId)
        {
            List<MedicationReconciliation> medicationReconciliations = await _applicationDbcontext.MedicationReconciliations.Where(m => m.Patient.Id == patientId).
                                            Include(x => x.DoctorPrescribed).
                                            Include(x => x.DoctorPrescribed.Contact).
                                            Include(x => x.Patient).
                                            Include(x => x.Patient.Contact).

                                            ToListAsync();

            return medicationReconciliations;
        }

        public async Task<MedicationReconciliation> UpdateReconciliationMedication(MedicationReconciliation medicationReconciliation)
        {
            await _applicationDbcontext.SaveChangesAsync();
            return medicationReconciliation;
        }

        public async Task<MedicationReconciliation> GetReconciliationMedicationsById(int id)
        {
            var medicationReconciliation = await _applicationDbcontext.MedicationReconciliations.Where(m => m.Id == id).
                                            Include(x => x.Patient).
                                            Include(x => x.Patient.Contact).
                                            Include(c => c.DoctorPrescribed).
                                            Include(c => c.DoctorPrescribed.Contact).
                                            SingleOrDefaultAsync();

            return medicationReconciliation;
        }

        public async Task<List<string>> GetUniqueConditionForMedRecByPatientId(int patientId)
        {
            var medications = await _applicationDbcontext.MedicationReconciliations.
                                   Where(p => p.Patient.Id == patientId).GroupBy(x => x.Condition).
                                   Select(p => p.FirstOrDefault().Condition).ToListAsync();

            return medications;
        }

        public async Task DeleteReconciliationMedication(MedicationReconciliation medicationReconciliation)
        {
            _applicationDbcontext.MedicationReconciliations
                .Remove(medicationReconciliation);

            await _applicationDbcontext.SaveChangesAsync();
        }

        public  List<string> SearchForRxNavMedicationSBDC(string text)
        {
            var result =  _applicationDbcontext.RxNavMedications.Where(r => r.SBDCName.Contains(text)).GroupBy(r => r.SBDCName).
            Select(r => r.FirstOrDefault().SBDCName).Take(20).ToList();
            return result;
        }

        public List<string> SearchForRxNavMedicationGPCK(string text)
        {
            var result = _applicationDbcontext.RxNavMedications.Where(r => r.GenericName.Contains(text)).GroupBy(r => r.GenericName).
                Select(r => r.FirstOrDefault().GenericName).Take(20).ToList();
            return result;
        }

        public async Task<List<Doctor>> GetAllDoctors(int id)
        {
            List<Doctor> doctors = await _applicationDbcontext.MedicationReconciliations.Include(m => m.DoctorPrescribed.Contact).Where(m => m.Patient.Id == id).Select(d => d.DoctorPrescribed).GroupBy(d => d.Id).Select(d => d.FirstOrDefault()).ToListAsync();
            return doctors;
        }

        public void DeleteReconciliationMedicationForServiceTakeAwayMedReconciliation(MedicationReconciliation medicationReconciliation)
        {
             _applicationDbcontext.MedicationReconciliations
                .Remove(medicationReconciliation);
        }

        public async Task<ReconciliationToDoRelated> GetMedicationToDoListRelatedByMedicationReconciliationId(int medicationReconciliationId)
        {
            var result = await _applicationDbcontext.ReconciliationToDoRelateds.Include(m => m.MedicationReconciliation).Include(m => m.Patient).Where(m => m.MedicationReconciliation.Id == medicationReconciliationId).FirstOrDefaultAsync();
            return result;
        }

        public void PatientDeleteReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated)
        {

            _applicationDbcontext.ReconciliationToDoRelateds.Remove(reconciliationToDoRelated);

        }

        public async Task<List<MedicationReconciliation>> GetMedReconciliationByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.MedicationReconciliations.Where(s => s.Patient.Id == patientId).ToListAsync();

            return result;
        }

        public void PatientDeleteForMedicationReconciliation(MedicationReconciliation medicationReconciliation)
        {
            _applicationDbcontext.MedicationReconciliations.Remove(medicationReconciliation);

        }

        public void DeleteMedRecForPatient(MedicationReconciliation medicationReconciliation)
        {
             _applicationDbcontext.MedicationReconciliations
                .Remove(medicationReconciliation);

        }


    }
}
