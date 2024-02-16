using Azure;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OntrackDb.Authorization;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using OntrackDb.Service;
using System.IO;
using System;
using System.Threading.Tasks;
using Aspose.Pdf;


namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfStorageController : ControllerBase
    {
        private readonly IPdfStorageService _pdfStorageService;
        private readonly IJwtUtils _jwtUtils;
        private IConfiguration _configuration;
        private readonly IUserData _userData;
        private readonly IServiceTakeawayInformationService _serviceTakeawayInformationService;
        private readonly IServiceTakeAwayMedReconciliationService _serviceTakeAwayMedReconciliationService;
        public PdfStorageController(IPdfStorageService pdfStorageService, IJwtUtils jwtUtils, IUserData userData, IConfiguration configuration, IServiceTakeawayInformationService serviceTakeawayInformationService, IServiceTakeAwayMedReconciliationService serviceTakeAwayMedReconciliationService)
        {
            _pdfStorageService = pdfStorageService;
            _jwtUtils = jwtUtils;
            _userData = userData;
            _configuration = configuration;
            _serviceTakeawayInformationService = serviceTakeawayInformationService;
            _serviceTakeAwayMedReconciliationService = serviceTakeAwayMedReconciliationService;
        }

        [Authorize(Roles = "Employee")]
        [Route("upload")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> Upload(PdfStorageModel model)
        {
            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
          if(!string.IsNullOrEmpty(model.PdfString))
            {
                var pdfName = await _pdfStorageService.UploadPDFForCmr(model);


                return Ok(pdfName);
            }

            response.Success = false;
            response.Message = "pdf Not Found";
            return Ok(response);
        }


        [Authorize(Roles = "Employee")]
        [Route("upload/MedRec")]
        [Produces("application/json")]
        [HttpPost]
        public async Task<IActionResult> UploadPdfForMedRec(PdfStorageModel model)
        {
            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            if (!string.IsNullOrEmpty(model.PdfString))
            {
                var pdfName = await _pdfStorageService.UploadPDFForMedRec(model);


                return Ok(pdfName);
            }

            response.Success = false;
            response.Message = "pdf Not Found";
            return Ok(response);
        }

        //  [Authorize(Roles = "Employee")]
        [Route("convert")]
        [HttpPost]
        public  IActionResult Converted(IFormFile form)
        {
            PdfStorageModel model = new PdfStorageModel();
            IFormFile file = Request.Form.Files[0];
            var ms = new MemoryStream();
            file.CopyTo(ms);
            var fileBytes = ms.ToArray();
            model.PdfString = Convert.ToBase64String((fileBytes));
            OntrackDb.Dto.Response<Response<BlobContentInfo>> response = new OntrackDb.Dto.Response<Response<BlobContentInfo>>();
            if (!string.IsNullOrEmpty(model.PdfString))
            {
                string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                string filePathNew = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                byte[] byteInfo = Convert.FromBase64String(model.PdfString);
                System.IO.FileStream stream = new FileStream(@filePath, FileMode.CreateNew);
                System.IO.BinaryWriter writer =
                    new BinaryWriter(stream);
                writer.Write(byteInfo, 0, byteInfo.Length);
                writer.Close();
                int count = 0;
                Document doc = new Document(filePath);
                
                foreach (Aspose.Pdf.Page page in doc.Pages)
                {

                    count++;

                    // Setting Rotation angle of page
                    if(count >= 2)
                    {
                        page.Rotate = Rotation.on270;
                    }
                    
                }
                doc.Save(filePathNew);
                System.IO.File.Delete(filePath);
                var fileStream = System.IO.File.OpenRead(filePathNew);
                return File(fileStream, "application/pdf", "Grid.pdf");

            }

            response.Success = false;
            response.Message = "pdf Not Found";
            return Ok(response);
        }

        [Authorize(Roles = "Employee")]
        [Route("get")]
        [HttpGet]
        public async Task<string> GetVerificationLinkForServiceTakeawayCmr([FromQuery] int id)
        {
           var takeawayVerify = await _serviceTakeawayInformationService.GetTakeawayVerifyByPatientId(id);
            string url = _configuration["VerifyUrl"] + "/" + id + "?uuid=" + takeawayVerify.Data.UUID;
            return url;
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [Route("get/preview")]
        [HttpGet]
        public async Task<string> GetPreviewPDFForCmr([FromQuery] int id)
        {
            var pdfBytes = await _pdfStorageService.GetPDFURIForCmr(id);
           var baseString = Convert.ToBase64String((pdfBytes));
            return baseString;
        }

        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [Route("get/medRec/preview")]
        [HttpGet]
        public async Task<string> GetPreviewPDFForMedRec([FromQuery] int id)
        {
            var pdfBytes = await _pdfStorageService.GetPDFURIForMedRec(id);
            var baseString = Convert.ToBase64String((pdfBytes));
            return baseString;
        }

        [Authorize(Roles = "Employee")]
        [Route("get/MedReconciliation")]

        [HttpGet]
        public async Task<string> GetVerificationLinkForServiceTakeawayMedRec([FromQuery] int id)
        {
            var takeawayVerify = await _serviceTakeAwayMedReconciliationService.GetTakeawayVerifyForMedRecByPatientId(id);
            string url = _configuration["VerifyUrl"] + "/" + id + "?uuid=" + takeawayVerify.Data.UUID + "?flag=" + takeawayVerify.Data.IsServiceTakeAwayMedRec;

            return url;
        }
    }
}
