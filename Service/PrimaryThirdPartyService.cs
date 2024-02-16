using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.Data;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class PrimaryThirdPartyService:IPrimaryThirdPartyService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPrimaryThirdPartyData _primaryThirdPartyData;

        public PrimaryThirdPartyService(ApplicationDbContext applicationDbContext,IPrimaryThirdPartyData primaryThirdPartyData)
        {
            _applicationDbContext = applicationDbContext;
            _primaryThirdPartyData = primaryThirdPartyData;
        }

        public void InsertIntoDBForPlanDataReader(DataSet dsexcelRecords)
        {
            DataTable dtRecords = dsexcelRecords.Tables[0];
            if (dtRecords != null)
            {
                int count = 0;
                int headerNameIndex = FindHeaderIndex(dtRecords, "Name");
                int headerBinIndex = FindHeaderIndex(dtRecords, "BIN");
                for (int i = 1; i < dtRecords.Rows.Count; i++)
                {
                    if (dtRecords.Rows[i][0] != DBNull.Value && dtRecords.Rows[i][1] != DBNull.Value)
                    {
                        _applicationDbContext.PrimaryThirdParties.Add(new PrimaryThirdParty
                        {
                            OrganizationMarketingName = dtRecords.Rows[i][headerNameIndex].ToString(),
                            Bin = dtRecords.Rows[i][headerBinIndex].ToString()
                        });
                    }
                    count++;
                    if (count == 500)
                    {
                        _applicationDbContext.SaveChanges();
                        count = 0;
                    }
                }
                _applicationDbContext.SaveChanges();
            }
        }

        public async Task<Response<PrimaryThirdParty>> GetAllPlans(int recordNumber, int pageLimit, string keywords)
        {
            Response<PrimaryThirdParty> response = new Response<PrimaryThirdParty>();
            var plans = await _primaryThirdPartyData.GetAllPlans(recordNumber,pageLimit,keywords);
            if (plans == null)
            {
                response.Success = false;
                response.Message = "Plans Not Found";
                return response;
            }

            response.Success = true;
            response.Message = "Plans retrived successfully";
            response.DataList = plans;
            return response;
        }
        private int FindHeaderIndex(DataTable dtRecords, string targetHeader)
        {
            for (int col = 0; col < dtRecords.Columns.Count; col++)
            {
                var header = dtRecords.Rows[0][col].ToString().Trim();

                if (header.Equals(targetHeader, StringComparison.OrdinalIgnoreCase))
                {
                    return col;
                }
            }
            return -1;
        }
    }
}
