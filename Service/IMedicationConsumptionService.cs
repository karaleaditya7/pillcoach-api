using OntrackDb.Dto;
using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IMedicationConsumptionService
    {
        Task<Response<MedicationConsumption>> GetRecentMedicationConsumption(string RXNumber, int PatientID);
        
    }
}
