using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace OntrackDb.Service
{
    public class OtcMedicationService : IOtcMedicationService
    {
        private readonly IDoctorData _doctorData;
        private readonly IDoctorMedicationData _doctorMedicationData;
        private readonly IPatientData _patientData;
        private readonly IMedicationData _medicationData;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly ApplicationDbContext _applicationDbcontext;
        public OtcMedicationService(IDoctorData doctorData, IDoctorMedicationData doctorMedicationData, IPatientData patientData, IMedicationData medicationData, IOtcMedicationData otcMedicationData, ApplicationDbContext applicationDbcontext)
        {

            _doctorData = doctorData;
            _patientData = patientData;
            _otcMedicationData = otcMedicationData;
            _applicationDbcontext = applicationDbcontext;
            _medicationData = medicationData;
            _doctorMedicationData = doctorMedicationData;
        }
        public async Task<Response<OtcMedication>> AddOtcMedication(OtcMedicationModel model)
        {
            Response<OtcMedication> response = new Response<OtcMedication>();
            response.Success = false;

            var doctor = await _doctorData.GetDoctorById(model.DoctorId);
            var patient = await _patientData.GetPatientById(model.PatientId);
            //   var otcCondition = await _otcMedicationData.GetOtcConditionById(model.OtcConditionId);

            OtcMedication otcMedication = new OtcMedication
            {
                DoctorPrescribed = doctor,
                Condition = model.Condition,
                Direction = model.Condition,
                SBDCName = model.SBDCName,
                Patient = patient
            };
            var result = await _otcMedicationData.AddOtcMedication(otcMedication);

            if (result == null)
            {
                response.Message = "Error while creating otcMedication";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = otcMedication;
            response.Message = "otcMedication created successfully!";
            return response;
        }

        public async Task<Response<OtcMedication>> GetOtcMedicationsByPatientId(int recordNumber, int pageLimit, int patientId)
        {
            Response<OtcMedication> response = new Response<OtcMedication>();
            var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientIdWithPagination(recordNumber, pageLimit,patientId);

            response.Success = true;
            response.Message = "otcMedication retrived successfully";
            response.DataList = otcMedications;
            return response;
        }

        public async Task<Response<OtcMedication>> UpdateOtcMedication(OtcMedicationModel model)
        {
            Response<OtcMedication> response = new Response<OtcMedication>();


            OtcMedication otcMedication = await _otcMedicationData.GetOtcMedicationsById(model.Id);
           
            if (otcMedication == null)
            {
                response.Success = false;
                response.Message = "otcMedication not found";
                return response;
            }
            if (!string.IsNullOrEmpty(model.FirstName) || !string.IsNullOrEmpty(model.LastName))
            {
                var doctor = await _doctorData.GetDoctorByName(model.FirstName,model.LastName);
                if (doctor == null)
                {
                    var contact = new Contact { FirstName = model.FirstName,LastName =model.LastName };

                    var result1 = await _doctorData.AddNewDoctor(new Doctor { Contact = contact });
                    otcMedication.DoctorPrescribed = result1;

                }
                else
                {
                    otcMedication.DoctorPrescribed = doctor;
                }
            }

            otcMedication.Condition = model.Condition;
            otcMedication.Direction = model.Direction;
            otcMedication.SBDCName = model.SBDCName;
           
            var result = await _otcMedicationData.UpdateOtcMedication(otcMedication);

            response.Success = true;
            response.Message = "OtcMedication Updated successfully!";
            response.Data = result;
            return response;
        }

        public async Task<Response<Doctor>> GetAlldoctorsforOtcMedication(int patientId)
        {

            Response<Doctor> response = new Response<Doctor>();
            var doctors = await _doctorMedicationData.GetAlldoctorsforMedication(patientId);
            var otcDoctors = await _doctorMedicationData.GetAlldoctorsforOtc(patientId);

            doctors.AddRange(otcDoctors);

            doctors = doctors.Distinct().ToList();


            if (doctors == null)
            {
                response.Success = false;
                response.Message = "doctors Not Found";
                return response;
            }

            response.Success = true;
            response.Message = "doctors retrived successfully";
            response.DataList = doctors;
            return response;
        }

        public async Task<Response<string>> GetAllConditionsforOtcMedication(int patientId)
        {
            Response<string> response = new Response<string>();
            List<string> medConditions =await _medicationData.GetUniqueConditionByPatientId(patientId);
            List<string> otcConditions = await _otcMedicationData.GetAllConditionsforOtcMedication(patientId);
            medConditions.AddRange(otcConditions);
            medConditions =  medConditions.Distinct().ToList();
            response.Success = true;
            response.Message = "Conditions Retrived Successfully!";
            response.DataList = new List<string>(medConditions);

            return response;
        }

        public async Task<Response<OtcMedication>> DeleteOtcMedicationById(int id)
        {
            Response<OtcMedication> response = new Response<OtcMedication>();
            OtcMedication otcMedication = await _otcMedicationData.GetOtcMedicationsById(id);
            if (otcMedication == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            otcMedication.IsDeleted = true;
            await _otcMedicationData.UpdateOtcMedication(otcMedication);

            response.Message = "OtcMedication Deleted Sucessfully";
            response.Success = true;
            return response;
        }
    }
}

