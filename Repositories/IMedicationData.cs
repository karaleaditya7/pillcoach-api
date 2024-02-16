using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IMedicationData
    {
        Task<Medication> GetMedicationByVendorRxID(string vendorRxID);
        Task<List<Medication>> GetMedicationByPatientId(int patientId);
        Task<List<Medication>> GetMedicationByCondition(string condition);
        Task<Medication> GetMedicationById(int medicationId);
        Task<Medication> AddMedication(Medication medication);
        Task<Medication> UpdateMedication(Medication medication);
        Medication UpdateMedicationWebhook(Medication medication);
        Task<List<Medication>> getAllMedicationsByUserID(string userId);
        Task<List<Medication>> getAllMedicationsByPharmacyID(int pharmacyId);
        Task<List<Medication>> GetDistintMedicationsByConditionAndPatientId(string condition, int patientId);
        Task<List<Medication>> GetMedicationByRxNumber(string rxNumber);
        Task<List<Medication>> GetUniqueMedicationByDrugSubGroup();
        Task<List<Patient>> GetPatientByUniqueMedication(int recordNumber, int pageLimit, string genericName,string sbdcName, string userId, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory);
        Task<List<Patient>> GetPatientsByUserId(string userId);
        Task<List<Medication>> GetMedicationdetails(string rxNumber);
        Task<List<Medication>> GetDistintMedicationsByConditionAndPatientIdForEarlyMedicationPDC(string condition, int patientId);
        Task<List<Medication>> getMedicationByNDCNumber(string nDCNumber);
        Task<List<Pdc_Medication>> GetAllPdcMedication();
        Task<Pdc_Medication> GetPdcMedicationWithNdcNumber(string ndcNumber);

        Task<List<Medication>> GetMedicationByPatientIdAndRxNumber(int patientId, string rxNumber);
        Task<List<Medication>> GetDistintMedicationsByConditionAndPatientIdAndSubgroupForEarlyMedicationPDC(string condition, int patientId, string value_set_subgroup);
        Task<List<Medication>> GetUniqueMedicationByPatientId(int id);
        Task<MedicationDto> GetUniqueMedicationByDrugSubGroupDto(string genericName, string sbdcName);
        Task<List<Medication>> GetUniqueMedicationByDrugSubGroupForGeneric(int recordNumber, int pageLimit, string userId, string keywords, string sortDirection);
        Task<List<Medication>> getAllMedicationsByPharmacyIDForAll(int pharmacyId);
        Task<Doctor> getDoctorToMedicationByMedicationId(int id);
        Task<List<Medication>> GetUniqueMedicationsByPatientIdForCmrMedication(int patientId);
        void DeleteMedication(Medication medication);

        Task<List<string>> GetUniqueConditionByPatientId(int patientId);

        Task<List<MedicationHistory>> GetPatientMedicationHistoryByDrugNameAsync(int patientId, string drugName);

        Task<MedicationUsageStatusUpdateResult> SetMedicationUsageStatusAsync(int medicationId, bool status);
        Task<bool> SetMedicationRefillDueStatusAsync(int medicationId, bool status);

        Task<List<MedicationCondition>> GetAllMedicationCondition();

        Task<MedicationCondition> GetMedicationConditionByName(string text);

        Task<MedicationCondition> AddMedicationCondition(MedicationCondition medicationCondition);
        Task DeleteMedicationHardCore(int patientId);
        Task<string> GetExlusionMedication(int patientId, string condition);
    }
}
