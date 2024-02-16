using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class DoctorMedicationData : IDoctorMedicationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public DoctorMedicationData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext= applicationDbcontext;    
        }

        public async Task<DoctorMedication> AddDoctorMedication(DoctorMedication doctorMedication)
        {
            var result = await _applicationDbcontext.DoctorMedications.AddAsync(doctorMedication);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<DoctorMedication>> GetDoctorMedicationByCmrId(int cmrId)
        {
           
            var result = await _applicationDbcontext.DoctorMedications.Include(d => d.CmrMedication).Where(d => d.CmrMedication.Id == cmrId).ToListAsync();
            return result;
        }
        public async Task<List<DoctorMedication>> GetDoctorMedicationByMedId(int medId)
        {

            var result = await _applicationDbcontext.DoctorMedications.Include(d => d.Medication).Where(d => d.Medication.Id == medId).ToListAsync();
            return result;
        }
        public async Task<List<Doctor>> GetAlldoctorsforMedication(int patientId)
        {
            List<Doctor> doctors = await _applicationDbcontext.Medications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.Patient.Id == patientId).GroupBy(d => d.DrugSubGroup).Select(d => d.FirstOrDefault().DoctorPrescribed).ToListAsync();
            return doctors;
        }
        public async Task<List<Doctor>> GetAlldoctorsforCmrMedication(int patientId)
        {
            List<Doctor> doctors = await _applicationDbcontext.DoctorMedications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.CmrMedication.Patient.Id == patientId).Select(d => d.DoctorPrescribed).GroupBy(d => d.Id).Select(d => d.FirstOrDefault()).ToListAsync();
            return doctors;
        }
        public async Task<List<Doctor>> GetAlldoctorsforMedRecMedication(int patientId)
        {
            List<Doctor> doctors = await _applicationDbcontext.DoctorMedications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.MedicationReconciliation.Patient.Id == patientId).Select(d => d.DoctorPrescribed).GroupBy(d => d.Id).Select(d => d.FirstOrDefault()).ToListAsync();
            return doctors;
        }
        public async Task<List<DoctorMedication>> GetDoctorMedicationByMedRecId(int medRecId)
        {
            var result = await _applicationDbcontext.DoctorMedications.Include(d => d.MedicationReconciliation).Where(d => d.MedicationReconciliation.Id == medRecId).ToListAsync(); 
            return result;
        }

        public async Task<List<DoctorMedication>> GetDoctorMedicationByOtcMedId(int otcMedId)
        {
            var result = await _applicationDbcontext.DoctorMedications.Include(d => d.OtcMedication).Where(d => d.OtcMedication.Id == otcMedId).ToListAsync();
            return result;
        }
        public async Task<List<Doctor>> GetAlldoctorsforOtc(int patientId)
        {
            List<Doctor> doctors = await _applicationDbcontext.OtcMedications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.Patient.Id == patientId && !m.IsDeleted).Select(d => d.DoctorPrescribed).GroupBy(d => d.Id).Select(d => d.FirstOrDefault()).ToListAsync();
            return doctors;
        }
        public void DeleteDoctorMedicationForCmr(DoctorMedication doctorMedication)
        {
            _applicationDbcontext.DoctorMedications.Remove(doctorMedication);

        }

        public void DeleteDoctorMedicationForMedication(DoctorMedication doctorMedication)
        {
            _applicationDbcontext.DoctorMedications.Remove(doctorMedication);

        }

        public void DeleteDoctorMedicationForMedRec(DoctorMedication doctorMedication)
        {
            _applicationDbcontext.DoctorMedications.Remove(doctorMedication);

        }
        public void DeleteDoctorMedicationForOtcMedication(DoctorMedication doctorMedication)
        {
            _applicationDbcontext.DoctorMedications.Remove(doctorMedication);

        }
    }
}
