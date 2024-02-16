using Azure.Storage.Blobs;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OntrackDb.Dto;
using OntrackDb.Dto.ImportWizard;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OntrackDb.Service;

public interface IImportWizardService
{
    Task<Response<ImportHistory>> GetImportHistoryAsync(int pharmacyId, int recordNumber, int pageLimit, string filterType, string filterValue, DateTime startDate, DateTime endDate, string keywords);
    Task<Response<ColumnMapping>> GetLastColumnMappingsAsync(int pharmacyId);
    Task<Response<UploadResponseDTO>> UploadSourceFileAsync(int pharmacyId, IFormFile file);
    Task<Response<string>> UpdateColumnMappingsAsync(ColumnMappingRequestDTO request);
    Task<Response<FileProcessingErrorDto>> GetFileErrorAsync(int sourceFileId, int recordNumber, int pageLimit);
    Task<Response<bool>> RerunImportProcessAsync(int sourceFileId);
}

public class ImportWizardService : IImportWizardService
{
    readonly IHttpContextAccessor httpContextAccessor;
    readonly BlobServiceClient blobServiceClient;
    readonly IImportWizardData importWizardData;
    readonly IPharmacyData pharmacyData;
    readonly IConfiguration configuration;
    readonly INotificationData notificationData;
    readonly IUserData userData;
    private readonly IImportLogService importLogService;

