
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Helper;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using static iTextSharp.text.pdf.PdfCopy;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Repositories
{
    public class PatientData : IPatientData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
      
        public PatientData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
           
        }

        public async Task<Patient> GetPatientById(int id)
        {

            var patientExists = await _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(p=>p.Pharmacy.Address).
            Include(p => p.Pharmacy.Contact).
            Include(c => c.Contact).
            Include(n => n.Note).
            Include(p => p.primaryThirdParty).
            Include(m => m.Medications).ThenInclude(m => m.DoctorPrescribed).ThenInclude(m => m.Contact).
            SingleAsync(x => x.Id == id );

            return patientExists;
        }

        public async Task<Patient> GetPatientBypatientId(int id)
        {

            var patientExists = await _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(p => p.Pharmacy.Address).
            Include(p => p.Pharmacy.Contact).
            Include(c => c.Contact).
            Include(n => n.Note).
            Include(m => m.primaryThirdParty).
            SingleAsync(x => x.Id == id);

            return patientExists;
        }

        public async Task<PatientDto> GetPatientByIdForPDCWithDto(int id)
        {

            var patientExists = await _applicationDbcontext.Patients.Select(x => new PatientDto
            {
                Id = x.Id
            }).
            SingleAsync(x => x.Id == id);
            return patientExists;
        }

        public async Task<Patient> GetPatientByContactNumber(string contactNumber)
        {
            if (string.IsNullOrWhiteSpace(contactNumber) || contactNumber.Length < 10) return null;

            var query = _applicationDbcontext.Patients
                .Include(p => p.Contact)
                .Where(p => !string.IsNullOrWhiteSpace(p.Contact.PrimaryPhone) && (p.Contact.PrimaryPhone.Replace("+1", "") == contactNumber.Replace("+1", "")));

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Patient> GetPatientByPatientVendorRxID(string patientVendorRxID)
        {

            var patientExists = await _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(c => c.Contact).
            Include(n=>n.Note).
            FirstOrDefaultAsync(x => x.PatientVendorRxID == patientVendorRxID);
            return patientExists;
        }


        public async Task<Patient> GetLastRefillDateWithPatientId(string patientVendorRxID)
        {
            var patientExists = await _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(c => c.Contact).
            Include(n => n.Note).
            FirstOrDefaultAsync(x => x.PatientVendorRxID == patientVendorRxID);
            return patientExists;
        }

        public async Task<Patient> GetPatientWithNoteById(int id)
        {

            var patientExists = await _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(c => c.Contact).
            Include(n => n.Note).
            SingleAsync(x => x.Id == id);
            return patientExists;
        }

        public Patient GetPatientByEmailId(string emailId)
        {

            var patientExists = _applicationDbcontext.Patients.Include(x => x.Address).
            Include(a => a.Address).
            Include(p => p.Pharmacy).
            Include(c => c.Contact).
            Include(n => n.Note).
            Where(x => x.Contact.PrimaryEmail == emailId).SingleOrDefault();
            return patientExists;
        }

        public async Task<List<Patient>> GetPatients(int recordNumber, int pageLimit,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);

            List<Patient> patients = null;
            bool condition = false;
            if (sortDirection == "asc")
            {
                 condition = true;
            }
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                   patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Where(p => !p.Patient.IsDeleted && (p.Patient.Status.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).Select(x => x.Patient).ToListAsync();
                        break;

                    case "Organization":
                        patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Where(p => !p.Patient.IsDeleted && (p.Pharmacy.Name.ToLower().ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).Select(x => x.Patient).ToListAsync();
                        break;
                    case "State":
                        patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Where(p => !p.Patient.IsDeleted && (p.Patient.Address.State.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).Select(x=>x.Patient).ToListAsync();
                        break;

                    case "PDC Category and Average":
                        patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Where(p => !p.Patient.IsDeleted).Skip(recordNumber).Take(pageLimit).Select(x=>x.Patient).ToListAsync();
                        break;
                }
                return patients;
            }
            else
            {
                        patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Where(p => !p.Patient.IsDeleted && (keywords == null || keywords == string.Empty ||
                      ((p.Patient.Contact.FirstName + " " + p.Patient.Contact.LastName).ToLower().Contains(keywords.ToLower())) || (isDateOfBirthValid && p.Patient.Contact.DoB.Date == searchDateOfBirth.Date))).OrderBy(p => condition ? p.Patient.Contact.FirstName : null)
                        .ThenByDescending(p => condition ? null : p.Patient.Contact.FirstName).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                        .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).Select(x=>x.Patient).ToListAsync();

                return patients;
            }
           

        }
        public async Task<List<Medication>> GetMedicationByPatientId(int patientId)
        {


            var medications = await _applicationDbcontext.Medications.
            Where(p => p.Patient.Id == patientId).ToListAsync();

            return medications;
        }
        public async Task<List<Medication>> GetMedicationByCondition(string condtion)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(m => m.Condition == condtion).
            Include(m => m.Patient).
            Include(m => m.Patient.Address).
            Include(m => m.Patient.Contact).
            ToListAsync();

            return medications;
        }
        public async Task<Medication> GetMedicationByVendorRxID(string vendorRxID)
        {
            var medications = await _applicationDbcontext.Medications.
            Where(p => p.RxVendorRxID == vendorRxID).FirstOrDefaultAsync();

            return medications;


        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            var result = await _applicationDbcontext.Patients.AddAsync(patient);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<Medication> AddMedication(Medication medication)
        {
            var result = await _applicationDbcontext.Medications.AddAsync(medication);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<Medication> UpdateMedication(Medication medication)
        {
            await _applicationDbcontext.SaveChangesAsync();
            return medication;
        }
        public async Task<Address> GetAddressById(int patientId)
        {
            var addressExists = await _applicationDbcontext.Address.FindAsync(patientId);
            return addressExists;
        }
        public async Task<Contact> GetContactById(int contactId)
        {
            var contactExists = await _applicationDbcontext.Contacts.FindAsync(contactId);
            return contactExists;
        }

        public async Task DeleteContactById(int contactId)
        {
            var contactExists = await _applicationDbcontext.Contacts.FindAsync(contactId);
            _applicationDbcontext.Remove(contactExists);
        }

        public async Task DeleteAddressById(int addressId)
        {
            var addressExists = await _applicationDbcontext.Address.FindAsync(addressId);
            _applicationDbcontext.Remove(addressExists);
        }
        public async Task DeleteMailListById(int patientId)
        {
            List<PatientMailList> mailList = await _applicationDbcontext.PatientMailLists.Where(x => x.PatientId == patientId).ToListAsync();
            if(mailList.Count() != 0)
            {
                List<Address> address = await _applicationDbcontext.PatientMailLists.Where(x => x.PatientId == patientId).Select(x => x.Address).ToListAsync();
                if (address.Count() != 0)
                {
                    _applicationDbcontext.Address.RemoveRange(address);
                }
                _applicationDbcontext.PatientMailLists.RemoveRange(mailList);
            }

        }
        public async Task DeleteMedicationConsumptionsById(int patientId)
        {
            var medicationConsumptions = await _applicationDbcontext.medicationConsumptions.FindAsync(patientId);
            if(medicationConsumptions != null)
            {
                _applicationDbcontext.Remove(medicationConsumptions);
            }
        }

        public async Task DeleteNotesById(int noteId)
        {
            var noteExists = await _applicationDbcontext.Notes.FindAsync(noteId);
            _applicationDbcontext.Remove(noteExists);
        }
        
        public  Patient UpdatePatientWebhook(Patient patient)
        {
            var result =  _applicationDbcontext.Patients.Update(patient);
       
            return result.Entity;
        }
        public async Task<int> UpdatePatient(Patient patient)
        {
            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;
        }


        
        public async Task DeletePatient(Patient patient)
        {
            _applicationDbcontext.Patients.Remove(patient);

             await _applicationDbcontext.SaveChangesAsync();
 
        }

        public void DeletePatientHardCoreForPharmacy(Patient patient)
        {
            _applicationDbcontext.Patients.Remove(patient);

        }
        public async Task<Contact> GetDoctorContact(Doctor doctor)
        {
            return await _applicationDbcontext.Doctors.Include(p => p.Contact).Where(d => d.Id == doctor.Id).Select(d => d.Contact).FirstOrDefaultAsync();

             
        }
        public async Task<List<Patient>> GetPatientsByPharmacyId(int pharmacyId)
        {
            var patients = await _applicationDbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId).
            Include(a => a.Address).
            Include(c => c.Contact).
            Include(n => n.Note).
            Include(m => m.Medications).
     
            ToListAsync();

            return patients;
        }



        public async Task<int> UpdatePatientNote(Note note)
        {
            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;
        }



        public async Task<Medication> GetMedicationById(int medicationId)
        {
            var medicationExists = await _applicationDbcontext.Medications
            .Where(m => m.Id == medicationId).Include(m => m.Patient).SingleOrDefaultAsync();
            return medicationExists;
        }

        public async Task<List<Patient>> GetPharmacyPatientsByCondition(int recordNumber, int pageLimit, int pharmacyId, string condition, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<Patient> patients = null;
            bool conditionCheck = false;
            if (sortDirection == "asc")
            {
                conditionCheck = true;
            }
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                        patients = await _applicationDbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Status.ToLower().Contains(filterValue.ToLower())).
                        Include(a => a.Address).
                        Include(p => p.Pharmacy).
                        Include(c => c.Contact).
                        Include(m => m.Medications).Skip(recordNumber).Take(pageLimit).AsNoTracking()
                        .ToListAsync();
                        break;

                    case "Organization":
                        patients = await _applicationDbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Pharmacy.Name.ToLower().Contains(filterValue.ToLower())).
                       Include(a => a.Address).
                       Include(p => p.Pharmacy).
                       Include(c => c.Contact).
                       Include(m => m.Medications).Skip(recordNumber).Take(pageLimit).AsNoTracking()
                        .ToListAsync();
                        break;
                    case "State":
                        patients = await _applicationDbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Address.State.ToLower().Contains(filterValue.ToLower())).
                        Include(a => a.Address).
                        Include(p => p.Pharmacy).
                        Include(c => c.Contact).
                        Include(m => m.Medications).Skip(recordNumber).Take(pageLimit).AsNoTracking()
                         .ToListAsync();
                        break;

                    case "PDC Category and Average":
                        patients = await _applicationDbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted).
                       Include(a => a.Address).
                       Include(p => p.Pharmacy).
                       Include(c => c.Contact).
                       Include(m => m.Medications).Skip(recordNumber).Take(pageLimit).AsNoTracking()
                        .ToListAsync();
                        break;
                }
                return patients;
            }

            else
            {
                if (condition == "Cholesterol")
                {
                    patients = _applicationDbcontext.Patients
                     .Include(a => a.Address)
                     .Include(p => p.Pharmacy).AsNoTracking()
                     .Include(c => c.Contact)
                     .Include(m => m.Medications).AsEnumerable()
                     .Where(p => p.Pharmacy.Id == pharmacyId &&
                      p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.CholesterolRefillDue && (string.IsNullOrEmpty(keywords) ||
                      p.Contact.FirstName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      p.Contact.LastName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date)))
                      .OrderBy(p => conditionCheck ? p.Contact.FirstName : null)
                      .ThenByDescending(p => conditionCheck ? null : p.Contact.FirstName)
                      .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                      .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                      .ToList();


                }
                if (condition == "Diabetes")
                {
                    patients = _applicationDbcontext.Patients
                       .Include(a => a.Address)
                       .Include(p => p.Pharmacy).AsNoTracking()
                       .Include(c => c.Contact)
                       .Include(m => m.Medications).AsEnumerable()
                       .Where(p => p.Pharmacy.Id == pharmacyId &&
                        p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.DiabetesRefillDue && (string.IsNullOrEmpty(keywords) ||
                        p.Contact.FirstName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                        p.Contact.LastName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                        (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date)))
                        .OrderBy(p => conditionCheck ? p.Contact.FirstName : null)
                        .ThenByDescending(p => conditionCheck ? null : p.Contact.FirstName)
                        .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                        .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                        .ToList();


                }
                if (condition == "RASA")
                {
                    patients = _applicationDbcontext.Patients
                     .Include(a => a.Address)
                     .Include(p => p.Pharmacy).AsNoTracking()
                     .Include(c => c.Contact)
                     .Include(m => m.Medications).AsEnumerable()
                     .Where(p => p.Pharmacy.Id == pharmacyId &&
                      p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.RasaRefillDue && (string.IsNullOrEmpty(keywords) ||
                      p.Contact.FirstName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      p.Contact.LastName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date)))
                      .OrderBy(p => conditionCheck ? p.Contact.FirstName : null)
                      .ThenByDescending(p => conditionCheck ? null : p.Contact.FirstName)
                      .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                      .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                      .ToList();


                }
                return patients;
            }
     
        }

        public async Task<List<Patient>> GetAllPharmacyPatientsByCondition(int recordNumber, int pageLimit, int pharmacyId, string condition, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<Patient> patients = null;
            patients = patients = await _applicationDbcontext.Patients
                    .Include(a => a.Address)
                    .Include(p => p.Pharmacy).AsNoTracking()
                    .Include(c => c.Contact)
                    .Include(m => m.Medications)
                    //.AsEnumerable()
                    .Where(p => p.Pharmacy.Id == pharmacyId &&
                     p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted)
                     .ToListAsync();

            return patients;

        }
        public int GetContributingPatients(int pharmacyId, string condition)
        {
            int patients_count = 0; 

            if (condition == "Cholesterol")
            {
                patients_count =  _applicationDbcontext.Patients
                    .Where(p => p.Pharmacy.Id == pharmacyId &&
                    p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.CholesterolRefillDue)
                    .Count();
            }
            else if (condition == "Diabetes")
            {
                patients_count =  _applicationDbcontext.Patients
                    .Where(p => p.Pharmacy.Id == pharmacyId &&
                    p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.DiabetesRefillDue)
                    .Count();
            }
            else if (condition == "RASA")
            {
                patients_count =  _applicationDbcontext.Patients
                    .Where(p => p.Pharmacy.Id == pharmacyId &&
                    p.Medications.Any(med => med.Condition == condition && med.IsActive && med.RefillDue && Convert.ToInt32(med.RefillsRemaining) > 0) && !p.IsDeleted && p.RasaRefillDue)
                    .Count();
            }

            return patients_count; 
        }


        public async Task<List<Patient>> GetPatientsForAssignedCount(MedicationDto medication, string userId)
        {
            IQueryable<Patient> query = null;

            if (!string.IsNullOrWhiteSpace(medication.GenericName)) 
            {
                if (string.IsNullOrWhiteSpace(userId)) // userId will be passed as null for SuperAdmin
                {
                    query = _applicationDbcontext.Patients
                        .Where(m => m.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !m.IsDeleted);
                }
                else
                {
                    query = _applicationDbcontext.Patients
                        .Join(_applicationDbcontext.PharmacyUsers.Where(p => p.UserId == userId), p => p.Pharmacy.Id, pu => pu.PharmacyId, (p, pu) => p)
                        .Where(m => m.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !m.IsDeleted);
                }

                var patients = await query.ToListAsync();

                return patients;
            }
            if (!string.IsNullOrWhiteSpace(medication.SbdcName))
            {
                if (string.IsNullOrWhiteSpace(userId)) // userId will be passed as null for SuperAdmin
                {
                    query = _applicationDbcontext.Patients
                        .Where(m => m.Medications.Any(Medication => Medication.SBDCName == medication.SbdcName) && !m.IsDeleted);
                }
                else
                {
                    query = _applicationDbcontext.Patients
                        .Join(_applicationDbcontext.PharmacyUsers.Where(p => p.UserId == userId), p => p.Pharmacy.Id, pu => pu.PharmacyId, (p, pu) => p)
                        .Where(m => m.Medications.Any(Medication => Medication.SBDCName == medication.SbdcName) && !m.IsDeleted);
                }

                var patients = await query.ToListAsync();

                return patients;
            }
            return null;
        }

        public async Task<List<Patient>> GetPatientsForAssignedCountForMedication(string genericName)
        {
            var patients = await _applicationDbcontext.Patients.
                Where(m => m.Medications.Any(Medication => Medication.GenericName == genericName) && !m.IsDeleted)
                .ToListAsync();

            return patients;
        }

        public  List<Patient> GetNonAdherencePatientList(List<PatientPdcDto> patientPdcDtos, int recordNumber, int pageLimit, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            var patientIds = patientPdcDtos.Select(dto => dto.PatientId).ToList();
            bool condition = false;
            List<Patient> patients = null;
            if (sortDirection == "asc")
            {
                condition = true;
            }
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                         patients = _applicationDbcontext.Patients.Include(p => p.Contact).Include(p => p.Address)
                                        .Include(p => p.Pharmacy).AsNoTracking().AsEnumerable()
                                        .Where(p => patientIds.Contains(p.Id) && p.Status.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).Skip(recordNumber).Take(pageLimit).ToList();
                    break;

                    case "Organization":
                        patients = _applicationDbcontext.Patients.Include(p => p.Contact).Include(p => p.Address)
                                      .Include(p => p.Pharmacy).AsNoTracking().AsEnumerable()
                                      .Where(p => patientIds.Contains(p.Id) && p.Pharmacy.Name.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).Skip(recordNumber).Take(pageLimit).ToList();            
                        break;
                    
                    case "State":
                         patients = _applicationDbcontext.Patients.Include(p => p.Contact).Include(p => p.Address)
                                         .Include(p => p.Pharmacy).AsNoTracking().AsEnumerable()
                                         .Where(p => patientIds.Contains(p.Id) && p.Address.State.Contains(filterValue, StringComparison.OrdinalIgnoreCase)).Skip(recordNumber).Take(pageLimit).ToList();
                        break;
                }
                return patients;
            }
                    DateTime searchDateOfBirth;
                    bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);
               patients = _applicationDbcontext.Patients.Include(p=>p.Contact).Include(p=>p.Address)
                          .Include(p=>p.Pharmacy).AsNoTracking().AsEnumerable()
                          .Where(p => patientIds.Contains(p.Id) && (string.IsNullOrEmpty(keywords) ||
                          p.Contact.FirstName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                          p.Contact.LastName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                          (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date))).OrderBy(p => condition ? p.Contact.FirstName : null)
                          .ThenByDescending(p => condition ? null : p.Contact.FirstName)
                          .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                          .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                          .ToList();
                        return patients;
        }

        public async Task<List<Patient>> GetPatientsByUserId( string userId)
        {
            var patients= await _applicationDbcontext.Patients.Where(p => p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId) && !p.IsDeleted)
            .Select(x=>new Patient
            (
                x.Id,
                x.Status,
                x.Address,
                x.Contact,
                x.Pharmacy.Name,
                x.Pharmacy.NpiNumber
                )).ToListAsync();

            return patients;
        }

        public async Task<List<Patient>> GetPatientsByUserIdWithPagination(int recordNumber, int pageLimit, string userId,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<Patient> patients = null;
            bool condition = false;
            if (sortDirection == "asc")
            {
                condition = true;
            }
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                       patients = await _applicationDbcontext.Patients.Include(p=>p.Address).Include(p => p.Contact).Include(p=>p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Join(_applicationDbcontext.PharmacyUsers
                       .Where(pu => pu.UserId == userId),
                        patient => patient.Pharmacy.Id,
                        pharmacyUser => pharmacyUser.PharmacyId,
                        (patient, pharmacyUser) => patient.Patient)
                        .Where(p => !p.IsDeleted && 
                         (p.Status.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                    case "Organization":
                        patients =await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Join(_applicationDbcontext.PharmacyUsers
                       .Where(pu => pu.UserId == userId),
                        patient => patient.Pharmacy.Id,
                        pharmacyUser => pharmacyUser.PharmacyId,
                        (patient, pharmacyUser) => patient.Patient)
                        .Where(p => !p.IsDeleted && 
                         (p.Pharmacy.Name.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;
                    case "State":
                        patients =await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Join(_applicationDbcontext.PharmacyUsers
                       .Where(pu => pu.UserId == userId),
                        patient => patient.Pharmacy.Id,
                        pharmacyUser => pharmacyUser.PharmacyId,
                        (patient, pharmacyUser) => patient.Patient)
                        .Where(p => !p.IsDeleted && 
                         (p.Address.State.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                    case "PDC Category and Average":
                        patients = await _applicationDbcontext.Patients.Include(p => p.Address).Include(p => p.Contact).Include(p => p.Pharmacy).AsNoTracking()
                        .Select(x => new
                        {
                            Patient = x,
                            Pharmacy = x.Pharmacy
                        }).Join(_applicationDbcontext.PharmacyUsers
                       .Where(pu => pu.UserId == userId),
                        patient => patient.Pharmacy.Id,
                        pharmacyUser => pharmacyUser.PharmacyId,
                        (patient, pharmacyUser) => patient.Patient)
                        .Where(p => !p.IsDeleted).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;
                }
                return patients;
            }
            else
            {
                patients =await _applicationDbcontext.Patients.Join(_applicationDbcontext.PharmacyUsers
               .Where(pu => pu.UserId == userId),
                patient => patient.Pharmacy.Id,
                pharmacyUser => pharmacyUser.PharmacyId,
                (patient, pharmacyUser) => patient)
                .Include(p => p.Pharmacy).AsNoTracking()
                .Include(p => p.Contact)
                .Include(p => p.Address)
                .Where(p => !p.IsDeleted && (keywords == null || keywords == string.Empty ||
                 (p.Contact.FirstName + " " + p.Contact.LastName).ToLower().Contains(keywords.ToLower()) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date))).OrderBy(p => condition ? p.Contact.FirstName : null)
                .ThenByDescending(p => condition ? null : p.Contact.FirstName).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToListAsync(); 
               
                return patients;
            }
           
        }


        public async Task<List<Patient>> GetPatientsByUserIdForEmployee(string userId)
        {
            var patients = await _applicationDbcontext.Patients
            .Include(p => p.Address)
                .Include(p => p.Contact).
                Include(p => p.Pharmacy)
                .Include(p => p.Medications)
                .Where(p => p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId) && !p.IsDeleted)
                .ToListAsync();
            return patients;
        }


        public async Task<List<Patient>> GetPatientsByUserIdForEmployeeForPDC(string userId)
        {
            var patients = await _applicationDbcontext.Patients
                .Include(p => p.Medications)
                .Where(p => p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId) && !p.IsDeleted)
                .ToListAsync();
            return patients;
        }

        public async Task<bool> IsValidPatientForUser(string userId, int patientId)
        {
            return await _applicationDbcontext.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.Pharmacy.PharmacyUsers.Any(u => u.UserId == userId)) != null;
        }


        public async Task<int> GetPatientCountByStatusAsync(string userId, string patientStatus)
        {
            var query = _applicationDbcontext.Patients
                .Where(p => !p.IsDeleted && p.Status == patientStatus);

            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(p => p.Pharmacy.PharmacyUsers.Any(p => p.UserId == userId));
            }

            return await query.CountAsync();
        }


        public List<Patient> GetPatientListByStatusAsync(string userId, string patientStatus,int recordNumber, int pageLimit,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)

        {
            bool condition = false;
            
            if (sortDirection == "asc")
            {
                condition = true;
            }
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);

            var query = _applicationDbcontext.Patients.Include(p=>p.Pharmacy).AsNoTracking().Include(p => p.Contact).Include(p => p.Address)
                .Where(p => !p.IsDeleted && p.Status == patientStatus).ToList();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                query = query.Where(p => _applicationDbcontext.PharmacyUsers
                                            .Include(p=>p.Pharmacy).AsNoTracking().Where(p => p.UserId == userId)
                                            .Select(pu => pu.PharmacyId).Contains(p.Pharmacy.Id)).ToList();
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                        query = query.AsEnumerable().Where(p => p.Status.ToLower().Contains(filterValue.ToLower())).Skip(recordNumber).Take(pageLimit).ToList();
                        break;

                    case "Organization":
                        query = query.AsEnumerable().Where(p => p.Pharmacy.Name.ToLower().Contains(filterValue.ToLower())).Skip(recordNumber).Take(pageLimit).ToList();
                        break;
                    case "State":
                        query = query.AsEnumerable().Where(p => p.Address.State.ToLower().Contains(filterValue.ToLower())).Skip(recordNumber).Take(pageLimit).ToList();
                        break;

                    case "PDC Category and Average":
                        query = query.Skip(recordNumber).Take(pageLimit).ToList();
                        break;
                }
                return query;
            }
            else
            {
                query = query.AsEnumerable().Where(p => !p.IsDeleted && (keywords == null || keywords == string.Empty ||
                      (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date))).ToList();

                return query.OrderBy(p => condition ? p.Contact.FirstName : null)
                     .ThenByDescending(p => condition ? null : p.Contact.FirstName).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                    .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).ToList();
            }
           
        }

        public async Task<bool> UpdatePatientConsentAsync(int patientId, string consentType, bool allow)
        {
            var patient = await _applicationDbcontext.Patients
                .Include(p => p.Contact)
                .Where(p => p.Id == patientId)
                .FirstOrDefaultAsync();

            if (patient == null) return false;

            switch (consentType)
            {
                case "email":
                    patient.Contact.ConsentForEmail = allow;
                    break;

                case "call":
                    patient.Contact.ConsentForCall = allow;
                    break;

                case "text":
                    patient.Contact.ConsentForText = allow;
                    break;

                case "birthday-sms":
                    patient.Contact.ConsentForBirthdaySms = allow;
                    break;
            }

            var updateCount = await _applicationDbcontext.SaveChangesAsync();

            return updateCount > 0;
        }

        public async Task<List<Patient>> GetPatientListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers)
        {
            var query = _applicationDbcontext.Patients
                .AsNoTracking()
                .Include(p => p.Contact).Include(x=>x.Pharmacy)
                .Where(p => phoneNumbers.Any(n => n == p.Contact.PrimaryPhone));

            return await query.ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsDueForRefillAsync(string userId, int recordNumber, int pageLimit, string searchText, string sortDirection, string filterType, string filterValue)
        {
            var query = _applicationDbcontext.Patients
                .Include(p => p.Pharmacy).AsNoTracking()
                .Include(p => p.Contact)
                .Include(p => p.Address)
                .Where(p => 
                    p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId)
                    && (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                    && p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0)
                );

            ApplyFilter(ref query, searchText, sortDirection, filterType, filterValue);

            query = query
                .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue);

            return await query.ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsWithNoRefillAsync(string userId, int recordNumber, int pageLimit, string searchText, string sortDirection, string filterType, string filterValue)
        {
            var query = _applicationDbcontext.Patients
                .Include(p => p.Pharmacy).AsNoTracking()
                .Include(p => p.Contact)
                .Include(p => p.Address)
                .Where(p => p.Pharmacy.PharmacyUsers.Any(pharmacyUser => pharmacyUser.UserId == userId)
                    && (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                    && !p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0)
                );

            ApplyFilter(ref query, searchText, sortDirection, filterType, filterValue);

            query = query
                .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue);

            return await query.ToListAsync();
        }

        void ApplyFilter(ref IQueryable<Patient> query, string searchText, string sortDirection, string filterType, string filterValue)
        {
            bool sortDescending = "desc".Equals(sortDirection, StringComparison.OrdinalIgnoreCase);

            bool isDateOfBirthValid = DateTime.TryParseExact(searchText, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var searchDateOfBirth);

            if (!string.IsNullOrWhiteSpace(filterType) && !string.IsNullOrWhiteSpace(filterValue))
            {
                filterValue = filterValue.Trim();

                switch (filterType)
                {
                    case "Status":
                        query = query.Where(p => p.Status.Contains(filterValue));
                        break;

                    case "Organization":
                        query = query.Where(p => p.Pharmacy.Name.Contains(filterValue));
                        break;

                    case "State":
                        query = query.Where(p => p.Address.State.Contains(filterValue));
                        break;
                }
            }
            else if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.Trim();

                query = query.Where(p => p.Contact.FirstName.Contains(searchText) || p.Contact.LastName.Contains(searchText)
                    || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date));
            }

            if (!sortDescending)
            {
                query = query.OrderBy(p => p.Contact.FirstName)
                    .ThenBy(p => p.Contact.LastName);
            }
            else
            {
                query = query.OrderByDescending(p => p.Contact.FirstName)
                    .ThenByDescending(p => p.Contact.LastName);
            }
        }
        public int GetPatientDiseaseCountById(int id, DateTime startDate, DateTime endDate,int month, string condition)
        {
            if (condition == "Cholesterol" || condition == "Diabetes" || condition == "RASA")
            {
                var patientCount =  _applicationDbcontext.Patients
               .Where(p => p.Pharmacy.Id == id &&
                p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted)
               .Count();
                return patientCount;
            }
            return 0;
        }
        public int countNewPatient(int id)
        {
            int count = 0;
            count = _applicationDbcontext.Patients.Where(x=> x.Status.Equals("New Patient") && x.Pharmacy.Id == id).Count();
            return count;
        }

    }
}
