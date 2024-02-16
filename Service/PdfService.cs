using EllipticCurve.Utils;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class PdfService : IPdfService
    { 
         private readonly IPatientData _patientData;
    
        public PdfService(IPatientData patientData)
        {
        _patientData = patientData;
        }
        public async Task<byte[]>  EncryptionAndDecryptionOfPDF(IFormFile file , int patientId)
        {
            byte[] bytes;
            var patient = await _patientData.GetPatientById(patientId);
            DateTime date = patient.Contact.DoB;
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;   
            using (MemoryStream inputData = new MemoryStream())
            {
                file.CopyTo(inputData);
                var fileBytes = inputData.ToArray();
                using (MemoryStream outputData = new MemoryStream())
                {
                    string PDFFilepassword = patient.Contact.FirstName.Substring(0,4).ToUpper() + day + "/" + month + "/" + year ;
                    PdfReader reader = new PdfReader(fileBytes);
                    PdfReader.unethicalreading = true;
                    PdfEncryptor.Encrypt(reader, outputData, true, PDFFilepassword, PDFFilepassword, PdfWriter.ALLOW_SCREENREADERS);
                     bytes = outputData.ToArray();

                }
                
            }
            return bytes;
        }
    }
}
