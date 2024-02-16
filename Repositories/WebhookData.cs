using Newtonsoft.Json.Linq;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class WebhookData: IWebhookData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public WebhookData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<ImportData> DumpImportData(ImportData importData)
        {
            var result = await _applicationDbcontext.ImportDatas.AddAsync(importData);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

     

        public async Task<ImportData> UpdateImportData(ImportData importData)
        {
            var result =  _applicationDbcontext.ImportDatas.Update(importData);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ImportData> GetImportDataById(int id)
        {
            var importDataExists = await _applicationDbcontext.ImportDatas.FindAsync(id);
            return importDataExists;
        }
    }
}
