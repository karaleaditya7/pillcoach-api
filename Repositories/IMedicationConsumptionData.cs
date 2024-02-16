using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IMedicationConsumptionData
    {
        Task<MedicationConsumption> Add(MedicationConsumption medicationConsumption);
        Task<MedicationConsumption> GetRecentMedicationConsumption(string RXNumber, int PatientID);
        Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndGenericDrugName(Medication medication, Patient patient, DateTime startdate, DateTime enddate, Boolean status);
        Task<MedicationConsumption> GetMedicationConsumptionByDateAndRxNumber(DateTime date, string RXNumber);
        MedicationConsumption Update(MedicationConsumption medicationConsumption);
        Task<MedicationConsumption> GetMedicationConsumptionByDate(DateTime date);
        Task<MedicationConsumption> GetRecentMedicationConsumptionByPatientId(int PatientID);
        Task<List<Pdc_Medication>> GetListPdcMedicationByNDCNumber(string ndcNumber);
        Task<MedicationConsumption> GetMedicationConsumptionByDateAndPatientId(DateTime date, int patienId);
        Task<MedicationConsumption> GetRecentMedicationConsumptionByGenericDrugNameAndPatientId(string genericDrugName, int PatientID);
        Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndCondition(Medication medication, Patient patient, DateTime startdate, DateTime enddate, Boolean status);
        Task<List<MedicationConsumption>> GetMedicationConsumptionByDateAndPatientIdWithGenericDrugName(DateTime date, int patienId, string genericDrugName);
        Task<MedicationConsumption> GetMedicationConsumptionByDateAndPatientIdWithCondition(DateTime date, int patienId, string condition);
        Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndConditionForPDC(Medication medication, int patientId, DateTime startdate, DateTime enddate, Boolean status);

        void DeleteMedicationConsumption(MedicationConsumption medicationConsumption);
        Task<List<MedicationConsumption>> GetAllMedicationConsumptionsDataByPatientId(int patientId);
    }
}
