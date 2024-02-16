using Microsoft.AspNetCore.Mvc.Formatters;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IMedicationService
    {
        Task<Double> CalculatePdc(int patientId, string condition, DateTime startDate, DateTime endDate, int month);
        Task<List<Patient>> CalculatePdcforPatients(List<Patient> patients, string condition, DateTime startDate, DateTime endDate, int month);
        Task<List<Medication>> getAllMedicationsByUserID(string userId);
        Task<int> countDueForRefill(List<Medication> medications);
        int countNoRefillRemaining(List<Medication> medications);

        Task<Response<Medication>> GetUniqueMedicationByDrugSubgroup();
        Task<Response<Patient>> GetPatientByUniqueMedication(int recordNumber, int pageLimit, int Month, DateTime startDate, DateTime endDate, string genericName,string sbdcName, string userId, string keywords, string sortDirection, string filterType, string filteValue, string filterCategory);
        Task<Response<Patient>> GetPatientsByUserId(string userId);

        Task<Response<Medication>> GetMedicationdetails(string rxNumber);

        Task<Response<Medication>> GetMedicationdetailsById(int id);

        Task<List<Patient>> CalculatePdcforPatientsGraph(List<Patient> patients, string condition, DateTime startDate, DateTime endDate);
        Task<Double> CalculatePdcForGraph(Patient patient, string condition, DateTime startDate, DateTime endDate);
        Task<Response<Medication>> GetUniqueMedicationByPatientId(int id);
        Task<Response<MedicationDto>> GetPatientPharmacyCountForMedication(string genericName, string sbdcName, string userRole, string userId);
        Task<Response<Medication>> GetUniqueNameMedicationWithGenericName(int recordNumber, int pageLimit, string userRole, string userId, string keywords, string sortDirection);

        Task<Response<MedicationHistory>> GetPatientMedicationHistoryByDrugNameAsync(int patientId, string drugName);

        Task<Response<MedicationUsageStatusUpdateResult>> SetMedicationUsageStatusAsync(int medicationId, bool status);
        Task<Response<bool>> SetMedicationRefillDueStatusAsync(int medicationId, bool status);

        Task<Response<MedicationCondition>> GetAllMedicationCondition();

        Task<Response<MedicationCondition>> SerachForMedicationCondition(string text);
        Task<string> GetExlusionMedication(int patientId, string condition);
    }
}
