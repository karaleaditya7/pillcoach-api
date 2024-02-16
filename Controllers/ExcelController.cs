using ExcelDataReader;
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



namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment Environment;
        private IExcelService _excelService;
        private readonly ApplicationDbContext _applicationDbcontext;
        public ExcelController(IConfiguration configuration, IWebHostEnvironment environment, IExcelService excelService, ApplicationDbContext applicationDbcontext)
        {
            _configuration = configuration;
            Environment = environment;
            _excelService = excelService;
            _applicationDbcontext = applicationDbcontext;
        }


        [Route("ReadFile")]
        [HttpPost]
        public IActionResult Index(IFormFile form)
        {
            string message = "";
            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader dataReader = null;
            IFormFile file = Request.Form.Files[0];
         
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            using (Stream stream = file.OpenReadStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    if (form != null && stream != null)
                    {
                        if (form.FileName.EndsWith(".xls"))
                            dataReader = ExcelReaderFactory.CreateBinaryReader(stream);
                        else if (form.FileName.EndsWith(".xlsx"))
                            dataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        else
                        {
                            message = "The file format is not supported.";
                            return Ok(message);
                        }
                           

                        dsexcelRecords = dataReader.AsDataSet();
                        dataReader.Close();

                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            _excelService.InsertIntoDBForExcelDataReader(dsexcelRecords);
                        }
                        else
                        {
                            message = "Selected file is empty.";
                            return Ok(message);
                        }
                           
                    }
                }
            }

           
            return Ok();
        }


       

    }
}
