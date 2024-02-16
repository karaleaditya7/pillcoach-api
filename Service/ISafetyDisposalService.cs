using Microsoft.AspNetCore.Http;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface ISafetyDisposalService
    {
        void InsertIntoDBForSafetyDisposalFromTextFile(IFormFile file);
        Task<Response<SafetyDisposal>> GetSafetyDisposalByZipCode(string zipCode, string username);
        Task<Response<SafetyDisposalModel>> GetNearByAddressByZipCode(string zipCode, string username);
    }
}
