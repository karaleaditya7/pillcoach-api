using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class PrimaryThirdPartyData : IPrimaryThirdPartyData
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public PrimaryThirdPartyData(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<PrimaryThirdParty>> GetAllPlans(int recordNumber, int pageLimit, string keywords)
        {
            List<PrimaryThirdParty> plans = null;
            var conditionCheck = true;

            plans = await _applicationDbContext.PrimaryThirdParties
                .Where(x => (keywords == null || keywords == string.Empty) ||
                            (!string.IsNullOrEmpty(x.OrganizationMarketingName) && x.OrganizationMarketingName.ToLower().Contains(keywords.ToLower())) ||
                            (!string.IsNullOrEmpty(x.Bin) && x.Bin.ToLower().Contains(keywords.ToLower())))
                .OrderByDescending(m => conditionCheck ? 0 : 1) 
                .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                .Take(pageLimit)  
                .ToListAsync();

            return plans;
        }
        public async Task<PrimaryThirdParty> GetPlanById(int planId)
        {
            return await _applicationDbContext.PrimaryThirdParties.
                Where(x=> x.Id == planId).FirstOrDefaultAsync();
        }
    }
}
