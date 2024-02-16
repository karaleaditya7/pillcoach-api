using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface INdcApiService
    {
        Task<string> GetRelatedNDCs(string NDCNumber);
        Task<Pdc_Medication> ForPdcMedicationAddition(string NDCNumber);
    }
}
