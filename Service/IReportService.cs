using Microsoft.AspNetCore.Mvc;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IReportService
    {
        Task<char[]> GetPharmacyCSVData(List<string> pharmacyIds, int month);
        Task<List<ReportsModel>> GetPharmacyListByPharmacyIdsAndUserId(List<string> pharmacyIds, int month);
    }
}
