using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IMediccationService
    {
        Task<Double>   CalculatePdc(Patient patient, string condition, DateTime startDate, DateTime endDate);
        Task<List<Patient>> CalculatePdcforPatients(List<Patient> patients, string condition, DateTime startDate, DateTime endDate);
        Task<List<Medication>> getAllMedicationsByUserID(string userId);
        int countDueForRefill(List<Medication> medications);
        int countNoRefillRemaining(List<Medication> medications);
     }
}
