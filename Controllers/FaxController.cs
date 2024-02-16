using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Assets;
using OntrackDb.Helper;
using OntrackDb.Model;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Authorize(Roles = "Admin, Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class FaxController : ControllerBase
    {
        private readonly IUtilityFax _utilityFax;
        private IConverter _converter;
        
        public FaxController(IUtilityFax utilityFax, IConverter converter)
        {
            _utilityFax = utilityFax;
            _converter = converter;
        }


        /// <summary>
        /// For Sending a fax. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Sending a fax.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> SendFax([FromBody] PdfModel model)
        {
            var sb = new StringBuilder();
            String htmlText = EmailTemplates.GetFaxTemplate(model);
            sb.Append(@htmlText);
            string filePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
            byte[] byteInfo = Convert.FromBase64String(model.ImageString);
            System.IO.FileStream stream = new FileStream(@filePath, FileMode.CreateNew);
            System.IO.BinaryWriter writer =
                new BinaryWriter(stream);
            writer.Write(byteInfo, 0, byteInfo.Length);
            writer.Close();

            var result = await _utilityFax.SendFax(model, filePath);
            return Ok(result);
        }

        /// <summary>
        /// For Adding description for a fax. Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Adding description for a fax.
        /// </remarks>
        [HttpPost]
        [Route("description")]
        public async Task<IActionResult> GetFaxDescriptionByIds([FromBody] PdfModel model)
        {
            
            var result = await _utilityFax.GetFaxDescriptionByIds(model);
            return Ok(result);
        }
    }
}
