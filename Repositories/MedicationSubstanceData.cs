using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class MedicationSubstanceData : IMedicationSubstanceData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public MedicationSubstanceData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<MedicationSubstance> AddMedicationSubstance(MedicationSubstance medicationSubstance)
        {
            var result = await _applicationDbcontext.MedicationSubstances.AddAsync(medicationSubstance);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<MedicationSubstance> GetMedicationSubstanceById(int medicationSubstanceId)
        {
            var medicationSubstance = await _applicationDbcontext.MedicationSubstances.Where(m => m.Id == medicationSubstanceId).
                                            FirstOrDefaultAsync();

            return medicationSubstance;
        }

        public async Task<MedicationSubstance> GetMedicationSubstanceByName(string MedicationSubstance)
        {
            var medicationSubstance = await _applicationDbcontext.MedicationSubstances.Where(m => m.Name == MedicationSubstance).
                                            FirstOrDefaultAsync();

            return medicationSubstance;
        }
    }
}
