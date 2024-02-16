using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class SafetyDisposalData : ISafetyDisposalData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
       
        public SafetyDisposalData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;   
  
        }

        public async Task<SafetyDisposal> GetSafetyDisposalsByZipCode(string zipCode)
        {
            return await _applicationDbcontext.SafetyDisposals.Where(a => a.ZIP == zipCode).FirstOrDefaultAsync();
        }

      
        public async Task<List<SafetyDisposal>> GetSafetyDisposalsByZipCodeForLatitudeAndLongitude(string lat, string longi)
        {
             lat = lat.Substring(0, 3);
             longi = longi.Substring(0, 4);
            var result = await _applicationDbcontext.SafetyDisposals.Where(s =>s.LATITUDE.Contains(lat) && s.LONGITUDE.Contains(longi)).OrderBy(s=>s.LATITUDE).ToListAsync();
            return result;
        }



    }
}
