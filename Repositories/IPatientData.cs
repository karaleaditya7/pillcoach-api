using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Repositories
{
    public interface IPatientData
    {
        Task<List<Patient>> GetPatients(int recordNumber, int pageLimit,string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<Patient> AddPatient(Patient patient);
        Task<int> UpdatePatientNote(Note note);
        Task<int> UpdatePatient(Patient patient);
        Patient UpdatePatientWebhook(Patient patient);
        
        Task<Patient> GetPatientById(int id);
        Task<Patient> GetPatientByPatientVendorRxID(string patientVendorRxID);
        Task<Patient> GetPatientWithNoteById(int id);
        Task<List<Patient>> GetPatientsByPharmacyId(int pharmacyId);
        Task<List<Patient>> GetPharmacyPatientsByCondition(int recordNumber, int pageLimit, int pharmacyId, string condition,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<List<Patient>> GetAllPharmacyPatientsByCondition(int recordNumber, int pageLimit, int pharmacyId, string condition, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Address> GetAddressById(int patientId);
        Task<Contact> GetContactById(int contactId);
        Task DeleteAddressById(int addressId);
        Task DeleteNotesById(int noteId);
        Task DeleteContactById(int contactId);
        Task DeleteMailListById(int patientId);
        Task<List<Patient>> GetPatientsByUserId(string userId);
        Patient GetPatientByEmailId(string emailId);

        Task<List<Patient>> GetPatientsForAssignedCount(MedicationDto medication, string userId);
        Task<Contact> GetDoctorContact(Doctor doctor);
        Task<List<Patient>> GetPatientsByUserIdForEmployee(string userId);
     
        Task<List<Patient>> GetPatientsByUserIdForEmployeeForPDC(string userId);
        Task<PatientDto> GetPatientByIdForPDCWithDto(int id);
        Task<List<Patient>> GetPatientsForAssignedCountForMedication(string genericName);
        Task DeletePatient(Patient patient);
        void DeletePatientHardCoreForPharmacy(Patient patient);

        Task<Patient> GetPatientByContactNumber(string contactNumber);

        Task<bool> IsValidPatientForUser(string userId, int patientId);

        List<Patient> GetPatientListByStatusAsync(string userId, string patientStatus, int recordNumber, int pageLimit,string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<bool> UpdatePatientConsentAsync(int patientId, string consentType, bool allow);
        Task<List<Patient>> GetPatientsByUserIdWithPagination(int recordNumber, int pageLimit, string userId, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<List<Patient>> GetPatientListByPhoneNumbersAsync(IEnumerable<string> phoneNumbers);

        Task<int> GetPatientCountByStatusAsync(string userId, string patientStatus);

        List<Patient> GetNonAdherencePatientList(List<PatientPdcDto> patientPdcDtos, int recordNumber, int pageLimit, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Patient> GetPatientBypatientId(int id);

        Task<List<Patient>> GetPatientsDueForRefillAsync(string userId, int recordNumber, int pageLimit, string searchText, string sortDirection, string filterType, string filterValue);

        Task<List<Patient>> GetPatientsWithNoRefillAsync(string userId, int recordNumber, int pageLimit, string searchText, string sortDirection, string filterType, string filterValue);
        int GetContributingPatients(int pharmacyId, string condition);
        Task DeleteMedicationConsumptionsById(int patientId);
        public int GetPatientDiseaseCountById(int id, DateTime startDate, DateTime endDate, int month, string condition);
        int countNewPatient(int id);
    }
}
