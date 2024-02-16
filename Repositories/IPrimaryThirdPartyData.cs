using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IPrimaryThirdPartyData
    {
        Task<List<PrimaryThirdParty>> GetAllPlans(int recordNumber, int pageLimit, string keywords);
        Task<PrimaryThirdParty> GetPlanById(int planId);
    }
}