    public ImportWizardService(IHttpContextAccessor httpContextAccessor, BlobServiceClient blobServiceClient,
        IImportWizardData importWizardData, IPharmacyData pharmacyData, IConfiguration configuration, INotificationData notificationData, IUserData userData,IImportLogService importLogService)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.blobServiceClient = blobServiceClient;
        this.importWizardData = importWizardData;
        this.pharmacyData = pharmacyData;
        this.configuration = configuration;
        this.notificationData = notificationData;
        this.userData = userData;
        this.importLogService = importLogService;
    }

    public async Task<Response<ImportHistory>> GetImportHistoryAsync(int pharmacyId, int recordNumber, int pageLimit, string filterType, string filterValue, DateTime startDate, DateTime endDate, string keywords)
    {
        //var result = new Response<ImportHistory>
        //{
        //    DataList = await importWizardData.GetImportHistoryAsync(pharmacyId),
        //    Success = true
        //};
        var pharmacyHistory = await importWizardData.GetImportHistoryAsync(pharmacyId);

        //foreach (ImportHistory importHistory in pharmacyHistory)
        //{
        //    var blobFile = await importLogService.DownloadFile(importHistory.ImportId);
        //    importHistory.BlobUri = blobFile;
        //}

        if (!string.IsNullOrWhiteSpace(filterType))
        {
            switch (filterType)
            {
                case "status":
                    pharmacyHistory = pharmacyHistory.Where(x => x.Status != null && x.Status.ToLower().Contains(filterValue.ToLower())).ToList();
                    break;

                case "received":
                    if (filterValue == "true")
                    {
                        bool success = true;
                        pharmacyHistory = pharmacyHistory.Where(x => x.BlobReceived.Equals(success)).ToList();
                    }
                    else
                    {
                        bool success = false;
                        pharmacyHistory = pharmacyHistory.Where(x => x.BlobReceived.Equals(success)).ToList();
                    }
                    break;

                case "date":
                    pharmacyHistory = pharmacyHistory.Where(x => x.ImportDateUTC >= startDate && x.ImportDateUTC <= endDate).ToList();
                    break;

                case "error":
                    if (filterValue == "true")
                    {
                        bool error = true;
                        pharmacyHistory = pharmacyHistory.Where(x => x.HasErrors == error).ToList();
                    }
                    else
                    {
                        bool error = false;
                        pharmacyHistory = pharmacyHistory.Where(x => x.HasErrors == error).ToList();
                    }
                    break;

            }
        }
        else if (!string.IsNullOrWhiteSpace(keywords))
        {
            pharmacyHistory = pharmacyHistory.Where(p => (p.UploadedBy != null && p.UploadedBy.ToLower().Contains(keywords.ToLower())) || (p.ImportNumber != null && p.ImportNumber.ToLower().Contains(keywords.ToLower())) || (p.Filename != null && p.Filename.ToLower().Contains(keywords.ToLower())))
            .OrderByDescending(x=>x.UploadDateUTC)
           .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
           .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
           .ToList();
        }
        else
        {
            pharmacyHistory = pharmacyHistory
           .OrderByDescending(x => x.UploadDateUTC)
           .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
           .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
           .ToList();
        }

        var result = new Response<ImportHistory>
        {
            DataList = pharmacyHistory,
            Success = true
        };

        return result;
    }

    public async Task<Response<ColumnMapping>> GetLastColumnMappingsAsync(int pharmacyId)
    {
        var result = new Response<ColumnMapping>();

        try
        {
            var json = await importWizardData.GetLastColumnMappingsJsonAsync(pharmacyId);

            if (!string.IsNullOrWhiteSpace(json))
            {
                result.DataList = JsonConvert.DeserializeObject<List<ColumnMapping>>(json);
                result.Success = true;
            }
            else
            {
                result.Success = false;
                result.Message = $"No mappings found for pharmacy id: {pharmacyId}";
            }
        }
        catch
        {
            result.Success = false;
            result.Message = "Failed to retrieve mappings";
        }

        return result;
    }

    public async Task<Response<UploadResponseDTO>> UploadSourceFileAsync(int pharmacyId, IFormFile file)
    {
        var response = new Response<UploadResponseDTO>();

        try
        {
            var pharmacy = await pharmacyData.GetPharmacyById(pharmacyId);

            if (pharmacy == null || pharmacy.IsDeleted)
            {
                response.Message = "Invalid Pharmacy";
                return response;
            }

            var userId = httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                response.Message = "Unable to detect User";
                return response;
            }

            var filename = file.FileName;

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            DataSet dataset = null;

            // read excel file for column details

            using (var stream = file.OpenReadStream())
            {
                IExcelDataReader excelReader = null;

                if (filename.EndsWith(".xls", StringComparison.OrdinalIgnoreCase))
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

                else if (filename.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                if (excelReader != null)
                {
                    dataset = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            FilterRow = (r) => r.Depth <= 1 // include the header and first record only
                        }
                    });
                }

                excelReader.Close();
            }

            if (dataset == null)
            {
                response.Message = "Invalid file format - expected .XLSX or .XLS";
                return response;
            }

            var table = dataset.Tables[0];
            var recordCount = table.Rows.Count;
            var fieldCount = table.Columns.Count;

            if (recordCount == 0 || fieldCount == 0)
            {
                response.Message = "No record found in the uploaded file";
                return response;
            }

            // collect column names in a list - to be returned to the caller

            var columnNames = new List<string>();

            for (int i = 0; i < fieldCount; i++)
                columnNames.Add(table.Columns[i].ColumnName);

            columnNames.Sort();

            // upload file to Azure storage

            var uniqueId = Guid.NewGuid().ToString().ToLower();
            string blobname = $"{uniqueId}{Path.GetExtension(filename)}".ToLower();

            var containerName = configuration.GetValue<string>("PatientImport:ContainerName");

            if (string.IsNullOrWhiteSpace(containerName))
            {
                response.Message = "Configuration missing for upload location";
                return response;
            }

            var blobContainer = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainer.GetBlobClient($"{pharmacy.NpiNumber}/{blobname}");

            var result = await blobClient.UploadAsync(file.OpenReadStream());

            var uploaded = await blobClient.ExistsAsync();
            
            if (!uploaded.Value)
            {
                response.Message = "Failed to upload file";
                return response;
            }

            // create initial record in the database

            var recordId = await importWizardData.CreateDraftAsync(pharmacyId, userId, filename, blobname);

            if (recordId <= 0)
            {
                response.Message = "Failed to initialize record";
                return response;
            }

            response.Data = new UploadResponseDTO
            {
                RecordId = recordId,
                UniqueId = uniqueId,
                ColumnNames = columnNames
            };

            response.Success = true;
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<Response<string>> UpdateColumnMappingsAsync(ColumnMappingRequestDTO request)
    {
        var response = new Response<string>();

        // removed all validations and updates, as they will be done by the background function

        var userId = httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

        var sourceFile = await importWizardData.GetSourceFileByIdAsync(request.RecordId);

        if (sourceFile == null || !sourceFile.BlobName.StartsWith(request.UniqueId) || sourceFile.UserId != userId ||
            (sourceFile.ImportStatusId != (int)Enums.ImportFileStatus.Draft && sourceFile.ImportStatusId != (int)Enums.ImportFileStatus.Uploaded))
        {
            response.Message = "No matching source file found";
            return response;
        }

        response.Success = true;
        response.Data = sourceFile.RequestId;

        return response;
    }

    public async Task<Response<bool>> RerunImportProcessAsync(int sourceFileId)
    {
        var response = new Response<bool>();

        var sourceFile = await importWizardData.GetSourceFileByIdAsync(sourceFileId);

        var eligibleStatus = new int[]
        {
            (int)Enums.ImportFileStatus.StagingCompleted,
            (int)Enums.ImportFileStatus.PartiallyImported
        };

        if (sourceFile == null || !eligibleStatus.Contains(sourceFile.ImportStatusId))
        {
            response.Message = "Source file not found or file is not in correct status";
            return response;
        }

        response.Success = await CallImportFunctionAsync(sourceFileId, skipStaging: true);
        response.Data = response.Success;

        return response;
    }

    private async Task<bool> CallImportFunctionAsync(int sourceFileId, bool skipStaging)
    {
        try
        {
            var json = JsonConvert.SerializeObject(new
            {
                sourceFileId,
                skipStaging
            });

            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var httpclient = new HttpClient();

            var url = configuration["AzureFunctionUrl"];

            var isLocal = httpContextAccessor.HttpContext.Request.Host.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase);
            if (isLocal) url = "http://localhost:7071";

            httpclient.BaseAddress = new Uri(url);
            httpclient.DefaultRequestHeaders.Add("x-functions-key", configuration["AzureFunctionApiKey"]);

            using var resp = await httpclient.PostAsync("/api/ImportFunction_HttpStart", content);

            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private async Task AddNotificationAsync(ImportSourceFile sourceFile, string message)
    {
        try
        {
            var user = await userData.GetUserByUserId(sourceFile.UserId);
            var pharmacy = await pharmacyData.GetPharmacyById(sourceFile.PharmacyId);
            var pharmacyName = pharmacy.Name;

            var notification = new AdminNotification
            {
                Status = "Patients Import",
                User = user,
                NotificationType = $"{pharmacyName} > {sourceFile.RequestId}: {message}",
                SendDateTime = DateTime.Now,
                ReadDateTime = DateTime.MinValue,
                ForSuperAdminOnly = true
            };

            await notificationData.AddAdminNotification(notification);
        }
        catch { }
    }

    private bool ValidateColumnMappings(List<ColumnMapping> mappings, out string errorMessage)
    {
        errorMessage = null;

        var stagingColumns = GetAllStagingColumns();

        var invalidMappings = mappings.Where(m => stagingColumns.All(sc => m.PCField != sc.ColumnName));

        if (invalidMappings.Any())
        {
            errorMessage = $"Mapping provided for unknown column(s): {string.Join(", ", invalidMappings.Select(c => c.PCField))}";
            return false;
        }

        var missingColumns = stagingColumns.Where(sc => !sc.IsOptional && mappings.All(m => m.PCField != sc.ColumnName));

        if (missingColumns.Any())
        {
            errorMessage = $"Mapping missing for: {string.Join(", ", missingColumns.Select(c => c.ColumnName))}";
            return false;
        }

        var comparer = new KeyComparer();

        var duplicateExternalMappings = mappings.GroupBy(m => m.ExternalField, comparer).Where(g => g.Count() > 1);

        if (duplicateExternalMappings.Any())
        {
            errorMessage = $"Multiple mapping provided for: {string.Join(", ", duplicateExternalMappings.Select(c => c.Key.Trim()))}";
            return false;
        }

        var duplicateInternalMappings = mappings.GroupBy(m => m.PCField, comparer).Where(g => g.Count() > 1);

        if (duplicateInternalMappings.Any())
        {
            errorMessage = $"Multiple mapping provided for: {string.Join(", ", duplicateInternalMappings.Select(c => c.Key.Trim()))}";
            return false;
        }

        return true;
    }

    private List<PCStagingColumn> GetAllStagingColumns()
    {
        return new List<PCStagingColumn>
        {
            new() { ColumnName = "PatientIdentifier" },
            new() { ColumnName = "PatientFirstName" },
            new() { ColumnName = "PatientLastName" },
            new() { ColumnName = "PatientDateofBirth" },
            new() { ColumnName = "PatientPrimaryAddress" },
            new() { ColumnName = "PatientPrimaryCity" },
            new() { ColumnName = "PatientPrimaryState" },
            new() { ColumnName = "PatientPrimaryZipCode" },
            new() { ColumnName = "PatientPrimaryPhone" },
            new() { ColumnName = "PatientEmail", IsOptional = true },
            new() { ColumnName = "PatientGender", IsOptional = true },
            new() { ColumnName = "PatientLanguage", IsOptional = true },
            new() { ColumnName = "PatientRace", IsOptional = true },
            new() { ColumnName = "PrescriberFirstName" },
            new() { ColumnName = "PrescriberLastName" },
            new() { ColumnName = "PrescriberNPI" },
            new() { ColumnName = "PrescriberPrimaryAddress" },
            new() { ColumnName = "PrescriberPrimaryCity" },
            new() { ColumnName = "PrescriberPrimaryState" },
            new() { ColumnName = "PrescriberPrimaryZip" },
            new() { ColumnName = "PrescriberPrimaryPhone" },
            new() { ColumnName = "PrescriberFaxNumber" },
            new() { ColumnName = "RxNumber" },
            new() { ColumnName = "RefillNumber" },
            new() { ColumnName = "DateFilled" },
            new() { ColumnName = "RxDate" }, // new column: prescribed date
            new() { ColumnName = "DaysSupply" },
            new() { ColumnName = "RefillsRemaining" },
            new() { ColumnName = "DispensedQuantity" },
            new() { ColumnName = "DispensedItemNDC" },
            new() { ColumnName = "DispensedItemName" },
            new() { ColumnName = "Directions" },
            new() { ColumnName = "PatientPaidAmount" },
            new() { ColumnName = "PrimaryThirdParty" },
            new() { ColumnName = "PrimaryThirdPartyBin" }
        };
    }

    public async Task<Response<FileProcessingErrorDto>> GetFileErrorAsync(int sourceFileId, int recordNumber, int pageLimit)
    {
        //var response = new Response<JObject>();
        var response = new Response<FileProcessingErrorDto>();

        try
        {
            var errorJson = await importWizardData.GetFileProcessingErrorAsync(sourceFileId);

            if (!string.IsNullOrWhiteSpace(errorJson.ErrorsJson))
            {
                //response.Data = JObject.Parse(errorJson);

                var result = JsonConvert.DeserializeObject<Dictionary<string, List<StagingError>>>(errorJson.ErrorsJson);
                List<StagingError> allErrors = new List<StagingError>();

                foreach (var kvp in result)
                {
                    allErrors.AddRange(kvp.Value);
                }

                errorJson.ErrorsJson = null;
                errorJson.StagingErrors = allErrors.Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
               .ToList();
                
                response.Data = errorJson;
                response.Success = true;
            }
            else
            {
                response.Message = "No errors found";
            }
        }
        catch
        {
            response.Message = "Unable to retrieve errors";
        }

        return response;
    }

    class PCStagingColumn
    {
        public string ColumnName { get; set; }
        public bool IsOptional { get; set; }
    }

    class KeyComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null || y == null) return false;

            return x.Trim().Equals(y.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.Trim().ToLower().GetHashCode();
        }
    }
}
