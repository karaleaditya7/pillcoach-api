using OntrackDb.Dto;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface ISafetyDisposalData
    {
        Task<SafetyDisposal> GetSafetyDisposalsByZipCode(string zipCode);
        Task<List<SafetyDisposal>> GetSafetyDisposalsByZipCodeForLatitudeAndLongitude(string lat, string longi);
     

    }
}
