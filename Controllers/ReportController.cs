
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Context;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        
        private readonly IReportService _reportService;
        private readonly IEmailService _emailService;
        public ReportController(IReportService reportService, IEmailService emailService)
        {
           
            _reportService = reportService;
            _emailService = emailService;
        }

        /// <summary>
        /// Get all Pharmacies CSV Data. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting pharmacies data in csv file.
        /// </remarks>
        [HttpPost]
 
        public async Task<IActionResult> GetPharmacyCSVListData(List <string> pharmacyIds,[FromQuery] int month)
        {
            var response = await _reportService.GetPharmacyCSVData(pharmacyIds,month);
            return  File(Encoding.ASCII.GetBytes(response), "text/csv", "Reports.csv");

        }

        /// <summary>
        /// Get Pharmacy preview Data for a userId. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting Pharmacy preview Data for a userId.
        /// </remarks>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("pharmacy")]
        public async Task<IActionResult> GetPharmacyPreviewDataForUserId(List<string> pharmacyIds, [FromQuery] int month)
        {
            var response = await _reportService.GetPharmacyListByPharmacyIdsAndUserId(pharmacyIds, month);
            return Ok(response);

        }


        /// <summary>
        /// To send a mail for pharmacy email for detail report. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used To send a mail for pharmacy email for detail report.
        /// </remarks>
        [HttpPost]
        [Route("sendmail")]
        public async Task<IActionResult> sendReportsOnEmail(List<string> pharmacyIds, [FromQuery] int month, [FromQuery] string email)
        {
            var response = await _reportService.GetPharmacyCSVData(pharmacyIds, month);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(response));
            IFormFile file = new FormFile(stream, 0, Encoding.ASCII.GetBytes(response).Length, "Reports.csv", "Reports.csv");
            
          
            await _emailService.SendReportsOnEmail(file, email);
            return Ok();

        }

    }


}
