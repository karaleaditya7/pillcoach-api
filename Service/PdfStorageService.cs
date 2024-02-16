using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using OntrackDb.Authorization;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class PdfStorageService: IPdfStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IPatientData _patientData;
        private readonly IJwtUtils _jwtUtils;
        public PdfStorageService(BlobServiceClient blobServiceClient, IJwtUtils jwtUtils, IPatientData patientData)
        {
            _blobServiceClient = blobServiceClient;

            _jwtUtils = jwtUtils;
            _patientData = patientData; 

        }

        public async Task<string> UploadPDFForCmr(PdfStorageModel model)
        {

            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("cmrpdf");

            byte[] byteInfo = Convert.FromBase64String(model.PdfString);
            

            Stream stream = new MemoryStream(byteInfo);

            var patient = await _patientData.GetPatientById(model.PatientId);
            string  pdfUrl = await GetPDFUrlByPatientIdForCmr(patient.Id);


            if (!string.IsNullOrEmpty(pdfUrl))
            {


                //string pdfName = String.Format("{0}{1}.pdf", patient.Contact.FirstName, patient.Contact.LastName);
                string pdfName = String.Format("{0}.pdf", patient.Id);
                var file = new FormFile(stream, 0, byteInfo.Length, "", pdfName);

                var blobClient = blobContainer.GetBlobClient(pdfName);
                await blobClient.DeleteIfExistsAsync();
                try
                {
                    var result = await blobClient.UploadAsync(file.OpenReadStream());
                }
                catch (Exception ex)
                {
                    return "Error Occured While Uplaoding Image :" + ex.Message;
                }

                return pdfName;
            }
            else
            {
                //string pdfName = String.Format("{0}{1}.pdf", patient.Contact.FirstName, patient.Contact.LastName);
                string pdfName = String.Format("{0}.pdf", patient.Id);
                var file = new FormFile(stream, 0, byteInfo.Length, "", pdfName);
                var blobClient = blobContainer.GetBlobClient(pdfName);
                try
                {
                    var result = await blobClient.UploadAsync(file.OpenReadStream());
                }
                catch (Exception ex)
                {
                    return "Error Occured While Uplaoding Image :" + ex.Message;
                }
                return pdfName;
            }


        }

        public async Task<string> UploadPDFForMedRec(PdfStorageModel model)
        {

            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("medrecpdf");

            byte[] byteInfo = Convert.FromBase64String(model.PdfString);


            Stream stream = new MemoryStream(byteInfo);

            var patient = await _patientData.GetPatientById(model.PatientId);
            string pdfUrl = await GetPDFUrlByPatientIdForMedRec(patient.Id);


            if (!string.IsNullOrEmpty(pdfUrl))
            {


                //string pdfName = String.Format("{0}{1}.pdf", patient.Contact.FirstName, patient.Contact.LastName);
                string pdfName = String.Format("{0}.pdf", patient.Id);
                var file = new FormFile(stream, 0, byteInfo.Length, "", pdfName);

                var blobClient = blobContainer.GetBlobClient(pdfName);
                await blobClient.DeleteIfExistsAsync();
                try
                {
                    var result = await blobClient.UploadAsync(file.OpenReadStream());
                }
                catch (Exception ex)
                {
                    return "Error Occured While Uplaoding Image :" + ex.Message;
                }

                return pdfName;
            }
            else
            {
                //string pdfName = String.Format("{0}{1}.pdf", patient.Contact.FirstName, patient.Contact.LastName);
                string pdfName = String.Format("{0}.pdf", patient.Id);
                var file = new FormFile(stream, 0, byteInfo.Length, "", pdfName);
                var blobClient = blobContainer.GetBlobClient(pdfName);
                try
                {
                    var result = await blobClient.UploadAsync(file.OpenReadStream());
                }
                catch (Exception ex)
                {
                    return "Error Occured While Uplaoding Image :" + ex.Message;
                }
                return pdfName;
            }


        }


        public  async Task<byte[]> GetPDFURIForCmr(int  id)
        {
            var patient = await _patientData.GetPatientById(id);
           
                var containerClient = _blobServiceClient.GetBlobContainerClient("cmrpdf");
                if (containerClient.CanGenerateSasUri)
                {

                    BlobSasBuilder sasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = containerClient.Name,
                        Resource = "b",

                    };

                    sasBuilder.StartsOn = DateTimeOffset.UtcNow;

                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(100);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);


                    Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                    string sasUrl = sasUri.ToString();
                    string[] arr = sasUrl.Split("?");
                    //string url = arr[0] + "/" + patient.Contact.FirstName + patient.Contact.LastName+".pdf" + "?" + arr[1];
                    string url = arr[0] + "/" + patient.Id + ".pdf" + "?" + arr[1];
                    Console.WriteLine("SAS URI for blob container is: {0}", url);
                    Console.WriteLine();
                    using var httpClient = new HttpClient();
                    {
                        byte[] data = await httpClient.GetByteArrayAsync(url);
                        return data;
                    }



                }

            return null;


        }


        public async Task<byte[]> GetPDFURIForMedRec(int id)
        {
            var patient = await _patientData.GetPatientById(id);

            var containerClient = _blobServiceClient.GetBlobContainerClient("medrecpdf");
            if (containerClient.CanGenerateSasUri)
            {

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "b",

                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow;

                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(100);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);


                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                string sasUrl = sasUri.ToString();
                string[] arr = sasUrl.Split("?");
                //string url = arr[0] + "/" + patient.Contact.FirstName + patient.Contact.LastName + ".pdf" + "?" + arr[1];
                string url = arr[0] + "/" + patient.Id + ".pdf" + "?" + arr[1];
                Console.WriteLine("SAS URI for blob container is: {0}", url);
                Console.WriteLine();
                //using (var webClient = new WebClient())
                //{
                //    return webClient.DownloadData(url);
                //}
                using var httpClient = new HttpClient();
                {
                    byte[] data = await httpClient.GetByteArrayAsync(url);
                    return data;
                }

            }

            return null;


        }



        public async Task<string> GetPDFUrlByPatientIdForCmr(int id)
        {
            var patient = await _patientData.GetPatientById(id);

            var containerClient = _blobServiceClient.GetBlobContainerClient("cmrpdf");
            if (containerClient.CanGenerateSasUri)
            {

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "b",

                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow;

                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(72);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);


                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                string sasUrl = sasUri.ToString();
                string[] arr = sasUrl.Split("?");
                string url = arr[0] + "/" + patient.Contact.FirstName + patient.Contact.LastName + ".pdf" + "?" + arr[1];
                Console.WriteLine("SAS URI for blob container is: {0}", url);
                Console.WriteLine();

                return url;
            }

            return null;


        }

        public async Task<string> GetPDFUrlByPatientIdForMedRec(int id)
        {
            var patient = await _patientData.GetPatientById(id);

            var containerClient = _blobServiceClient.GetBlobContainerClient("medrecpdf");
            if (containerClient.CanGenerateSasUri)
            {

                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = containerClient.Name,
                    Resource = "b",

                };

                sasBuilder.StartsOn = DateTimeOffset.UtcNow;

                sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(72);
                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);


                Uri sasUri = containerClient.GenerateSasUri(sasBuilder);
                string sasUrl = sasUri.ToString();
                string[] arr = sasUrl.Split("?");
                string url = arr[0] + "/" + patient.Contact.FirstName + patient.Contact.LastName + ".pdf" + "?" + arr[1];
                Console.WriteLine("SAS URI for blob container is: {0}", url);
                Console.WriteLine();

                return url;
            }

            return null;


        }

    }
}
