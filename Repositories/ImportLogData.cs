using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class ImportLogData : IImportLogData
    {
        private readonly ApplicationDbContext dbContext;

        public ImportLogData(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<List<ImportLogsHistoryDto>> GetLatestImportHistory(int recordNumber, int pageLimit, string filterType, string filterValue,DateTime startDate,DateTime endDate, string keywords, string sortDirection)
        {
            var latestRecords = await dbContext.ImportSourceFiles.Include(x=>x.Status).Include(x=>x.User)
            .GroupBy(i => i.PharmacyId)
            .Select(group => group
                .OrderByDescending(i => i.UploadDateUTC)
                .FirstOrDefault())
            .ToListAsync();

            var latestRecordsWithPharmacyName = latestRecords
            .AsEnumerable()
            .Select(i => new ImportLogsHistoryDto
            {
                ImportId = i.Id,
                PharmacyId = i.PharmacyId,
                Filename = i.Filename,
                ImportDateUTC = i.ImportEndTimeUTC ?? i.ImportStartTimeUTC,
                RecordsImported = i.TotalImported ?? 0,
                RecordsUploaded = i.TotalRecords ?? 0,
                Status = i.Status.Name,
                UploadDateUTC = i.UploadDateUTC,
                UploadedBy = $"{i.User.FirstName} {i.User.LastName}",
                HasErrors = !(string.IsNullOrWhiteSpace(i.ErrorsJson) && string.IsNullOrWhiteSpace(i.ErrorStack)),
                PharmacyName = dbContext.Pharmacies.FirstOrDefault(p => p.Id == i.PharmacyId)?.Name,
                BlobReceived = !(i.Status.Id == (int)Enums.ImportFileStatus.Draft),
                BlobName = i.BlobName
            }).ToList();
           
            return latestRecordsWithPharmacyName.ToList();
        }

        public async Task<FileDownloadDto> DownloadFile(int importId)
        {
            var file= await dbContext.ImportSourceFiles.Where(x=> x.Id == importId)
                .Select(i => new FileDownloadDto
                {
                    BlobName=i.BlobName,
                    FileName=i.Filename
                }).FirstOrDefaultAsync();

            return file;
        }

        public async Task<ImportFileStagingData> GetErrorDetails(int importId, int rowNumber)
        {
            var errorDetail = await dbContext.ImportFileStagingData
            .Where(x => x.ImportSourceFileId == importId && x.RowNo == rowNumber)
            .FirstOrDefaultAsync();

            return errorDetail;
        }

        public async Task<int> UpdateStaggingErrorRow()
        {
            var result = await dbContext.SaveChangesAsync();
            return result;

        }

        public async Task<int> GetTotalStagingRow(int importId)
        {
            return await dbContext.ImportFileStagingData.Where(x => x.ImportSourceFileId == importId).CountAsync();
        }

        public async Task<ImportSourceFile> GetImportSourceFileRow(int importId)
        {
            return await dbContext.ImportSourceFiles.Where(x => x.Id == importId).FirstOrDefaultAsync();
        }

    }
            
}
