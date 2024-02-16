using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Service;
using System.Data;
using System.IO;
using System.Threading.Tasks;
[Route("api/[controller]")]
[ApiController]
public class PrimaryThirdPartyController : ControllerBase
{
    private readonly IPrimaryThirdPartyService _primaryThirdPartyService;

    public PrimaryThirdPartyController(IPrimaryThirdPartyService primaryThirdPartyService)
    {
        _primaryThirdPartyService = primaryThirdPartyService;
    }

    [Route("upload")]
    [HttpPost]
    public async Task<IActionResult> ImportDataFromExcel(IFormFile form)
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
                        _primaryThirdPartyService.InsertIntoDBForPlanDataReader(dsexcelRecords);
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

    /// <summary>
    /// Get All Plans 
    /// </summary>
    [Produces("application/json")]
    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllPlans([FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromQuery] string keywords)
    {
        var response = await _primaryThirdPartyService.GetAllPlans(recordNumber,pageLimit,keywords);
        return Ok(response);
    }
}