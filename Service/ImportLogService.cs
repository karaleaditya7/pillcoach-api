using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Amqp.Framing;
using Newtonsoft.Json.Linq;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using Quartz.Util;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ImportLogService : IImportLogService
    {
        private readonly IImportLogData importLogData;
        private readonly BlobServiceClient blobServiceClient;

        public ImportLogService(IImportLogData importLogData, BlobServiceClient blobServiceClient)
        {
            this.importLogData = importLogData;
            this.blobServiceClient = blobServiceClient;
        }
        public async Task<Response<ImportLogsHistoryDto>> GetLatestImportHistory(int recordNumber, int pageLimit, string filterType, string filterValue,DateTime startDate,DateTime endDate, string keywords, string sortDirection)
        {
            Response<ImportLogsHistoryDto> response = new Response<ImportLogsHistoryDto>();

            bool condition = false;
            if (sortDirection == "asc")
            {
                condition = true;
            }

            if (startDate > endDate)
            {
                response.Message = "Invalid date range";
                return response;
            }
            var importHistory = await importLogData.GetLatestImportHistory(recordNumber, pageLimit, filterType, filterValue,startDate, endDate,keywords,sortDirection);
            
            //foreach (ImportLogsHistoryDto ImportHistory in importHistory)
            //{
            //    var blobFile = await this.DownloadFile(ImportHistory.ImportId);
            //    ImportHistory.BlobUri = blobFile;
            //}

            if(importHistory == null)
            {
                response.Success = false;
                response.Message = "User Not Found";
                return response;
            }
            if(!string.IsNullOrWhiteSpace(filterType)) 
            {
                switch(filterType)
                {
                    case "status":
                        importHistory = importHistory.Where(x => x.Status != null && x.Status.ToLower().Contains(filterValue.ToLower())).ToList();
                        break;
                    case "received":
                        if (filterValue == "true")
                        {
                            bool success = true;
                            importHistory = importHistory.Where(x => x.BlobReceived.Equals(success)).ToList();
                        } 
                        else if(filterValue == "false")
                        {
                            bool success = false;
                            importHistory = importHistory.Where(x => x.BlobReceived.Equals(success)).ToList();
                        }
                        else
                        {
                            response.Success = false;
                            response.Message = "filterValue other than true or false";
                            return response;
                        }
                        break;
                    case "username":
                        importHistory = importHistory.Where(x => x.UploadedBy != null && x.UploadedBy.ToLower().Contains(filterValue.ToLower())).ToList();
                        break;
                    case "date":
                        importHistory = importHistory.Where(x => x.UploadDateUTC.Date >= startDate.Date && x.UploadDateUTC.Date <= endDate.Date).ToList();
                        break;

                }
            }
            else if(!string.IsNullOrWhiteSpace(keywords))
            {
                importHistory = importHistory.Where(p => (p.PharmacyName!=null && p.PharmacyName.ToLower().Contains(keywords.ToLower())) || (p.UploadedBy!=null && p.UploadedBy.ToLower().Contains(keywords.ToLower())))
               .OrderBy(p => condition ? p.PharmacyName : null)
               .ThenByDescending(p => condition ? null : p.PharmacyName)
               .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
               .ToList();
            }
            else
            {
                importHistory = importHistory
               .OrderBy(p => condition ? p.PharmacyName : null)
               .ThenByDescending(p => condition ? null : p.PharmacyName)
               .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
               .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
               .ToList();
            }
            response.Success = true;
            response.DataList = importHistory;
            response.Message = "Import log history";
            return response;
        }

        public async Task<string> DownloadFile(int importId)
        {
            var data = await importLogData.DownloadFile(importId);

            var containerClient = blobServiceClient.GetBlobContainerClient("import-source-files");
            var blobClient = containerClient.GetBlobClient(data.BlobName);
           
            if (containerClient.CanGenerateSasUri)
            {
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                    BlobContainerName = containerClient.Name,
                    BlobName = blobClient.Name,
                    Resource = "b" // "b" indicates a blob resource    
                };
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                var BlobUri = sasUri.ToString();
                return BlobUri;
            }
            return null;

        }

        public async Task<Response<ImportFileStagingData>> GetErrorDetails(int importId,int rowNumber)
        {
            Response<ImportFileStagingData> response = new Response<ImportFileStagingData>();

            var errorDetails = await importLogData.GetErrorDetails(importId,rowNumber);

            if (errorDetails == null)
            {
                response.Success = false;
                response.Message = "User Not Found";
                return response;
            }
           
            response.Success = true;
            response.Data = errorDetails;
            response.Message = "Error details";
            return response;
        }
        public async Task<Response<ImportFileStagingData>> UpdateStagingErrorRow(StagingRowErrorDto stagingRowErrorDto)
        {
            Response<ImportFileStagingData> response = new Response<ImportFileStagingData>();
            ImportFileStagingData errorDetails = await importLogData.GetErrorDetails(stagingRowErrorDto.ImportSourceFileId, stagingRowErrorDto.RowNo);
            if (errorDetails.ErrorsJson == null)
            {
                response.Success = false;
                response.Message = "Row for the error not found";
                return response;
            }

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.PatientIdentifier))
            {
                response.Message = $"Missing or invalid value for: PatientIdentifier.";
                return response;
            } 

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.PatientFirstName))
            {
                response.Message = $"Missing or invalid value for : PatientFirstName.";
                return response;
            } 

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.PatientLastName))
            {
                response.Message = $"Missing or invalid value for: PatientLastName.";
                return response;
            }

            if (stagingRowErrorDto.PatientDateofBirth == null || stagingRowErrorDto.PatientDateofBirth > DateTime.Today || stagingRowErrorDto.PatientDateofBirth == DateTime.MinValue)
            {
                response.Message = $"Missing or invalid value for: PatientDateofBirth.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.PrescriberNPI))
            {
                response.Message = $"Missing or invalid value for: PrescriberNPI.";
                return response;
            }
            if (stagingRowErrorDto.RxDate == null || stagingRowErrorDto.RxDate > DateTime.Today || stagingRowErrorDto.RxDate == DateTime.MinValue)
            {
                response.Message = $"Missing or invalid value for: RxDate.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.RxNumber))
            {
                response.Message = $"Missing or invalid value for: RxNumber.";
                return response;
            }

            if (stagingRowErrorDto.DateFilled == null || stagingRowErrorDto.DateFilled > DateTime.Today || stagingRowErrorDto.DateFilled == DateTime.MinValue)
            {
                response.Message = $"Missing or invalid value for: DateFilled.";
                return response;
            }

            if (stagingRowErrorDto.RefillNumber < 0)
            {
                response.Message = $"Missing or invalid value for: RefillNumber.";
                return response;
            } 

            if (stagingRowErrorDto.DaysSupply <= 0)
            {
                response.Message = $"Missing or invalid value for: DaysSupply.";
                return response;
            }
            var regex = new Regex("[^0-9]");

            if (!string.IsNullOrWhiteSpace(stagingRowErrorDto.PatientPrimaryPhone))
            {
                var PatientPrimaryPhone = regex.Replace(stagingRowErrorDto.PatientPrimaryPhone, "").Trim();

                if (PatientPrimaryPhone.StartsWith("1"))// remove leading US country code, if any
                    PatientPrimaryPhone = PatientPrimaryPhone[1..];

                if (PatientPrimaryPhone.Length > 10)
                {
                    response.Message = $"Missing or invalid value for: PatientPrimaryPhone.";
                    return response;
                }
            }

            if (!string.IsNullOrWhiteSpace(stagingRowErrorDto.PrescriberPrimaryPhone))
            {
                var PrescriberPrimaryPhone = regex.Replace(stagingRowErrorDto.PrescriberPrimaryPhone, "").Trim();

                if (PrescriberPrimaryPhone.StartsWith("1"))// remove leading US country code, if any
                    PrescriberPrimaryPhone = stagingRowErrorDto.PrescriberPrimaryPhone[1..];

                if (PrescriberPrimaryPhone.Length > 10)
                {
                        response.Message = $"Missing or invalid value for: PrescriberPrimaryPhone.";
                        return response;
                }
            }

            if (!string.IsNullOrWhiteSpace(stagingRowErrorDto.PrescriberFaxNumber))
            {
                var PrescriberFaxNumber = regex.Replace(stagingRowErrorDto.PrescriberFaxNumber, "").Trim();

                if (PrescriberFaxNumber.StartsWith("1"))// remove leading US country code, if any
                     PrescriberFaxNumber = PrescriberFaxNumber[1..];

                if (PrescriberFaxNumber.Length > 10)
                {
                    response.Message = $"Missing or invalid value for: PrescriberFaxNumber.";
                    return response;
                }
            }

            /*
        * DispensedItemNDC excluded from validation to allow non-PDC related medications
        * to be imported without specifying a dummy text as NDC
        */

            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.DispensedItemName))
            {
                response.Message = $"Missing or invalid value for: DispensedItemName.";
                return response;
            }
            
            if (string.IsNullOrWhiteSpace(stagingRowErrorDto.Directions))
            {
                response.Message = $"Missing or invalid value for: Directions.";
                return response;
            }
                
            errorDetails.PatientIdentifier = stagingRowErrorDto.PatientIdentifier;
            errorDetails.PatientFirstName = stagingRowErrorDto.PatientFirstName;
            errorDetails.PatientLastName = stagingRowErrorDto.PatientLastName;
            errorDetails.PatientDateofBirth = stagingRowErrorDto.PatientDateofBirth;
            errorDetails.PatientPrimaryAddress = stagingRowErrorDto.PatientPrimaryAddress;
            errorDetails.PatientPrimaryCity = stagingRowErrorDto.PatientPrimaryCity;
            errorDetails.PatientPrimaryState = stagingRowErrorDto.PatientPrimaryState;
            errorDetails.PatientPrimaryZipCode = stagingRowErrorDto.PatientPrimaryZipCode;
            errorDetails.PatientPrimaryPhone = stagingRowErrorDto.PatientPrimaryPhone;
            errorDetails.PatientEmail = stagingRowErrorDto.PatientEmail;
            errorDetails.PatientGender = stagingRowErrorDto.PatientGender;
            errorDetails.PatientLanguage = stagingRowErrorDto.PatientLanguage;
            errorDetails.PatientRace = stagingRowErrorDto.PatientRace;
            errorDetails.PrescriberFirstName = stagingRowErrorDto.PrescriberFirstName;
            errorDetails.PrescriberLastName = stagingRowErrorDto.PrescriberLastName;
            errorDetails.PrescriberNPI = stagingRowErrorDto.PrescriberNPI;
            errorDetails.PrescriberPrimaryAddress = stagingRowErrorDto.PrescriberPrimaryAddress;
            errorDetails.PrescriberPrimaryCity = stagingRowErrorDto.PrescriberPrimaryCity;
            errorDetails.PrescriberPrimaryState = stagingRowErrorDto.PrescriberPrimaryState;
            errorDetails.PrescriberPrimaryZip = stagingRowErrorDto.PrescriberPrimaryZip;
            errorDetails.PrescriberFaxNumber = stagingRowErrorDto.PrescriberFaxNumber;
            errorDetails.RxNumber = stagingRowErrorDto.RxNumber;
            errorDetails.RefillNumber = stagingRowErrorDto.RefillNumber;
            errorDetails.DateFilled = stagingRowErrorDto.DateFilled;
            errorDetails.RxDate = stagingRowErrorDto.RxDate;
            errorDetails.DaysSupply = stagingRowErrorDto.DaysSupply;
            errorDetails.RefillsRemaining = stagingRowErrorDto.RefillsRemaining;
            errorDetails.DispensedQuantity = stagingRowErrorDto.DispensedQuantity;
            errorDetails.DispensedItemNDC = stagingRowErrorDto.DispensedItemNDC;
            errorDetails.DispensedItemName = stagingRowErrorDto.DispensedItemName;
            errorDetails.Directions = stagingRowErrorDto.Directions;
            errorDetails.PatientPaidAmount = stagingRowErrorDto.PatientPaidAmount;
            errorDetails.DrugSbdcName = stagingRowErrorDto.DrugSbdcName;
            errorDetails.ErrorsJson = null;

            //Updating errors of importSourceFiles table
            var importErrorDetails = await importLogData.GetImportSourceFileRow(stagingRowErrorDto.ImportSourceFileId);
            JObject jsonObject = JObject.Parse(importErrorDetails.ErrorsJson);

            JArray stagingErrors = (JArray)jsonObject["stagingErrors"];

            JToken itemToRemove = stagingErrors.FirstOrDefault(item => item["row"] != null && item["row"].Value<int>() == stagingRowErrorDto.RowNo);
            if (itemToRemove != null)
                itemToRemove.Remove();

            string modifiedJsonString = jsonObject.ToString();
            importErrorDetails.ErrorsJson = modifiedJsonString;

            int result = await importLogData.UpdateStaggingErrorRow();
            if (result > 0)
            {
                errorDetails.HasRowUpdated = true;
                response.Data = errorDetails;
                response.Success = true;
                response.Message = "Row Updated successfully!";
            }
            
            return response;
        }

        public async Task<Response<string>> UpdateImportSourceFileRow(int importId)
        {
            Response<string> response = new Response<string>();
            var errorDetails = await importLogData.GetImportSourceFileRow(importId);
            if (errorDetails == null)
            {
                response.Success = false;
                response.Message = "Row not found";
                return response;
            }
            errorDetails.ImportStatusId = 4;
            errorDetails.TotalRecords = await importLogData.GetTotalStagingRow(importId);
            errorDetails.TotalImported = null;
            errorDetails.ErrorsJson = null;
            errorDetails.ImportStartTimeUTC = null;
            errorDetails.ImportEndTimeUTC = null;
            int result = await importLogData.UpdateStaggingErrorRow();
            if (result > 0)
            {
                response.Success = true;
                response.Message = "Row Updated successfully!";
            }

            return response;
        }

        public async Task<Response<string>> UpdateImportSourceFileStatusRejected(int importId)
        {
            Response<string> response = new Response<string>();
            var errorDetails = await importLogData.GetImportSourceFileRow(importId);
            if (errorDetails == null)
            {
                response.Success = false;
                response.Message = "Row not found";
                return response;
            }
            if (errorDetails.ImportStatusId == 6)
            {
                response.Success = false;
                response.Message = "File is already imported";
                return response;
            }
            errorDetails.ImportStatusId = 10;
            int result = await importLogData.UpdateStaggingErrorRow();
            if (result > 0)
            {
                response.Success = true;
                response.Message = "Status field Updated successfully!";
            }

            return response;
        }

    }
   
}
