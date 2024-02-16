using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories

{
    public interface IOtcMedicationData
    {
        Task<OtcMedication> AddOtcMedication(OtcMedication otcMedication);

        Task<List<OtcMedication>> GetOtcMedicationsByPatientId( int patientId);

        Task<OtcMedication> GetOtcMedicationsById(int id);

        Task<OtcMedication> UpdateOtcMedication(OtcMedication otcMedication);
        

        Task<List<string>> GetAllConditionsforOtcMedication(int patientId);

        Task<List<string>> GetUniqueConditionForOTCByPatientId(int patientId);

        Task<List<OtcMedication>> GetAllOtcMedicationsByPatientId(int patientId);

        void PatientDeleteForOtcMedication(OtcMedication otcMedication);
        Task<List<OtcMedication>> GetOtcMedicationsByPatientIdWithPagination(int recordNumber, int pageLimit, int patientId);

    }
}
