using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Helper;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using static iTextSharp.text.pdf.PdfCopy;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Repositories
{
    public class PharmacyData : IPharmacyData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public PharmacyData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<Pharmacy> GetPharmacyById(int id)
        {

            var pharmacyExists = await _applicationDbcontext.Pharmacies.
                Include(x => x.Address).
            Include(c => c.Contact).
            FirstOrDefaultAsync(x => x.Id == id);
            return pharmacyExists;
        }

        public List<Pharmacy> GetPharmacyCSVData(List<string> pharmacyIds)
        {
            List<Pharmacy> pharmacyList = new List<Pharmacy>();
            pharmacyIds.ForEach(x =>
            {
                pharmacyList.Add( _applicationDbcontext.PharmacyUsers.Include(p => p.Pharmacy.Patients).Include(p => p.Pharmacy.Contact).Include(p => p.Pharmacy.Address).Where(p => p.Pharmacy.Id == Int32.Parse(x)).Select(pharmacyUser => pharmacyUser.Pharmacy
                ).FirstOrDefault());

            });

            return pharmacyList;

        }

        public  List<Pharmacy> GetPharmacyListByPharmacyIdsAndUserId(List<string> pharmacyIds)
        {
            List<Pharmacy> pharmacyList = new List<Pharmacy>();
            pharmacyIds.ForEach(x =>
            {
                pharmacyList.Add(_applicationDbcontext.PharmacyUsers.Include(p => p.Pharmacy).Include(p => p.Pharmacy.Patients).ThenInclude(p => p.Medications).Include(p => p.Pharmacy.Contact).Include(p => p.Pharmacy.Address).Where(p => p.Pharmacy.Id == Int32.Parse(x)).Select(pharmacyUser => pharmacyUser.Pharmacy
                ).FirstOrDefault());

            });

            return pharmacyList;

        }


        public async Task<Pharmacy> GetPharmacyByNcpdpNumber(string ncpdpNumber)
        {

            var pharmacyExists = await _applicationDbcontext.Pharmacies.
             Include(x => x.Address).
             Include(c => c.Contact).
             Include(n => n.Note).
            FirstOrDefaultAsync(x => x.NcpdpNumber == ncpdpNumber);
            return pharmacyExists;
        }
        public async Task<Pharmacy> GetPharmacyWithNoteById(int id)
        {
            var pharmacyExists = await _applicationDbcontext.Pharmacies.
            Include(x => x.Address).
            Include(c => c.Contact).
            Include(n => n.Note).
            Include(p => p.Patients).
            ThenInclude(x => x.Medications).
            SingleAsync(x => x.Id == id);
            return pharmacyExists;
        }

        public async Task<Pharmacy> GetPharmacyWithNoteByIdForPDC(int id)
        {
            var pharmacyExists = await _applicationDbcontext.Pharmacies.
            Include(p => p.Patients).
            ThenInclude(x => x.Medications).
            SingleAsync(x => x.Id == id);
            return pharmacyExists;
        }

        public async Task<PharmacyDto> GetPharmacyWithNoteByIdForPDCForDto(int id)
        {
            var pharmacyExists = await _applicationDbcontext.Pharmacies.Select(x => new PharmacyDto
            {
                Id = x.Id
            }).
            SingleAsync(x => x.Id == id);
            return pharmacyExists;
        }

        public async Task<List<Pharmacy>> GetPharmacies(int recordNumber, int pageLimit,string keywords,string sortDirection, string filterType,string filterValue,string filterCategory)
        {
            List<Pharmacy> pharmacies = null;
            bool condition = false;
            if (sortDirection == "asc")
            {
                condition = true;
            }
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                        case "Pharmacy Name":
                            pharmacies = await _applicationDbcontext.Pharmacies.Include(p => p.Address).
                            Include(p => p.Contact)
                           .Where(p => !p.IsDeleted && p.Name.ToLower().Contains(filterValue.ToLower())).Skip(recordNumber).Take(pageLimit).ToListAsync();

                        
                            break;

                        case "City":
                        pharmacies =await _applicationDbcontext.Pharmacies.Include(p => p.Address).
                               Include(p => p.Contact)
                                .Where(p => !p.IsDeleted  && (p.Address.City.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).ToListAsync();

                        
                            break;
                        case "State":
                                        pharmacies =await _applicationDbcontext.Pharmacies.Include(p => p.Address).
                                      Include(p => p.Contact)
                                      .Where(p => !p.IsDeleted && (p.Address.State.ToLower().Contains(filterValue.ToLower()))).Skip(recordNumber).Take(pageLimit).ToListAsync();

                    
                            break;

                        case "Due for Refills":
                         
                                       pharmacies = await _applicationDbcontext.Pharmacies.Include(p => p.Address).
                                       Include(p => p.Contact)
                                       .Where(p => !p.IsDeleted ).Skip(recordNumber).Take(pageLimit).ToListAsync();
                          
                            break;
                    case "New Patients":
                         pharmacies =await _applicationDbcontext.Pharmacies.Include(p => p.Address).Include(p => p.Contact)
                            .Where(p => !p.IsDeleted).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;
                    case "PDC Category and Average":
                        pharmacies = await _applicationDbcontext.Pharmacies.
                            Include(p => p.Address).
                               Include(p => p.Contact).Where(p => !p.IsDeleted).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                }
                return pharmacies;
            }
            else
            {
                pharmacies = await _applicationDbcontext.Pharmacies
               .Include(p => p.Address).
                Include(p => p.Contact).
               Where(p =>!p.IsDeleted && (keywords == null || keywords == string.Empty || p.Name.ToLower().Contains(keywords.ToLower())))
               .OrderBy(p => condition ? p.Name : null)
               .ThenByDescending(p => condition ? null : p.Name)
               .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).AsNoTracking()
               .ToListAsync();
                return pharmacies;
            }


        }


        public async Task<List<Pharmacy>> GetPharmaciesForRelatedPharmacyCount(MedicationDto medication, string userId)
        {
            IQueryable<Pharmacy> query = null;

            if (string.IsNullOrWhiteSpace(userId)) // userId will be passed as null for SuperAdmin
            {
                //query = _applicationDbcontext.Pharmacies
                //    .Where(pharmacy => pharmacy.Patients.Any(patient => patient.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !patient.IsDeleted ) && !pharmacy.IsDeleted);
                query = _applicationDbcontext.Pharmacies
                             .Join(_applicationDbcontext.Patients, p => p.Id, pt => pt.Pharmacy.Id, (p, pt) => new { p, pt })
                             .Join(_applicationDbcontext.Medications, pp => pp.pt.Id, m => m.Patient.Id, (pp, m) => new { pp.p, pp.pt, m })
                             .Where(joinedTables => joinedTables.m.GenericName == medication.GenericName && !joinedTables.pt.IsDeleted && !joinedTables.p.IsDeleted)
                             .Select(joinedTables => joinedTables.p)
                             .Distinct();
            }
            else
            {
                //query = _applicationDbcontext.Pharmacies
                //    .Join(_applicationDbcontext.PharmacyUsers.Where(p => p.UserId == userId), p => p.Id, pu => pu.PharmacyId, (p, pu) => p)
                //    .Where(pharmacy => pharmacy.Patients.Any(patient => patient.Medications.Any(Medication => Medication.GenericName == medication.GenericName) && !patient.IsDeleted) && !pharmacy.IsDeleted);
                query = _applicationDbcontext.Pharmacies
                            .Join(_applicationDbcontext.Patients, p => p.Id, pt => pt.Pharmacy.Id, (p, pt) => new { p, pt })
                            .Join(_applicationDbcontext.Medications, pp => pp.pt.Id, m => m.Patient.Id, (pp, m) => new { pp.p, pp.pt, m })
                            .Join(_applicationDbcontext.PharmacyUsers, pp => pp.pt.Pharmacy.Id, pu => pu.PharmacyId, (pp, pu) => new { pp.p, pp.pt, pp.m, pu })
                            .Where(joinedTables => joinedTables.m.GenericName == medication.GenericName && !joinedTables.pt.IsDeleted && !joinedTables.p.IsDeleted
                                && joinedTables.pu.UserId == userId)
                            .Select(joinedTables => joinedTables.p)
                            .Distinct();
            }

            var pharmacies = await query.ToListAsync();

            return pharmacies;
        }
        public async Task<Pharmacy> GetPharmacyWithContactByPharmacyId(int pharamcyId)
        {
            var result = await _applicationDbcontext.Pharmacies.Include(p => p.Contact).
                                   SingleAsync(x => x.Id == pharamcyId);


            return result;

        }

        public async Task<Pharmacy> GetPharmacyWithNoteByPharmacyId(int pharamcyId)
        {
            var result = await _applicationDbcontext.Pharmacies.Include(p => p.Note).
                                   SingleAsync(x => x.Id == pharamcyId);

            return result;
        }



        public async Task<Pharmacy> AddPharmacy(Pharmacy pharmacy)
        {
            var result = await _applicationDbcontext.Pharmacies.AddAsync(pharmacy);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Pharmacy> GetPharmacybyPatientById(int id)
        {
            var result = await _applicationDbcontext.Patients.
                //.Pharmacies.
               // Where(p => p.Patients.Any(Patient => Patient.Id == id))
               Where(p=>p.Id == id).Include(x=>x.Pharmacy)
                .SingleOrDefaultAsync();

            if (result != null)
            {
                return result.Pharmacy;
            }

            return null;
        }
        public async Task<List<Patient>> GetPatientByPharmacyId(int id)
        {
            var result = await _applicationDbcontext.Patients.
                Where(p => p.Pharmacy.Id == id).ToListAsync();
            return result;
        }
        public async Task<Address> GetAddressById(int addressId)
        {
            var addressExists = await _applicationDbcontext.Address.FindAsync(addressId);
            return addressExists;
        }
        public async Task<Contact> GetContactById(int contactId)
        {
            var contactExists = await _applicationDbcontext.Contacts.FindAsync(contactId);
            return contactExists;
        }


        public async Task<int> UpdatePharmacy(Pharmacy pharmacy)
        {

            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;

        }
        public Pharmacy UpdatePharmacyWebhook(Pharmacy pharmacy)
        {
            var result = _applicationDbcontext.Pharmacies.Update(pharmacy);
            return result.Entity;

        }

        public async Task<Pharmacy> DeletePharmacy(int pharmacyId)
        {
            var result = await _applicationDbcontext.Pharmacies
                .FirstOrDefaultAsync(p => p.Id == pharmacyId);

            return result;
        }

        public async Task DeletePharmacyHardCore(Pharmacy pharmacy)
        {
            _applicationDbcontext.Pharmacies
              .Remove(pharmacy);
            await _applicationDbcontext.SaveChangesAsync();


        }



        public async Task<Pharmacy> GetPharmacyByName(string Name)
        {

            var pharmacyExists = await _applicationDbcontext.Pharmacies.FirstOrDefaultAsync(a => a.Name == Name);
            return pharmacyExists;
        }

        public async Task<List<Pharmacy>> GetPharmaciesByUserId(string userId)
        {
           
            List<Pharmacy> pharmacies = await _applicationDbcontext.PharmacyUsers.
                                  Include(p => p.Pharmacy.Address).
                                  Include(x => x.Pharmacy.Contact).
                                  Where(x => x.UserId == userId && !x.Pharmacy.IsDeleted).
                                  Select(x => x.Pharmacy).ToListAsync();


            return pharmacies;
        }
        public  async Task<List<Pharmacy>> GetPharmaciesByUserIdWithPagination(int recordNumber, int pageLimit, string userId,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<Pharmacy> pharmacies = null;
            bool condition = false;
            if (sortDirection == "asc")
            {
                condition = true;
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Pharmacy Name":
                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                               Include(p => p.Pharmacy.Address).
                               Include(p => p.Pharmacy.Contact).AsNoTracking().
                               Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted && p.Pharmacy.Name.ToLower().Contains(filterValue.ToLower())).
                               Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                    case "City":
                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                            Include(p => p.Pharmacy.Address).
                            Include(p => p.Pharmacy.Contact).AsNoTracking().
                            Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted && (p.Pharmacy.Address.City.ToLower().Contains(filterValue.ToLower()))).
                            Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;
                    case "State":
                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                            Include(p => p.Pharmacy.Address).
                            Include(p => p.Pharmacy.Contact).AsNoTracking().
                            Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted && p.Pharmacy.Address.State.ToLower().Contains(filterValue.ToLower())).
                            Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;
                    case "Due for Refills":

                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                             Include(p => p.Pharmacy.Address).
                             Include(p => p.Pharmacy.Contact).AsNoTracking().
                             Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted).
                             Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();

                        break;
                    case "New Patients":
                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                              Include(p => p.Pharmacy.Address).
                              Include(p => p.Pharmacy.Contact).AsNoTracking().
                              Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted).
                              Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                    case "PDC Category and Average":
                        pharmacies = await _applicationDbcontext.PharmacyUsers.
                              Include(p => p.Pharmacy.Address).
                              Include(p => p.Pharmacy.Contact).AsNoTracking().
                              Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted).
                              Select(p => p.Pharmacy).Skip(recordNumber).Take(pageLimit).ToListAsync();
                        break;

                }
                return pharmacies;
            }
            else
            {
                pharmacies = await _applicationDbcontext.PharmacyUsers.
                                Include(p => p.Pharmacy.Address).
                                Include(p => p.Pharmacy.Contact).AsNoTracking().
                                 Where(p => p.UserId == userId && !p.Pharmacy.IsDeleted && (keywords == null || keywords == string.Empty || p.Pharmacy.Name.ToLower().Contains(keywords.ToLower()))).
                                Select(p => p.Pharmacy).OrderBy(p => condition ? p.Name : null)
                                .ThenByDescending(p => condition ? null : p.Name).Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue).Distinct().ToListAsync();

                return pharmacies;
            }

           
        
        }

        public async Task<List<Doctor>> GetDoctorPharmacyByPharmacyId(int pharmacyId)
        {

            var result = await _applicationDbcontext.doctorPharmacy.Include(x => x.Doctor.Contact).Where(x => x.PharmacyId == pharmacyId).
                                  Select(x => x.Doctor).ToListAsync();
            return result;
        }

        public async Task<Pharmacy> GetPharmacyByNpiNumber(string npiNumber)
        {
            if (string.IsNullOrWhiteSpace(npiNumber)) return null;

            return await _applicationDbcontext.Pharmacies.FirstOrDefaultAsync(p => p.NpiNumber == npiNumber);
        }

        public async Task<List<Pharmacy>> GetPharmacyListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers)
        {
            var query = _applicationDbcontext.Pharmacies
                .AsNoTracking()
                .Include(p => p.Contact)
                .Where(p => phoneNumbers.Any(n => n == p.Contact.PrimaryPhone));

            return await query.ToListAsync();
        }

        public async Task<List<string>> GetAssignedTwilioNumbersAsync()
        {
            return await _applicationDbcontext.Pharmacies
                .Where(u => !u.IsDeleted && u.TwilioSmsNumber != null || u.TwilioSmsNumber != "")
                .Select(u => u.TwilioSmsNumber)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<Pharmacy>> GetAllPharmacyNames()
        {

                var pharmacies = await _applicationDbcontext.Pharmacies
               .Where(p => !p.IsDeleted)
               .ToListAsync();
                return pharmacies;

        }
        public async Task DeleteImportNotification(int pharmacyId)
        {
            List<string> requestIdList = await _applicationDbcontext.ImportSourceFiles.Where(x => x.PharmacyId == pharmacyId).Select(x => x.RequestId).ToListAsync();
            var notifications = _applicationDbcontext.AdminNotifications.AsEnumerable().Where(x => requestIdList.Any(r => x.NotificationType.Contains(r))).ToList();
            _applicationDbcontext.AdminNotifications.RemoveRange(notifications);
        }
    }
}
