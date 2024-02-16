using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface ICmrMedicationService
    {
        Task<Response<CmrMedication>> AddCmrMedication(CmrMedicationModel model);
        Task<Response<CmrMedication>> GetUniqueCmrMedicationsByPatientId(int patientId);
        Task<Response<CmrMedication>> GetCmrMedicationById(int id);

        Task<Response<CmrMedication>> UpdateCmrMedication(CmrMedicationModel model);
        Task<Response<CmrMedication>> DeleteCmrMedicationById(int id);
        Task<Response<RxNavMedication>> SaveAllRxNavMedications();
        Task<Response<string>> SearchForMedication(string search);
        Task<Response<Doctor>> GetAllDoctors(int id);
        Task<Response<string>> GetAllConditionsforCmrMedication(int patientId);
        Task<string> GetSBDCNameForNDCNumber(string ndcNumber);
    }
}
