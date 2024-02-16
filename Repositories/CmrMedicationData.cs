using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class CmrMedicationData : ICmrMedicationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public CmrMedicationData(ApplicationDbContext applicationDbcontext, IHttpClientFactory clientFactory)
        {
            _applicationDbcontext = applicationDbcontext;
        }


        public async Task<CmrMedication> AddCmrMedication(CmrMedication cmrMedication)
        {
            var result = await _applicationDbcontext.CmrMedications.AddAsync(cmrMedication);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<string>> GetUniqueConditionForCMRByPatientId(int patientId)
        {
            var medications = await _applicationDbcontext.CmrMedications.
                                   Where(p => p.Patient.Id == patientId).GroupBy(x => x.Condition).
                                   Select(p => p.FirstOrDefault().Condition).ToListAsync();

            return medications;
        }
        public IQueryable<RxNavMedication> SearchForRxNavMedication(string text)
        {
            var result = _applicationDbcontext.RxNavMedications.Where(r=>r.SBDCName.Contains(text)).GroupBy(r=>r.SBDCName).Select(r=>r.FirstOrDefault()).Take(20);
            
            return result;
        }

        public async Task<List<string>> SearchForRxNavMedicationSBDC(string text)
        {
            var result = await _applicationDbcontext.RxNavMedications.Where(r => r.SBDCName.Contains(text)).GroupBy(r => r.SBDCName).
                Select(r => r.FirstOrDefault().SBDCName).Take(20).ToListAsync();


            return result;
        }

        public async Task<List<string>> SearchForRxNavMedicationGPCK(string text)
        {
            var result = await _applicationDbcontext.RxNavMedications.Where(r => r.GenericName.Contains(text)).GroupBy(r => r.GenericName).
                Select(r => r.FirstOrDefault().GenericName).Take(20).ToListAsync();

            return result;
        }

        public async Task<List<CmrMedication>> GetUniqueCmrMedicationsByPatientId(int patientId)
        {
            List<CmrMedication> cmrMedications = await _applicationDbcontext.CmrMedications.Where(m => m.Patient.Id == patientId).
                                            Include(x=>x.DoctorPrescribed).
                                            Include(x=>x.DoctorPrescribed.Contact).
                                            Include(x=>x.Patient).
                                            Include(x=>x.Patient.Contact).

                                            ToListAsync();

            return cmrMedications;
        }

        public async Task<List<Doctor>> GetAllDoctors(int id)
        {
            List<Doctor> doctors = await _applicationDbcontext.CmrMedications.Include(m => m.DoctorPrescribed.Contact).Where(m => m.Patient.Id == id)
                .Select(d => d.DoctorPrescribed).GroupBy(d => d.Id).Select(d => d.FirstOrDefault())
                .ToListAsync();
            return doctors;
        }

        public async Task<CmrMedication> GetCmrMedicationsById(int id)
        {
            var cmrMedication = await _applicationDbcontext.CmrMedications.Where(m => m.Id == id).
                                            Include(x => x.Patient).
                                            Include(x => x.Patient.Contact).
                                            Include(c=>c.DoctorPrescribed).
                                            Include(c=>c.DoctorPrescribed.Contact).
                                            SingleOrDefaultAsync();

            return cmrMedication;
        }

        public async Task DeleteCmrMedication(CmrMedication cmrMedication)
        {
             _applicationDbcontext.CmrMedications
                .Remove(cmrMedication);

            await _applicationDbcontext.SaveChangesAsync();
        }

        public void DeleteCmrMedicationForServiceTakeAwayInformation(CmrMedication cmrMedication)
        {
            _applicationDbcontext.CmrMedications.Remove(cmrMedication);
            
        }

        public void DeleteCmrMedicationForPatient(CmrMedication cmrMedication)
        {
            _applicationDbcontext.CmrMedications
                .Remove(cmrMedication);

        }

        public async Task<CmrMedication> UpdateCmrMedication(CmrMedication cmrMedication)
        {
            await _applicationDbcontext.SaveChangesAsync();
            return cmrMedication;
        }

        public bool ValidateCmrMedication(CmrMedication cmrMedication)
        {
            bool isAnyPropEmpty = cmrMedication.GetType().GetProperties()
               .Where(pi => pi.PropertyType == typeof(string))
               .Select(pi => (string)pi.GetValue(cmrMedication))
               .Any(value => string.IsNullOrEmpty(value));
            return isAnyPropEmpty;

        }
    }
}
