using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System.Data;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IPrimaryThirdPartyService
    {
        void InsertIntoDBForPlanDataReader(DataSet dsexcelRecords);
        Task<Response<PrimaryThirdParty>> GetAllPlans(int recordNumber,int pageLimit,string keywords);
    }
}
