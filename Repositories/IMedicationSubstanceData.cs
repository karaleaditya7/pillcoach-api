using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IMedicationSubstanceData
    {
        Task<MedicationSubstance> AddMedicationSubstance(MedicationSubstance medicationSubstance);
        Task<MedicationSubstance> GetMedicationSubstanceByName(string MedicationSubstance);
        Task<MedicationSubstance> GetMedicationSubstanceById(int medicationSubstanceId);
    }
}
