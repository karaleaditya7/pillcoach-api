using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IDoctorMedicationData
    {
        Task<DoctorMedication> AddDoctorMedication(DoctorMedication doctorMedication);
        Task<List<DoctorMedication>> GetDoctorMedicationByCmrId(int cmrId);
        Task<List<DoctorMedication>> GetDoctorMedicationByMedRecId(int medRecId);

        Task<List<Doctor>> GetAlldoctorsforOtc(int patientId);
        Task<List<Doctor>> GetAlldoctorsforMedication(int patientId);

        Task<List<Doctor>> GetAlldoctorsforCmrMedication(int patientId);
        Task<List<Doctor>> GetAlldoctorsforMedRecMedication(int patientId);

        void DeleteDoctorMedicationForCmr(DoctorMedication doctorMedication);
        void DeleteDoctorMedicationForMedRec(DoctorMedication doctorMedication);
        Task<List<DoctorMedication>> GetDoctorMedicationByOtcMedId(int otcMedId);
        void DeleteDoctorMedicationForOtcMedication(DoctorMedication doctorMedication);
        Task<List<DoctorMedication>> GetDoctorMedicationByMedId(int medId);

        void DeleteDoctorMedicationForMedication(DoctorMedication doctorMedication);


    }
}
