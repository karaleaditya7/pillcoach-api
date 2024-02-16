using Newtonsoft.Json.Linq;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IWebhookData
    {
        Task<ImportData> DumpImportData(ImportData importData);
        Task<ImportData> UpdateImportData(ImportData importData);
        Task<ImportData> GetImportDataById(int id);
    }
}
