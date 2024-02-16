using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IDoctorData
    {
        Task<Doctor> AddDoctor(Doctor doctor);
        Doctor UpdateDoctorWebhook(Doctor doctor);
        Task<Doctor> GetDoctorByNPIID(string npiId);
        Task<DoctorPharmacy> AddDoctorPharmacy(DoctorPharmacy doctorPharmacy);
        Task<DoctorPharmacy> GetDoctorPharmacyByDoctorIdAndPharmacyId(int doctorId,int pharmacyId);
        Task<Doctor> GetDoctorById(int doctorId);
        Task<Doctor> GetDoctorByName(string firstName, string lastName);
        Task<Doctor> AddNewDoctor(Doctor doctor);
        Doctor UpdateDoctorEntity(Doctor doctor);

        Task<Contact> UpdateContact(Contact contact);

        Task<List<Doctor>> GetDoctorListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers);
    }
}
