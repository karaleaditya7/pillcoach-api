using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class MedicationService : IMedicationService
    {
        private readonly IMedicationData _medicationData;
        private readonly IWebHookService _webHookService;
        private readonly IPharmacyData _pharmacyData;
        private readonly IPatientData _patientData;
        private readonly IMedicationConsumptionData _medicationConsumptionData;
        private IConfiguration _configuration;
        private readonly INdcApiService _ndcApiService;
        private readonly ApplicationDbContext _applicationDbcontext;
        readonly IPatientPdcService _patientPdcService;
        readonly ICmrMedicationService _cmrMedicationService;

        public MedicationService(IMedicationData medicationData, ICmrMedicationService cmrMedicationService, IPharmacyData pharmacyData, IWebHookService webHookService, IMedicationConsumptionData medicationConsumptionData, IConfiguration configuration, IPatientData patientData, INdcApiService ndcApiService, ApplicationDbContext applicationDbcontext, IPatientPdcService patientPdcService)
        {
            _medicationData = medicationData;
            _webHookService = webHookService;
            _medicationConsumptionData = medicationConsumptionData;
            _configuration = configuration;
            _pharmacyData = pharmacyData;
            _patientData = patientData;
            _ndcApiService = ndcApiService;
            _applicationDbcontext = applicationDbcontext;
            _patientPdcService = patientPdcService;
            _cmrMedicationService= cmrMedicationService;    
        }
        public async Task<Double> CalculatePdc(int patientId ,string condition, DateTime startDate, DateTime endDate,int month)
        {
            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }

            var pdc = await _patientPdcService.GetPdcForPatientAsync(patientId, condition, pdcMonth, month, queryType);

            return pdc.Value;
        }


        public async Task<Double> CalculatePdcForGraph(Patient patient, string condition, DateTime startDate, DateTime endDate)
        {

           
            

            var newLastRefillDates = new DateTime();
            var newStartDates = new DateTime();
            Double totalPDC = 0.00;

             newLastRefillDates =endDate;
                newStartDates = startDate;
           
            var medications = await _medicationData.GetMedicationByPatientId(patient.Id);
            for (int i = 0; i <= medications.Count - 1; i++)
            {

                var pdcMedication = await _medicationData.GetPdcMedicationWithNdcNumber(medications[i].NDCNumber);
                if (pdcMedication != null)
                {
                    if (pdcMedication.category == InclusionDisease.Diabetes.ToString() || pdcMedication.category == InclusionDisease.RASA.ToString()
                    || pdcMedication.category == InclusionDisease.Statins.ToString() && medications[i].Condition == InclusionDisease.Cholesterol.ToString())
                    {
                        medications[i].IsInclude = true;
                        await _medicationData.UpdateMedication(medications[i]);
                    }
                    else if (pdcMedication.category == ExclusionDisease.Insulins.ToString() && condition.Equals(InclusionDisease.Diabetes.ToString()))
                    {
                        medications[i].IsExclude = true;
                        await _medicationData.UpdateMedication(medications[i]);
                        return 0.00;
                    }
                }


            }
            var medicationListForPDC = await _medicationData.GetDistintMedicationsByConditionAndPatientIdForEarlyMedicationPDC(condition, patient.Id);
            if (medicationListForPDC.Count > 0)
            {
                
                var medicationConsumptions = await _medicationConsumptionData.GetMedicationConsumptionByPatientIDAndCondition(medicationListForPDC.First(), patient, newStartDates, newLastRefillDates, true);
                
                Double average = 0.00; int days = 0;
                TimeSpan difference = newLastRefillDates - newStartDates;
                days = difference.Days + 1;
                if (days - medicationConsumptions.Count >= 60)
                {
                    return 0.00;
                }
                average = ((Double)(medicationConsumptions.Count) / (Double)days) * 100;
                return medicationConsumptions.Count != 0 ? Math.Round(average, 2) : 0.00;
            }

           return medicationListForPDC.Count != 0 ? Math.Round(totalPDC, 2) : 0.00;

        }
        public async Task<List<Patient>> CalculatePdcforPatients(List<Patient>patients, string condition, DateTime startDate, DateTime endDate,int month)
        {
            for (int i = 0; i < patients.Count; i++)
            {
                if (condition.ToLower().Trim() == PDC.Cholesterol.ToString().ToLower())
                {
                    patients[i].CholestrolPDC = await this.CalculatePdc(patients[i].Id, condition, startDate, endDate, month);
                }
             
                if (condition.ToLower().Trim() == PDC.Diabetes.ToString().ToLower())
                {
                    patients[i].DiabetesPDC =await this.CalculatePdc(patients[i].Id, condition, startDate, endDate, month);
                }
                if (condition.ToLower().Trim() == PDC.RASA.ToString().ToLower())
                {
                    patients[i].RASAPDC = await this.CalculatePdc(patients[i].Id, condition, startDate, endDate, month);
                }

            }
            return patients;
             
           
        }


        public async Task<List<Patient>> CalculatePdcforPatientsGraph(List<Patient> patients, string condition, DateTime startDate, DateTime endDate)
        {
            for (int i = 0; i < patients.Count; i++)
            {
                if (condition.ToLower().Trim() == PDC.Cholesterol.ToString().ToLower())
                {
                    patients[i].CholestrolPDC = await this.CalculatePdcForGraph(patients[i], condition, startDate, endDate);
                }

                if (condition.ToLower().Trim() == PDC.Diabetes.ToString().ToLower())
                {
                    patients[i].DiabetesPDC = await this.CalculatePdcForGraph(patients[i], condition, startDate, endDate);
                }
                if (condition.ToLower().Trim() == PDC.RASA.ToString().ToLower())
                {
                    patients[i].RASAPDC = await this.CalculatePdcForGraph(patients[i], condition, startDate, endDate);
                }

            }
            return patients;
         

        }

        public async Task<List<Medication>> getAllMedicationsByUserID(string userId)
        {
            var medications = await _medicationData.getAllMedicationsByUserID(userId);
            return medications;
        }

        public async Task<int> countDueForRefill(List<Medication> medications)
        {
            //var latestMedications = medications
            //    .Where(m => m.IsActive)
            //    .GroupBy(m => new { m.Patient.Id, m.GenericName })
            //    .Select(m => m.OrderByDescending(m => m.LastFillDate).FirstOrDefault());

            var latestMedications = medications
                .Where(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0)
                .GroupBy(m => new { m.Patient.Id, m.GenericName })
                .Select(m => m.OrderByDescending(m => m.LastFillDate).FirstOrDefault());


            var count = latestMedications.Where(m => HasRefillDue(m))
                .DistinctBy(m => m.Patient.Id)
                .Count();

            return await Task.FromResult(count);
        }

        Func<Medication, bool> HasRefillDue = med =>
        {
            DateTime today = DateTime.UtcNow.Date;

            return (int.TryParse(med.RefillsRemaining, out var refillRemaining) && refillRemaining > 0)
                && med.IsActive
                && new string[] { "diabetes", "rasa", "cholesterol" }.Contains((med.Condition ?? string.Empty).ToLower())
                && med.NextFillDate.Subtract(today).TotalDays <= 5;
        };

        Func<Medication, bool> HasNoRefill = med =>
        {
            DateTime today = DateTime.UtcNow.Date;

            return (int.TryParse(med.RefillsRemaining, out var refillRemaining) && refillRemaining == 0)
                && med.IsActive
                && new string[] { "diabetes", "rasa", "cholesterol" }.Contains((med.Condition ?? string.Empty).ToLower());
        };

        public int countNoRefillRemaining(List<Medication> medications)
        {
            var latestMedications = medications
                .Where(m => m.IsActive)
                .GroupBy(m => new { m.Patient.Id, m.GenericName })
                .Select(m => m.OrderByDescending(m => m.LastFillDate).FirstOrDefault());

            var count = latestMedications.Where(m => HasNoRefill(m))
                .DistinctBy(m => m.Patient.Id)
                .Count();

            return count;
        }

        public async Task<Response<Medication>> GetUniqueMedicationByDrugSubgroup()
        {
            DateTime endDate = DateTime.Now;
            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, 6);
            Response<Medication> response = new Response<Medication>();
            var medications = await _medicationData.GetUniqueMedicationByDrugSubGroup();

            if (medications == null)
            {
                response.Success = false;
                response.Message = "Medication Not Found With That NDCNumber";
                return response;
            }


            response.Success = true;
            response.Message = "Medications retrived successfully";
            response.DataList = medications;
            return response;
        }

        public async Task<Response<MedicationCondition>> GetAllMedicationCondition()
        {

            Response<MedicationCondition> response = new Response<MedicationCondition>();
            var medicationConditions = await _medicationData.GetAllMedicationCondition();

            if (medicationConditions == null)
            {
                response.Success = false;
                response.Message = "MedicationConditions Not Found";
                return response;
            }


            response.Success = true;
            response.Message = "MedicationConditions retrived successfully";
            response.DataList = medicationConditions;
            return response;
        }
        public async Task<Response<MedicationCondition>> SerachForMedicationCondition(string text)
        {

            Response<MedicationCondition> response = new Response<MedicationCondition>();
            var medicationCondition = await _medicationData.GetMedicationConditionByName(text);

            if (medicationCondition == null)
            {
                MedicationCondition medicationCondition1 = new MedicationCondition();

                medicationCondition1.Name = text;

                var result = await _medicationData.AddMedicationCondition(medicationCondition1);
              
                response.Success = true;
                response.Message = "MedicationCondition retrived successfully";
                response.Data = result;
                return response;
            }
            else
            {

                response.Success = true;
                response.Message = "MedicationCondition retrived successfully";
                response.Data = medicationCondition;
                return response;
            }


        }



        public async Task<Response<Medication>> GetUniqueNameMedicationWithGenericName(int recordNumber, int pageLimit, string userRole, string userId, string keywords, string sortDirection)
        {
            Response<Medication> response = new Response<Medication>();
            var medications = await _medicationData.GetUniqueMedicationByDrugSubGroupForGeneric(recordNumber,pageLimit, Roles.SuperAdmin.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase) ? null : userId,keywords,sortDirection);
        
            if (medications == null)
            {
                response.Success = false;
                response.Message = "Medication Not Found With That NDCNumber";
                return response;
            }
 
            response.Success = true;
            response.Message = "Medications retrived successfully";
            response.DataList = medications;
            return response;
        }

        public async Task<Response<MedicationDto>> GetPatientPharmacyCountForMedication(string genericName,string sbdcName, string userRole, string userId)
        {
            Response<MedicationDto> response = new Response<MedicationDto>();
            var medication = await _medicationData.GetUniqueMedicationByDrugSubGroupDto(genericName,sbdcName);

            if (medication != null)
            {
                var patients = await _patientData.GetPatientsForAssignedCount(medication, Roles.SuperAdmin.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase) ? null : userId);
                medication.AssignedPatient = patients.Count;

                //var pharmacies = await _pharmacyData.GetPharmaciesForRelatedPharmacyCount(medication, Roles.SuperAdmin.ToString().Equals(userRole, StringComparison.OrdinalIgnoreCase) ? null : userId);
                //medication.RelatedPharmacies = pharmacies.Count;
            }

            response.Success = true;
            response.Message = "Medication Pharmacy and Patient Count retrived successfully";
            response.Data = medication;
            return response;
        }

        public async Task<Response<Patient>> GetPatientByUniqueMedication(int recordNumber, int pageLimit, int Month,DateTime startDate,DateTime endDate, string genericName, string sbdcName, string userId, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            Response<Patient> response = new Response<Patient>();
            var patients = await _medicationData.GetPatientByUniqueMedication(recordNumber, pageLimit,genericName,sbdcName, userId,keywords,sortDirection,filterType,filterValue,filterCategory);
            if (patients == null)
            {
                response.Success = false;
                response.Message = "Patient Not Found With That NDCNumber";
                return response;
            }
            foreach ( var patient in patients )
            {
                Double cholesterol = await this.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, Month);
                Double rasa = await this.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, Month);
                Double diabetes = await this.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, Month);


                patient.CholestrolPDC = cholesterol;
                patient.RASAPDC = rasa;
                patient.DiabetesPDC = diabetes;
            }
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "PDC Category and Average":
                        switch (filterCategory)
                        {
                            case "Cholesterol":
                                string[] filterArrayCholesterol = filterValue.Split("-");

                                double filterMinValue = Convert.ToDouble(filterArrayCholesterol[0].Split("%")[0]);
                                double filterMaxValue = Convert.ToDouble(filterArrayCholesterol[1].Split("%")[0]);
                                patients = patients.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                patients = patients.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC <= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                patients = patients.Where(p => p.CholestrolPDC >= filterMinValueD && p.CholestrolPDC <= filterMaxValueD).ToList();
                                break;
                        }
                        break;
                }
            }
            
       
            response.Success = true;
            response.Message = "Patient retrived successfully";
            response.DataList = patients;
            return response;
        }


        public async Task<Response<Patient>> GetPatientsByUserId(string userId)
        
        {
            Response<Patient> response = new Response<Patient>();
            var patients = await _medicationData.GetPatientsByUserId(userId);
            if (patients == null)
            {
                response.Message = " Patients not found";
                response.Success = false;
                return response;
            }
      

            response.Success = true;
            response.DataList = patients;
            response.Message = "patients retrieved successfully!";
            return response;
        }


        public async Task<Response<Medication>> GetMedicationdetails(string rxNumber)

        {
            Response<Medication> response = new Response<Medication>();
            var medications = await _medicationData.GetMedicationdetails(rxNumber);

            List<Medication> medicationList = new List<Medication>();
            foreach (Medication medication in medications)
            {
                List<Doctor> doctor = await _pharmacyData.GetDoctorPharmacyByPharmacyId(medication.Patient.Pharmacy.Id);
                medication.Doctor = doctor;
                medicationList.Add(medication);
            }

            if (medications == null)
            {
                response.Message = " Medication not found";
                response.Success = false;
                return response;
            }


            response.Success = true;
            response.DataList = medicationList;
            response.Message = "Medication retrieved successfully!";
            return response;
        }


        public async Task<Response<Medication>> GetMedicationdetailsById(int id)

        {
            Response<Medication> response = new Response<Medication>();
            var medication = await _medicationData.GetMedicationById(id);

            if (medication == null)
            {
                response.Message = " Medication not found";
                response.Success = false;
                return response;
            }

        

            List<Doctor> doctor = await _pharmacyData.GetDoctorPharmacyByPharmacyId(medication.Patient.Pharmacy.Id);
            medication.Doctor = doctor;

            response.Success = true;
            response.Data = medication;
            response.Message = "Medication retrieved successfully!";
            return response;
        }

           
        public async Task<Response<Medication>> GetUniqueMedicationByPatientId(int id)
        {

            Response<Medication> response = new Response<Medication>();
            var medications = await _medicationData.GetUniqueMedicationByPatientId(id);

             response.Success = true;
            response.Message = "Unique Medication with Patient Id retrived Successfully";
            response.DataList = medications;
            return response;
        }

        public async Task<Response<MedicationHistory>> GetPatientMedicationHistoryByDrugNameAsync(int patientId, string drugName)
        {
            var response = new Response<MedicationHistory> { DataList = new() };

            if (patientId > 0 && !string.IsNullOrWhiteSpace(drugName))
            {
                var history = await _medicationData.GetPatientMedicationHistoryByDrugNameAsync(patientId, drugName.Trim());

                if (history != null && history.Any())
                {
                    history.ForEach(m => m.NextFillDate = m.RefillRemaining > 0 ? m.NextFillDate : null);

                    response.DataList = history;
                }

                response.Success = true;
            }
            else
            {
                response.Message = "Invalid Patient Id or Rx Number";
            }

            return response;
        }

        public async Task<Response<MedicationUsageStatusUpdateResult>> SetMedicationUsageStatusAsync(int medicationId, bool status)
        {
            var response = new Response<MedicationUsageStatusUpdateResult>();

            response.Data = await _medicationData.SetMedicationUsageStatusAsync(medicationId, status);
            response.Success = response.Data != null;

            return response;
        }

        public async Task<Response<bool>> SetMedicationRefillDueStatusAsync(int medicationId, bool status)
        {
            var response = new Response<bool>();

            response.Success = await _medicationData.SetMedicationRefillDueStatusAsync(medicationId, status);

            return response;
        }
        public async Task<string> GetExlusionMedication(int patientId, string condition)
        {
            return await _medicationData.GetExlusionMedication(patientId, condition);
        }
    }
}
