using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Service;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SafetyDisposalController : ControllerBase
    {
        private readonly ISafetyDisposalService _safetyDisposalService;
        public SafetyDisposalController(ISafetyDisposalService safetyDisposalService)
        {
            _safetyDisposalService = safetyDisposalService; 
        }

        [Route("TextFile")]
        [HttpPost]
        public  IActionResult TextFileReader(IFormFile form)
        {
            IFormFile file = Request.Form.Files[0];
            _safetyDisposalService.InsertIntoDBForSafetyDisposalFromTextFile(file);
            return Ok();

        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("{zipCode}")]
        public async Task<IActionResult> GetSafetyDisposalByZipCode(string zipCode,string username)
        {
            var response = await _safetyDisposalService.GetSafetyDisposalByZipCode(zipCode,username);
            return Ok(response);
        }


        //[Produces("application/json")]
        //[Authorize(Roles = "Admin,Employee")]
        //[HttpGet]
        //[Route("NearByAddress/{zipCode}")]
        //public async Task<IActionResult> GetNearByAddressByZipCode(string zipCode,string username)
        //{
        //    var response = await _safetyDisposalService.GetNearByAddressByZipCode(zipCode,username);
        //    return Ok(response);
        //}
    }
}
