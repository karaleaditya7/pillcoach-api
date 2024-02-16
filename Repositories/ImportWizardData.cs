using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto.ImportWizard;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories;

public interface IImportWizardData
{
    Task<int> CreateDraftAsync(int pharmacyId, string userId, string filename, string blobName);
    Task<ImportSourceFile> GetSourceFileByIdAsync(int id);
    Task<bool> IsOtherFileInProcess(int pharmacyId, int excludeSourceFileId);
    Task<List<ImportSourceFile>> GetDraftSourceFileByPharmacyIdAsync(int pharmacyId);
    void DeleteDraftSourceFileAsync(ImportSourceFile importSourceFile);
    Task<int> SaveChangesAsync();
    Task<List<ImportHistory>> GetImportHistoryAsync(int pharmacyId);
    Task<string> GetLastColumnMappingsJsonAsync(int pharmacyId);
    Task<FileProcessingErrorDto> GetFileProcessingErrorAsync(int sourceFileId);
}

internal class ImportWizardData : IImportWizardData
{
    readonly ApplicationDbContext dbContext;

    public ImportWizardData(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ImportSourceFile> GetSourceFileByIdAsync(int id)
    {
        return await dbContext.ImportSourceFiles
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<List<ImportSourceFile>> GetDraftSourceFileByPharmacyIdAsync(int pharmacyId)
    {
      //  var statusId = (int)Enums.ImportFileStatus.Draft;

        var importSourceFiles = await dbContext.ImportSourceFiles
            .Where(f => f.PharmacyId == pharmacyId ).ToListAsync();
        return importSourceFiles;
    }

    public void DeleteDraftSourceFileAsync(ImportSourceFile importSourceFile)
    {
          dbContext.ImportSourceFiles.Remove(importSourceFile);
            
    }

    public async Task<int> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> CreateDraftAsync(int pharmacyId, string userId, string filename, string blobName)
    {
        var record = new ImportSourceFile
        {
            PharmacyId = pharmacyId,
            UserId = userId,
            Filename = filename,
            BlobName = blobName,
            ImportStatusId = (int)Enums.ImportFileStatus.Draft,
            UploadDateUTC = DateTime.UtcNow
        };

        dbContext.ImportSourceFiles.Add(record);

        if (await dbContext.SaveChangesAsync() == 0)
            return 0;

        return record.Id;
    }

    public async Task<bool> IsOtherFileInProcess(int pharmacyId, int excludeSourceFileId)
    {
        var inProcessStatus = new int[]
        {
            (int)Enums.ImportFileStatus.Uploaded,
            (int)Enums.ImportFileStatus.StagingInProgress,
            (int)Enums.ImportFileStatus.StagingCompleted,
            (int)Enums.ImportFileStatus.StagingFailed,
            (int)Enums.ImportFileStatus.ImportInProgress,
            (int)Enums.ImportFileStatus.PartiallyImported,
            (int)Enums.ImportFileStatus.ImportFailed
        };

        var otherFile = await dbContext.ImportSourceFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.PharmacyId == pharmacyId && f.Id != excludeSourceFileId && inProcessStatus.Contains(f.ImportStatusId));

        return otherFile != null;
    }

    public async Task<List<ImportHistory>> GetImportHistoryAsync(int pharmacyId)
    {
        var query = dbContext.ImportSourceFiles
            .Where(i => i.PharmacyId == pharmacyId)
            .OrderByDescending(i => i.UploadDateUTC)
            .Select(i => new ImportHistory
            {
                ImportId = i.Id,
                Filename = i.Filename,
                ImportDateUTC = i.ImportEndTimeUTC ?? i.ImportStartTimeUTC,
                RecordsImported = i.TotalImported ?? 0,
                RecordsUploaded = i.TotalRecords ?? 0,
                Status = i.Status.Name,
                UploadDateUTC = i.UploadDateUTC,
                UploadedBy = $"{i.User.FirstName} {i.User.LastName}",
                HasErrors = !(string.IsNullOrWhiteSpace(i.ErrorsJson) && string.IsNullOrWhiteSpace(i.ErrorStack)),
                BlobReceived = !(i.Status.Id == (int)Enums.ImportFileStatus.Draft)
            });

        return await query.ToListAsync() ?? new();
    }

    public async Task<string> GetLastColumnMappingsJsonAsync(int pharmacyId)
    {
        var query = dbContext.ImportSourceFiles
            .Where(i => i.PharmacyId == pharmacyId && !string.IsNullOrWhiteSpace(i.ColumnMappingsJson))
            .OrderByDescending(i => i.UploadDateUTC)
            .Select(i => i.ColumnMappingsJson)
            .Take(1);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<FileProcessingErrorDto> GetFileProcessingErrorAsync(int sourceFileId)
    {
        return await dbContext.ImportSourceFiles
            .Where(i => i.Id == sourceFileId)
             .Select(i => new FileProcessingErrorDto
             {
                 ErrorsJson = i.ErrorsJson,
                 PharmacyId = i.PharmacyId,
                 FileName = i.Filename                                                              
             })
        //.Select(i => i.ErrorsJson)
        .FirstOrDefaultAsync();
    }
}
