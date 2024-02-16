using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Dto.ImportWizard;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IImportLogData
    {
        Task<List<ImportLogsHistoryDto>> GetLatestImportHistory(int recordNumber, int pageLimit, string filterType, string filterValue,DateTime startDate,DateTime endDate, string keywords, string sortDirection);
        Task<FileDownloadDto> DownloadFile(int importId);
        Task<ImportFileStagingData> GetErrorDetails(int importId, int rowNumber);
        Task<int> UpdateStaggingErrorRow();
        Task<int> GetTotalStagingRow(int importId);
        Task<ImportSourceFile> GetImportSourceFileRow(int importId);
    }
}
