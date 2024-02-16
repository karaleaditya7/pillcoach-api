using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class DoctorData:IDoctorData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public DoctorData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;    
        }

        public async Task<DoctorPharmacy> AddDoctorPharmacy(DoctorPharmacy doctorPharmacy)
        {
            var result = await _applicationDbcontext.doctorPharmacy.AddAsync(doctorPharmacy);

            return result.Entity;
        }
        public async Task<Doctor> AddDoctor(Doctor doctor)
        {
            var result = await _applicationDbcontext.Doctors.AddAsync(doctor);
            return result.Entity;
        }

        public async Task<Doctor> AddNewDoctor(Doctor doctor)
        {
            var result = await _applicationDbcontext.Doctors.AddAsync(doctor);
            return result.Entity;
        }

        public Doctor UpdateDoctorWebhook(Doctor doctor)
        {
            var result = _applicationDbcontext.Doctors.Update(doctor);
            return result.Entity;
        }

        public async Task<Contact> UpdateContact(Contact contact)
        {
            var result = _applicationDbcontext.Contacts.Update(contact);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public Doctor UpdateDoctorEntity(Doctor doctor)
        {
            var result = _applicationDbcontext.Doctors.Update(doctor);
            return result.Entity;
        }

        public async Task<Doctor> GetDoctorById(int doctorId)
        {
            var result = await _applicationDbcontext.Doctors.Include(d=>d.Contact).Where(d => d.Id == doctorId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<Doctor> GetDoctorByName(string firstName, string lastName)
        {
            var result = await _applicationDbcontext.Doctors.Include(d => d.Contact).Where(d => d.Contact.FirstName.Contains(firstName) && d.Contact.LastName.Contains(lastName)).FirstOrDefaultAsync();
            return result;
        }


        public async Task<Doctor> GetDoctorByNPIID(string npiId)
        {

            var doctorExists = await _applicationDbcontext.Doctors.
            Include(c => c.Contact).
            Include(a=>a.Pharmacies).
            FirstOrDefaultAsync(x => x.Npi == npiId);
            return doctorExists;
        }

        public async Task<DoctorPharmacy> GetDoctorPharmacyByDoctorIdAndPharmacyId(int doctorId, int pharmacyId)
        {
            var doctorExists = await _applicationDbcontext.doctorPharmacy.
                        FirstOrDefaultAsync(x => x.Doctor.Id == doctorId && x.Pharmacy.Id == pharmacyId);
            return doctorExists;

        }

        public async Task<List<Doctor>> GetDoctorListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers)
        {
            var query = _applicationDbcontext.Doctors
                .AsNoTracking()
                .Include(p => p.Contact)
                .Where(p => phoneNumbers.Any(n => n == p.Contact.PrimaryPhone));

            return await query.ToListAsync();
        }
    }
}
