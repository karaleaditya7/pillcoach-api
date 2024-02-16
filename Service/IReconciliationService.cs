using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace OntrackDb.Service
{
    public interface IReconciliationService
    {

        Task<Response<MedicationReconciliation>> AddReconciliationMedication(MedicationRecocilationModel model);

        Task<Response<MedicationReconciliation>> GetReconciliationMedicationsByPatientId(int patientId);

        Task<Response<RxNavMedication>> SaveAllRxNavMedications();

        Task<Response<MedicationReconciliation>> GetRecMedicationById(int id);

        Task<Response<MedicationReconciliation>> DeleteReconciliationMedicationsById(int id);

        Response<string> SearchForMedication(string search);

        Task<Response<Doctor>> GetAllDoctors(int id);

        Task<Response<MedicationReconciliation>> UpdateReconciliationMedication(MedicationRecocilationModel model);

        Task<Response<string>> GetAllConditionsforMedRe(int patientId);
    }
   
}
