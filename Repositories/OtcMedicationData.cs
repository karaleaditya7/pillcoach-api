using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class OtcMedicationData : IOtcMedicationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public OtcMedicationData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }
        public async Task<OtcMedication> AddOtcMedication(OtcMedication otcMedication)
        {
            var result = await _applicationDbcontext.OtcMedications.AddAsync(otcMedication);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<List<OtcMedication>> GetOtcMedicationsByPatientId( int patientId)
        {
                var otcMedications = await _applicationDbcontext.OtcMedications.Where(m => m.Patient.Id == patientId && !m.IsDeleted).
                                              Include(x => x.DoctorPrescribed).
                                              Include(x => x.DoctorPrescribed.Contact).
                                              Include(x => x.Patient).
                                              Include(x => x.Patient.Contact).
                                              ToListAsync();
            

            return otcMedications;
        }

        public async Task<List<OtcMedication>> GetOtcMedicationsByPatientIdWithPagination(int recordNumber, int pageLimit, int patientId)
        {
            List<OtcMedication> otcMedications = null;
            if (recordNumber >= 0 && pageLimit != 0)
            {
                otcMedications = await _applicationDbcontext.OtcMedications.Where(m => m.Patient.Id == patientId && !m.IsDeleted).
                                            Include(x => x.DoctorPrescribed).
                                            Include(x => x.DoctorPrescribed.Contact).
                                            Include(x => x.Patient).
                                            Include(x => x.Patient.Contact).Skip(recordNumber).Take(pageLimit)
                                            .ToListAsync();
            }
            else
            {

                otcMedications = await _applicationDbcontext.OtcMedications.Where(m => m.Patient.Id == patientId && !m.IsDeleted).
                                             Include(x => x.DoctorPrescribed).
                                             Include(x => x.DoctorPrescribed.Contact).
                                             Include(x => x.Patient).
                                             Include(x => x.Patient.Contact).
                                             ToListAsync();
            }

            return otcMedications;
        }
        public async Task<List<string>> GetUniqueConditionForOTCByPatientId(int patientId)
        {
            var medications = await _applicationDbcontext.OtcMedications.
                                   Where(p => p.Patient.Id == patientId).GroupBy(x => x.Condition).
                                   Select(p => p.FirstOrDefault().Condition).ToListAsync();

            return medications;
        }

        public async Task<OtcMedication> GetOtcMedicationsById(int id)
        {
            var otcMedication = await _applicationDbcontext.OtcMedications.Where(m => m.Id == id && !m.IsDeleted).
                                            Include(x => x.Patient).
                                            Include(x => x.Patient.Contact).
                                            Include(c => c.DoctorPrescribed).
                                            Include(c => c.DoctorPrescribed.Contact).
                                            SingleOrDefaultAsync();

            return otcMedication;
        }

        public async Task<OtcMedication> UpdateOtcMedication(OtcMedication otcMedication)
        {
            await _applicationDbcontext.SaveChangesAsync();
            return otcMedication;
        }

       
        public async Task<List<string>> GetAllConditionsforOtcMedication(int patientId)
        {
            List<string> conditions = await _applicationDbcontext.OtcMedications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.Patient.Id == patientId).Select(d => d.Condition).ToListAsync();
            return conditions;
        }

        public async Task<List<OtcMedication>> GetAllOtcMedicationsByPatientId(int patientId)
        {
            List<OtcMedication> otcMedications = await _applicationDbcontext.OtcMedications.Where(m => m.Patient.Id == patientId).
                                              Include(x => x.DoctorPrescribed).
                                              Include(x => x.DoctorPrescribed.Contact).
                                              Include(x => x.Patient).
                                              Include(x => x.Patient.Contact).
                                              ToListAsync();

            return otcMedications;
        }

        public void PatientDeleteForOtcMedication(OtcMedication otcMedication)
        {
            _applicationDbcontext.OtcMedications.Remove(otcMedication);

        }



    }
}
