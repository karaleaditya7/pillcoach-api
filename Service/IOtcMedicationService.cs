using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IOtcMedicationService
    {
        Task<Response<OtcMedication>> AddOtcMedication(OtcMedicationModel model);

        Task<Response<OtcMedication>> GetOtcMedicationsByPatientId(int recordNumber, int pageLimit, int patientId);

        Task<Response<OtcMedication>> UpdateOtcMedication(OtcMedicationModel model);

        Task<Response<Doctor>> GetAlldoctorsforOtcMedication(int patientId);

        Task<Response<string>> GetAllConditionsforOtcMedication(int patientId);

        Task<Response<OtcMedication>> DeleteOtcMedicationById(int id);

    }
}
