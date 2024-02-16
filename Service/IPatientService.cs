using OntrackDb.Controllers;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace OntrackDb.Service
{
    public interface IPatientService
    {
        Task<Response<Patient>> GetPatients(int recordNumber, int pageLimit, DateTime startDate, DateTime endDate, int month, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Response<Patient>> AddPatient(Model.PatientModel model);

        Task<Response<Patient>> UpdatePatient(EditPatientModel patient);
        Task<Response<Patient>> DeletePatient(int patientId);
        Task<Response<Patient>> GetPatientById(int recordNumber, int pageLimit, int id, DateTime startDate, DateTime endDate, int month);
        Task<Response<Medication>> UpdateMedicationByCondition(int medicationId,MedicationModel model);
        Task<Response<Medication>> GetMedicationByCondition(string condition);
        Task<Response<Medication>> GetMedicationByPatientId(int patientId);
        Task<Response<List<Patient>>> GetPatientsByPharmacyId(int pharmacyId);
        Task<Response<Note>> AddPatientNote(NoteModel noteModel, int patientId);
        Task<Response<Note>> GetPatientNoteById(int patientId);
        Task<Response<Note>> UpdateNote(NoteModel model);
        Task<Response<Note>> UpdatePatientNote(NoteModel patientId);
        Task<Response<Patient>> UpdatePatientStatus(PatientModel patientModel);
        Task<List<Patient>> GetPharmacyPatientsByCondition(int recordNumber, int pageLimit, DateTime startDate, DateTime endDate, int pharmacyId, string condition,int month,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<List<Patient>> GetAllPharmacyPatientsByCondition(int recordNumber, int pageLimit, DateTime startDate, DateTime endDate, int pharmacyId, string condition,int month,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Response<Patient>> GetPatientsByUserId( string userId, DateTime startDate, DateTime endDate,int month);
        int countPatientByCondition(List<Patient> patients, string condition);

        int countNewPatient(List<Patient> patients);
        int countNewPatient(int id);
        int countInProgressPatient(List<Patient> patients);

        Task<Response<Patient>> GetPatientsByPatientStatus(string userId, DateTime startDate, DateTime endDate, string patientStatus,int month);
        Task<Response<Patient>> GetPatientsByDueforRefills(string userId, DateTime startDate, DateTime endDate,int month);
        Task<Response<Patient>> GetPatientsByNoRefillRemaining(string userId, DateTime startDate, DateTime endDate,int month);

        Task<Response<Patient>> GetPatientByIdGraph(int id, DateTime startDate, DateTime endDate);
        List<Patient> GetPatientsByCondition(List<Patient> patients, string condition);
        Task<Response<Patient>> GetPatientsByUserIdForEmployee(string userId, DateTime startDate, DateTime endDate, int month, bool calculatePDCs = true);
        Task<Response<PatientDto>> GetPatientByIdForPDCForDto(int id, DateTime startDate, DateTime endDate, int month);
        Task<Response<Patient>> GetPatientsByUserIdForEmployeeForPDC(string userId, DateTime startDate, DateTime endDate, int month);

        Task<Response<Patient>> DeletePatientForPharmacy(int patientId);
        Task<Response<Patient>> GetPatientByContactNumber(string contactNumber);
        Task<Response<Patient>> GetPatientListByStatusAsync(string userId, string patientStatus,int recordNumber, int pageLimit, int month,DateTime startDate,DateTime endDate,  string keywords,string sortDirection,string filterType, string filterValue, string filterCategory);
        Task<Response<Doctor>> UpdateDoctor(DoctorModel model);

        Task<bool> UpdatePatientConsentAsync(int patientId, string consentType, bool allow);
        Task<Response<Patient>> GetPatientsByUserIdWithPagination(int firstpage, int lastPage, string userId, DateTime startDate, DateTime endDate, int month, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory);

        Task<Response<Medication>> GetPrescribedMedicationsBypatientId(int recordNumber, int pageLimit, int id);

        Task<Response<Patient>> GetNonAdherencePatientsForUser(string userId, String condition, DateTime startDate, DateTime endDate, int month, int recordNumber, int pageLimit, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<PatientDiseaseCount> GetPatientDiseaseCountById(int id, DateTime startDate, DateTime endDate,int month, string condition);
    }
}
