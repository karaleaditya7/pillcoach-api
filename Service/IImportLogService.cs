using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Dto.ImportWizard;
using OntrackDb.Entities;
using System;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OntrackDb.Service
{
    public interface IImportLogService
    {
        Task<Response<ImportLogsHistoryDto>> GetLatestImportHistory(int recordNumber, int pageLimit, string filterType, string filterValue,DateTime startDate,DateTime endDate,string keywords, string sortDirection);
        Task<string> DownloadFile(int importId);
        Task<Response<ImportFileStagingData>> GetErrorDetails(int importId, int rowNumber);
        Task<Response<ImportFileStagingData>> UpdateStagingErrorRow(StagingRowErrorDto staggingRowErrorDto);
        Task<Response<string>> UpdateImportSourceFileRow(int importId);
        Task<Response<string>> UpdateImportSourceFileStatusRejected(int importId);
    }
}
