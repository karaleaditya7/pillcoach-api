using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface ICmrMedicationData
    {
        Task<CmrMedication> AddCmrMedication(CmrMedication cmrMedication);
        Task<List<CmrMedication>> GetUniqueCmrMedicationsByPatientId(int patientId);
        Task<CmrMedication> GetCmrMedicationsById(int id);

        Task DeleteCmrMedication(CmrMedication cmrMedication);


        Task<CmrMedication> UpdateCmrMedication(CmrMedication cmrMedication);
        Task<List<Doctor>> GetAllDoctors(int id);
        IQueryable<RxNavMedication> SearchForRxNavMedication(string text);
        void DeleteCmrMedicationForPatient(CmrMedication cmrMedication);

        bool ValidateCmrMedication(CmrMedication cmrMedication);

        void DeleteCmrMedicationForServiceTakeAwayInformation(CmrMedication cmrMedication);

        Task<List<string>> SearchForRxNavMedicationSBDC(string text);

        Task<List<string>> SearchForRxNavMedicationGPCK(string text);

        Task<List<string>> GetUniqueConditionForCMRByPatientId(int patientId);
    }
}
