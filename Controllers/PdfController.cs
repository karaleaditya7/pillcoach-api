using ExcelDataReader;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OntrackDb.Repositories;
using OntrackDb.Enums;
using Microsoft.AspNetCore.Authorization;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {

        private readonly IPdfService _pdfService;
       
        
        public PdfController(IPdfService pdfService )
        {
            _pdfService = pdfService;
            
        }

        [Authorize(Roles = "Admin, Employee")]
        [Route("password")]
        [HttpPost]
        public async Task<IActionResult> EncryptionAndDecryptionOfPDF(IFormFile form , int patientId)
        {
            IFormFile file = Request.Form.Files[0];
            
            byte[] bytes = await _pdfService.EncryptionAndDecryptionOfPDF(file , patientId);
            Response.ContentType = "application/pdf";
            Response.Headers.Add("content-disposition", "attachment;filename=Reports.pdf");
            return File(bytes, "application/pdf");

        }
    }     
}


    
