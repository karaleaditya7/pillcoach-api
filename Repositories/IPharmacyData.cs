using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IPharmacyData
    {
        Task<List<Pharmacy>> GetPharmacies(int recordNumber, int pageLimit, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<Pharmacy> AddPharmacy(Pharmacy pharmacy);
        Task<int> UpdatePharmacy(Pharmacy pharmacy);
        Pharmacy UpdatePharmacyWebhook(Pharmacy pharmacy);
        Task<Pharmacy> DeletePharmacy(int pharmacyId);
        Task<Pharmacy> GetPharmacyById(int id);
        Task<Pharmacy> GetPharmacyByNcpdpNumber(string ncpdpNumber);
        Task<Pharmacy> GetPharmacyWithNoteById(int id);
        Task<Pharmacy> GetPharmacyByName(string Name);
        Task<Pharmacy> GetPharmacyWithContactByPharmacyId(int pharamcyId);
        Task<Pharmacy> GetPharmacyWithNoteByPharmacyId(int pharamcyId);
        Task<List<Pharmacy>> GetPharmaciesByUserId(string userId);

        Task<List<Doctor>> GetDoctorPharmacyByPharmacyId(int pharmacyId);
        Task<Address> GetAddressById(int addressId);
        Task<Contact> GetContactById(int contactId);
        List<Pharmacy> GetPharmacyCSVData(List<string> pharmacyIds);

        List<Pharmacy> GetPharmacyListByPharmacyIdsAndUserId(List<string> pharmacyIds);
        Task<List<Pharmacy>> GetPharmaciesForRelatedPharmacyCount(MedicationDto medication, string userId);
        Task<Pharmacy> GetPharmacybyPatientById(int id);
        Task<Pharmacy> GetPharmacyWithNoteByIdForPDC(int id);
        Task<PharmacyDto> GetPharmacyWithNoteByIdForPDCForDto(int id);

        Task DeletePharmacyHardCore(Pharmacy pharmacy);
        Task<List<Patient>> GetPatientByPharmacyId(int id);
        Task<Pharmacy> GetPharmacyByNpiNumber(string npiNumber);
        Task<List<Pharmacy>> GetPharmaciesByUserIdWithPagination(int recordNumber, int pageLimit, string userId,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<List<Pharmacy>> GetPharmacyListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers);

        Task<List<string>> GetAssignedTwilioNumbersAsync();
        Task<List<Pharmacy>> GetAllPharmacyNames();
        Task DeleteImportNotification(int pharmacyId);
    }
}
