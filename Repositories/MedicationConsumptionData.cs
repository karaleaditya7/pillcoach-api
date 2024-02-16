using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class MedicationConsumptionData:IMedicationConsumptionData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public MedicationConsumptionData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext= applicationDbcontext;
        }
        public async Task<MedicationConsumption> Add (MedicationConsumption medicationConsumption)
        {
            var result = await _applicationDbcontext.medicationConsumptions.AddAsync(medicationConsumption);

            return result.Entity;
        }

        public MedicationConsumption Update(MedicationConsumption medicationConsumption)
        {
            var result = _applicationDbcontext.medicationConsumptions.Update(medicationConsumption);
           
            return result.Entity;
        }

        public void DeleteMedicationConsumption(MedicationConsumption medicationConsumption)
        {
             _applicationDbcontext.medicationConsumptions.Remove(medicationConsumption);

            
        }

        public async Task<MedicationConsumption> GetRecentMedicationConsumption(string RXNumber, int PatientID)
        {
            var result = await _applicationDbcontext.medicationConsumptions.OrderBy(x => x.Id)
                             .FirstOrDefaultAsync(x => x.RxNumber == RXNumber && x.PatienId == PatientID);
            return result;

        }

        public async Task<MedicationConsumption> GetRecentMedicationConsumptionByGenericDrugNameAndPatientId(string genericDrugName, int PatientID)
        {
            var result = await _applicationDbcontext.medicationConsumptions.OrderBy(x => x.Id)
                             .FirstOrDefaultAsync(x => x.DrugSubGroup == genericDrugName && x.PatienId == PatientID);
            return result;

        }

        public async Task<List<Pdc_Medication>> GetListPdcMedicationByNDCNumber(string ndcNumber)
        {
            var result = await _applicationDbcontext.Pdc_Medications.Where(p=>p.code == ndcNumber).ToListAsync();
            return result;

        }

        public async Task<MedicationConsumption> GetRecentMedicationConsumptionByPatientId(int PatientID)
        {
            var result = await _applicationDbcontext.medicationConsumptions.OrderBy(x => x.Id)
                             .FirstOrDefaultAsync(x => x.PatienId == PatientID);
            return result;

        }

        public async Task<MedicationConsumption> GetMedicationConsumptionByDateAndRxNumber(DateTime date, string RXNumber)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.RxNumber == RXNumber && x.Date == date).FirstOrDefaultAsync();
            return result;

        }

        public async Task<MedicationConsumption> GetMedicationConsumptionByDateAndPatientId(DateTime date, int patienId)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patienId && x.Date == date).FirstOrDefaultAsync();
            return result;

        }

        public async Task<MedicationConsumption> GetMedicationConsumptionByDateAndPatientIdWithCondition(DateTime date, int patienId,string condition)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patienId && x.Date == date && x.Condition == condition).FirstOrDefaultAsync();
            return result;

        }

        public async Task<List<MedicationConsumption>> GetMedicationConsumptionByDateAndPatientIdWithGenericDrugName(DateTime date, int patienId,string genericDrugName)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patienId && x.Date >= date && x.DrugSubGroup == genericDrugName).ToListAsync();
            return result;

        }

        public async Task<List<MedicationConsumption>> GetAllMedicationConsumptionsDataByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patientId).ToListAsync();
            return result;

        }

        public async Task<MedicationConsumption> GetMedicationConsumptionByDate(DateTime date)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.Date == date).FirstOrDefaultAsync();
            return result;

        }

        public async Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndGenericDrugName(Medication medication,Patient patient,DateTime startdate, DateTime enddate ,Boolean status)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x=>x.PatienId == patient.Id && x.DrugSubGroup == medication.DrugSubGroup && x.Status == status && x.Date >= startdate && x.Date < enddate ).ToListAsync();
            return result;

        }

        public async Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndCondition(Medication medication, Patient patient, DateTime startdate, DateTime enddate, Boolean status)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patient.Id && x.Condition == medication.Condition && x.Status == status && x.Date >= startdate && x.Date <= enddate).ToListAsync();
            return result;

        }

        public async Task<List<MedicationConsumption>> GetMedicationConsumptionByPatientIDAndConditionForPDC(Medication medication, int patientId, DateTime startdate, DateTime enddate, Boolean status)
        {
            var result = await _applicationDbcontext.medicationConsumptions.Where(x => x.PatienId == patientId && x.Condition == medication.Condition && x.Status == status && x.Date >= startdate && x.Date <= enddate).ToListAsync();
            return result;

        }
    }
}
