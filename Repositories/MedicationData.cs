using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Repositories
{
    public class MedicationData:IMedicationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        
        public MedicationData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
            
        }
        public async Task<List<Medication>> GetMedicationByPatientId(int patientId)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(p => p.Patient.Id == patientId).ToListAsync();

            return medications;
        }
        public async Task<List<Medication>> GetMedicationByPatientIdAndRxNumber(int patientId,string rxNumber)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(p => p.Patient.Id == patientId && p.RxNumber == rxNumber).ToListAsync();

            return medications;
        }
        public async Task<List<Medication>> GetMedicationByRxNumber(string rxNumber)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(p => p.RxNumber == rxNumber).ToListAsync();

            return medications;
        }

        public async Task<List<string>> GetUniqueConditionByPatientId(int patientId)
        {
            var medications = await _applicationDbcontext.Medications.
                                   Where(p => p.Patient.Id == patientId).GroupBy(x => x.DrugSubGroup).
                                   Select(p => p.FirstOrDefault().OptionalCondition).ToListAsync();

            return medications;
        }


        public async Task<List<Medication>> GetMedicationdetails(string rxNumber)
        {
            var medications = await _applicationDbcontext.Medications.Where(m => m.RxNumber == rxNumber).
                                                                      Include(m => m.Patient).
                                                                      Include(m => m.Patient.Pharmacy).ToListAsync();

            return medications;
        }

        public async Task<List<Pdc_Medication>> GetAllPdcMedication()
        {
            var pdcMedications = await _applicationDbcontext.Pdc_Medications.ToListAsync();

            return pdcMedications;
        }

        public async Task<List<Medication>> GetMedicationByCondition(string condition)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(m => m.Condition == condition).
            Include(m => m.Patient).
            Include(m => m.Patient.Address).
            Include(m => m.Patient.Contact).
            ToListAsync();

            return medications;
        }

        public async Task<List<Patient>> GetPatientsByUserId(string userId)
        {
            var patients = await _applicationDbcontext.Patients
                      .Include(a => a.Address)
                      .Include(c => c.Contact)
                      .Where(p => p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId))
                      .ToListAsync();

            return patients;
        }
        public async Task<Medication> GetMedicationByVendorRxID(string vendorRxID)
        {
            var medications = await _applicationDbcontext.Medications.Include(m => m.Patient).
            Where(p => p.RxVendorRxID == vendorRxID).FirstOrDefaultAsync();

            return medications;
        }


        public async Task<Pdc_Medication> GetPdcMedicationWithNdcNumber(string ndcNumber)
        {
            var pdcMedication = await _applicationDbcontext.Pdc_Medications.
            Where(p => p.code == ndcNumber ).FirstOrDefaultAsync();

            return pdcMedication;
        }

        public async Task<List<Medication>> getMedicationByNDCNumber(string nDCNumber)
        {
            var medication = await _applicationDbcontext.Medications.Include(m => m.Patient).
            Where(p => p.RxVendorRxID == nDCNumber).ToListAsync();

            return medication;
        }
        public async Task<List<Medication>> GetUniqueMedicationByDrugSubGroup()
        {

            var medications = await _applicationDbcontext.Medications.
                              GroupBy(p => p.DrugSubGroup).
                              Select(p => p.FirstOrDefault()).ToListAsync();
            return medications;
        }
        public async Task<List<MedicationCondition>> GetAllMedicationCondition()
        {
            var medicationConditions = await _applicationDbcontext.MedicationConditions.ToListAsync();
            return medicationConditions;
        }

        public async Task<MedicationCondition> GetMedicationConditionByName(string text)
        {
            //var medicationCondition = await _applicationDbcontext.MedicationConditions.Where(r => r.Name.Contains(text)).FirstOrDefaultAsync();
            var medicationCondition = await _applicationDbcontext.MedicationConditions.Where(r => r.Name == text).FirstOrDefaultAsync();
            return medicationCondition;
        }

        public async Task<MedicationCondition> AddMedicationCondition(MedicationCondition medicationCondition)
        {
             var result = await _applicationDbcontext.MedicationConditions.AddAsync(medicationCondition);
              
            return result.Entity;
        }


        public async Task<List<Medication>> GetUniqueMedicationByDrugSubGroupForGeneric(int recordNumber,int pageLimit, string userId,string keywords, string sortDirection)
        {
            bool conditionCheck = false;
            if (sortDirection == "asc")
            {
                conditionCheck = true;
            }
            List<Medication> medications = null;
         
            if (!string.IsNullOrWhiteSpace(userId)) // userId will be passed as null for SuperAdmin
            {
                medications = await _applicationDbcontext.Medications
                    .Join(_applicationDbcontext.Patients, m => m.Patient.Id, p => p.Id, (m, p) => new { m, p })
                    .Join(_applicationDbcontext.PharmacyUsers.Where(pu => pu.UserId == userId), a => a.p.Pharmacy.Id, b => b.PharmacyId, (a, b) => a.m).ToListAsync();
                medications = medications.Where(m => (keywords == null || keywords == string.Empty) || (!string.IsNullOrEmpty(m.SBDCName) && m.SBDCName.ToLower().Contains(keywords.ToLower())) || (!string.IsNullOrEmpty(m.GenericName) && m.GenericName.ToLower().Contains(keywords.ToLower()))).GroupBy(m => m.SBDCName ?? m.GenericName).OrderBy(m => conditionCheck ? m.Key : null)
                           .ThenByDescending(m => conditionCheck ? null : m.Key).Select(m => m.FirstOrDefault()).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                                   .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();

                foreach (var medication in medications)
                {
                    medication.AssignedPatient = _applicationDbcontext.Patients
                        .Join(_applicationDbcontext.PharmacyUsers.Where(p => p.UserId == userId), p => p.Pharmacy.Id, pu => pu.PharmacyId, (p, pu) => p)
                        .Where(m => m.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !m.IsDeleted).Count();
                }
            }
            else
            {
                medications = await _applicationDbcontext.Medications.Where(m => (keywords == null || keywords == string.Empty) || (!string.IsNullOrEmpty(m.SBDCName) && m.SBDCName.ToLower().Contains(keywords.ToLower())) || (!string.IsNullOrEmpty(m.GenericName) && m.GenericName.ToLower().Contains(keywords.ToLower()))).GroupBy(m => m.SBDCName ?? m.GenericName).OrderBy(m => conditionCheck ? m.Key : null)
                            .ThenByDescending(m => conditionCheck ? null : m.Key).Select(m => m.FirstOrDefault()).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                                    .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToListAsync();

                foreach (var medication in medications)
                {
                    medication.AssignedPatient = _applicationDbcontext.Patients
                            .Where(m => m.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !m.IsDeleted).Count();
                }

            }
            
            return medications;
        }


        public async Task<MedicationDto> GetUniqueMedicationByDrugSubGroupDto(string genericName,string sbdcName)
        {

            if (!string.IsNullOrWhiteSpace(genericName))
            {
                var medication = await _applicationDbcontext.Medications.Where(x => x.GenericName == genericName).Select(x => new MedicationDto
                {
                    Id = x.Id,
                    GenericName = x.GenericName,

                }).
                                  GroupBy(x => x.GenericName).Select(x => x.First())
                                  .SingleOrDefaultAsync();

                return medication;
            }
            if (!string.IsNullOrWhiteSpace(sbdcName))
            {
                var medication = await _applicationDbcontext.Medications.Where(x => x.SBDCName == sbdcName).Select(x => new MedicationDto
                {
                    Id = x.Id,
                    SbdcName = x.SBDCName,

                }).
                                  GroupBy(x => x.SbdcName).Select(x => x.First())
                                  .SingleOrDefaultAsync();

                return medication;
            }
            return null;
        

        }

        public async Task<List<Patient>> GetPatientByUniqueMedication(int recordNumber, int pageLimit, string genericName,string sbdcName, string userId,string keywords, string sortDirection,string filterType,string filterValue,string filterCategory)
        {
            IQueryable<Patient> query = null;
            bool conditionCheck = false;
            if (sortDirection == "asc")
            {
                conditionCheck = true;
            }

            if(!string.IsNullOrWhiteSpace(genericName)) 
            {
               // query = _applicationDbcontext.Patients
               //.Include(p => p.Pharmacy)
               //.Include(p => p.Contact)
               //.Include(p => p.Medications).ThenInclude(m => m.DoctorPrescribed).ThenInclude(m => m.Contact)
               //.Where(m => m.Medications.Any(Medication => Medication.GenericName == genericName) && !m.IsDeleted);

                query = _applicationDbcontext.Patients.Include(p => p.Contact)
                           .Include(p => p.Pharmacy)
                           .Include(p => p.Contact)
                           .Include(p => p.Medications)
                               .ThenInclude(m => m.DoctorPrescribed)
                                   .ThenInclude(dp => dp.Contact)
                           .Where(m => m.Medications.Any(Medication => Medication.GenericName == genericName) && !m.IsDeleted)
                           .Select(m => new Patient
                           {
                               Id = m.Id,
                               Medications = m.Medications
                                   .Where(med => med.IsActive)
                                   .OrderByDescending(med => med.LastFillDate)
                                   .ToList(),
                               Contact = m.Contact, 
                               Pharmacy = m.Pharmacy,
                               Status = m.Status,
                               ImportSourceFileId = m.ImportSourceFileId,
                               NoteId = m.NoteId,
                               PatientVendorRxID = m.PatientVendorRxID,
                           });






            }

            if (!string.IsNullOrWhiteSpace(sbdcName))
            {
                // query = _applicationDbcontext.Patients
                //.Include(p => p.Pharmacy)
                //.Include(p => p.Contact)
                //.Include(p => p.Medications).ThenInclude(m => m.DoctorPrescribed).ThenInclude(m => m.Contact)
                //.Where(m => m.Medications.Any(Medication => Medication.SBDCName == sbdcName) && !m.IsDeleted);

                query = _applicationDbcontext.Patients.Include(p => p.Contact)
                          .Include(p => p.Pharmacy)
                          .Include(p => p.Contact)
                          .Include(p => p.Medications)
                              .ThenInclude(m => m.DoctorPrescribed)
                                  .ThenInclude(dp => dp.Contact)
                          .Where(m => m.Medications.Any(Medication => Medication.SBDCName == sbdcName) && !m.IsDeleted)
                          .Select(m => new Patient
                          {
                              Id = m.Id,
                              Medications = m.Medications
                                  .Where(med => med.IsActive)
                                  .OrderByDescending(med => med.LastFillDate)
                                  .ToList(),
                              Contact = m.Contact,
                              Pharmacy = m.Pharmacy,
                              Status = m.Status,
                              ImportSourceFileId = m.ImportSourceFileId,
                              NoteId = m.NoteId,
                              PatientVendorRxID = m.PatientVendorRxID,
                          });
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(m => m.Pharmacy.PharmacyUsers.Any(pu => pu.UserId == userId));
            }
            
            if (!string.IsNullOrWhiteSpace(filterType))
            {
                switch (filterType)
                {
                    case "Doctor Name":
                        query = query.Where(m => m.Medications.Any(Medication => Medication.DoctorPrescribed.Contact.FirstName.ToLower().Contains(filterValue.ToLower()) || Medication.DoctorPrescribed.Contact.LastName.ToLower().Contains(filterValue.ToLower())));
                        break;
                    case "Organization":
                        query = query.Where(m => m.Pharmacy.Name.ToLower().Contains(filterValue.ToLower()));
                        break;
                    case "Conditions":
                        query = query.Where(m => m.Medications.Any(Medication => Medication.Condition.ToLower().Contains(filterValue.ToLower())));
                        break;
                    case "PDC Category and Average":
                        query = query.Where(m => m.Pharmacy.PharmacyUsers.Any(pu => pu.UserId == userId));
                        break;
                }
                return query.Skip(recordNumber).Take(pageLimit).ToList();
            }


            if (!string.IsNullOrEmpty(keywords))
            {
                query = query.Where(m => (keywords == null || keywords == string.Empty ||
                ((m.Contact.FirstName + " " + m.Contact.LastName).ToLower().Contains(keywords.ToLower()))));
            }

            return await query.OrderBy(p => conditionCheck ? p.Contact.FirstName : null)
                .ThenByDescending(p => conditionCheck ? null : p.Contact.FirstName)
                .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                .ToListAsync();


        }

        public async Task<List<Medication>> GetUniqueMedicationByPatientId(int id)
        {
            List<Medication> medications = await _applicationDbcontext.Medications.Where(m => m.Patient.Id == id).
                                            GroupBy(x => x.DrugSubGroup).
                                            Select(p => p.FirstOrDefault()).
                                            ToListAsync();
                                            

            return medications;
        }


        public async Task<List<Medication>> GetUniqueMedicationsByPatientIdForCmrMedication(int patientId)
        {
            List<Medication> medications = await _applicationDbcontext.Medications.Where(m => m.Patient.Id == patientId).Include(m=>m.Patient).
                                            Include(m=>m.Patient.Contact).
                                            Include(m=>m.Patient.Address).
                                            Include(m=>m.DoctorPrescribed).
                                            Include(m=>m.DoctorPrescribed.Contact).
                                            GroupBy(m => m.SBDCName ?? m.GenericName ?? m.DrugName).
                                            Select(p => p.OrderByDescending(p => p.LastFillDate).FirstOrDefault()).
                                            ToListAsync();


            return medications;
        }

        public async Task<List<Medication>> GetDistintMedicationsByConditionAndPatientId(string condition,int patientId)
        {
            var medications = await _applicationDbcontext.Medications.
                                Where(p => p.Patient.Id == patientId && p.Condition == condition).
                                GroupBy(p => p.RfNumber).Select(p => p.First()).ToListAsync();
            return medications;
        }

        public async Task<List<Medication>> GetDistintMedicationsByConditionAndPatientIdForEarlyMedicationPDC(string condition, int patientId)
        {
            var medications = await _applicationDbcontext.Medications.
                                   Include(p =>p.Patient).
                                Where(p => p.Patient.Id == patientId && p.Condition == condition)
                                .ToListAsync();
            return medications;
        }

        public void DeleteMedication(Medication medication)
        {
             _applicationDbcontext.Medications.Remove(medication);
            
        }

        public async Task<List<Medication>> GetDistintMedicationsByConditionAndPatientIdAndSubgroupForEarlyMedicationPDC(string condition, int patientId, string value_set_subgroup)
        {
            var medications = await _applicationDbcontext.Medications.
                                   Include(p => p.Patient).
                                Where(p => p.Patient.Id == patientId && p.Condition == condition && p.DrugSubGroup == value_set_subgroup)
                                .ToListAsync();
            return medications;
        }


        public async Task<Medication> AddMedication(Medication medication)
        {
            var result = await _applicationDbcontext.Medications.AddAsync(medication);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public Medication UpdateMedicationWebhook(Medication medication)
        {
            var result = _applicationDbcontext.Medications.Update(medication);
            return result.Entity;
        }
        public async Task<Medication> UpdateMedication(Medication medication)
        {

            await _applicationDbcontext.SaveChangesAsync();
            return medication;
        }

        public async Task<Medication> GetMedicationById(int medicationId)
        {
            var medicationExists = await _applicationDbcontext.Medications
            .Where(m => m.Id == medicationId).Include(m => m.Patient).SingleOrDefaultAsync();
            return medicationExists;
        }

        public Task<List<Medication>> getAllMedicationsByUserID(string userId)
        {
            return _applicationDbcontext.Medications.Include(p=>p.Patient)
           .Where(p => p.Patient.Pharmacy.PharmacyUsers.Any(PharmacyUser => PharmacyUser.UserId == userId))
           .ToListAsync();
        }

        public async Task<Doctor> getDoctorToMedicationByMedicationId(int id)
        {
            Doctor doctor = await _applicationDbcontext.Medications
           .Where(m=>m.Id == id).Select(m=>m.DoctorPrescribed)
           .SingleOrDefaultAsync();
            return doctor;
        }
        public async Task<List<Medication>> getAllMedicationsByPharmacyID(int pharmacyId)
        {
            return await _applicationDbcontext.Medications
           .Where(p => p.Patient.Pharmacy.PharmacyUsers.Any(PharmacyUser => PharmacyUser.PharmacyId == pharmacyId))
           .ToListAsync();
        }

        public async Task<List<Medication>> getAllMedicationsByPharmacyIDForAll(int pharmacyId)
        {
            return await _applicationDbcontext.Medications
           .Where(p => p.Patient.Pharmacy.Id== pharmacyId).Include(x=>x.Patient)
           .ToListAsync();
        }

        public async  Task<int> GetAssignedPharmacyCount(int pharmacyId)
        {
            
            int count = 0;
            var medications = await _applicationDbcontext.Medications.Where(p => p.Patient.Pharmacy.Id == pharmacyId).ToListAsync();
            foreach(Medication medication in medications)
            {
                count += 1;
            }

            return count;
        }

        public async Task<int> GetAssignedPatientCount(int patientId)
        {

            int count = 0;
            var medications = await _applicationDbcontext.Medications.Where(p => p.Patient.Id == patientId).ToListAsync();
            foreach (Medication medication in medications)
            {
                count += 1;
            }

            return count;
        }

        public async Task DeleteMedicationHardCore(int patientId)
        {

            List<Medication> medications = await this.GetMedicationByPatientId(patientId);
            _applicationDbcontext.Medications.RemoveRange(medications);

        }

        public async Task<List<MedicationHistory>> GetPatientMedicationHistoryByDrugNameAsync(int patientId, string drugName)
        {
            var query = _applicationDbcontext.Medications
                .Where(m => m.Patient.Id == patientId && (m.SBDCName ?? m.GenericName ?? m.DrugName) == drugName)
                .Select(m => new MedicationHistory
                {
                    PatientId = m.Patient.Id,
                    RxNumber = m.RxNumber,
                    RxDate = m.RxDate,
                    Quantity = m.Quantity,
                    Supply = m.Supply,
                    PrescriberName = $"{m.DoctorPrescribed.Contact.FirstName} {m.DoctorPrescribed.Contact.LastName}",
                    LastFillDate = m.LastFillDate,
                    NextFillDate = m.NextFillDate,
                    RefillRemaining = Convert.ToInt32(m.RefillsRemaining),
                    PatientPayDue = m.PayDue,
                    RefillNumber = Convert.ToInt32(m.RfNumber),
                    PrimaryThirdParty = m.PrimaryThirdParty
                });
            
            return await query.ToListAsync();
        }

        public async Task<MedicationUsageStatusUpdateResult> SetMedicationUsageStatusAsync(int medicationId, bool status)
        {
            var result = new MedicationUsageStatusUpdateResult();

            var medication = await _applicationDbcontext.Medications
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == medicationId);

            if (medication == null || medication.InUse == status) return null;

            medication.InUse = status;

            var today = DateTime.Today;

            if (!status)
            {
                await UpdateRefillDueStatusAsync(medication, status);
            }
            else if (today.AddDays(5) >= medication.NextFillDate && today.AddMonths(-4) < medication.NextFillDate)
            {
                /*
                 * in order to turn the refill-due flag ON, following conditions should be met:
                 * - next fill date should not be beyond 5 days from current date
                 * - and, should be within the last 4 months from current day
                 */
                await UpdateRefillDueStatusAsync(medication, status);
            }

            if (await _applicationDbcontext.SaveChangesAsync() > 0)
            {
                result.UsageStatus = status;
                result.RefillDueStatus = medication.RefillDue;

                return result;
            }

            return null;
        }

        public async Task<bool> SetMedicationRefillDueStatusAsync(int medicationId, bool status)
        {
            var medication = await _applicationDbcontext.Medications
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.Id == medicationId);

            if (medication == null) return false;
            
            await UpdateRefillDueStatusAsync(medication, status);

            return await _applicationDbcontext.SaveChangesAsync() > 0;
        }

        public async Task<string> GetExlusionMedication(int patientId, string condition)
        {
            if(condition == "RASA")
            {
                var medicationCount =  _applicationDbcontext.Medications.Where(x => x.Patient.Id == patientId && x.Condition == "Sacubitril_Valsartan").Any(); 
                if (medicationCount) 
                {
                    return "Entresto";
                }
                else
                {
                    return null;
                }
               
            }
            if (condition == "Diabetes")
            {
                var medicationCount =  _applicationDbcontext.Medications.Where(x => x.Patient.Id == patientId && x.Condition == "Insulins").Any();
                if (medicationCount)
                {
                    return "Insulin";
                }
                else
                {
                    return null;
                }

            }
            return null;
        }

        private async Task UpdateRefillDueStatusAsync(Medication medication, bool status)
        {
            if (medication == null || medication.RefillDue == status) return;

            medication.RefillDue = status;

            var pdcCategories = new[]
            {
                "Diabetes",
                "Cholesterol",
                "RASA"
            };

            if (pdcCategories.Any(c => c.Equals(medication.Condition, StringComparison.OrdinalIgnoreCase)))
            {
                if (medication.RefillDue)
                {
                    switch (medication.Condition)
                    {
                        case "Diabetes":
                            medication.Patient.DiabetesRefillDue = true;
                            break;

                        case "Cholesterol":
                            medication.Patient.CholesterolRefillDue = true;
                            break;

                        case "RASA":
                            medication.Patient.RasaRefillDue = true;
                            break;
                    }
                }
                else
                {
                    var otherMedWithRefillDue = await _applicationDbcontext.Medications
                        .Where(m => m.Patient.Id == medication.Patient.Id && m.Condition == medication.Condition && !m.RefillDue)
                        .FirstOrDefaultAsync();

                    if (otherMedWithRefillDue == null)
                    {
                        switch (medication.Condition)
                        {
                            case "Diabetes":
                                medication.Patient.DiabetesRefillDue = false;
                                break;

                            case "Cholesterol":
                                medication.Patient.CholesterolRefillDue = false;
                                break;

                            case "RASA":
                                medication.Patient.RasaRefillDue = false;
                                break;
                        }
                    }
                }
            }
        }
    }
}
