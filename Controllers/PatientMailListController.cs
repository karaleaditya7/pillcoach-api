using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Threading.Tasks;


namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientMailListController : ControllerBase
    {
        private readonly IPatientMailListService _patientMailListService;

        public PatientMailListController(IPatientMailListService patientMailListService)
        {
            _patientMailListService = patientMailListService;
        }
        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        public async Task<IActionResult> AddPatientMailList([FromBody] PatientMailListModel model)
        {
            var response = await _patientMailListService.AddPatientMailList(model);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllPatientMailLists([FromQuery] int recordNumber, [FromQuery] int pageLimit, [FromHeader] string authorization, [FromQuery] string keywords, [FromQuery] string sortDirection, [FromQuery] string filterType, [FromQuery] string filterValue, [FromQuery] string filterCategory)
        {
            var response = await _patientMailListService.GetPatientMailLists(recordNumber, pageLimit,authorization,keywords,sortDirection,filterType,filterValue,filterCategory);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee,SuperAdmin")]
        [HttpGet]
        [Route("{patientId}")]
        public async Task<IActionResult> GetPatientMailListForNearestCompletedDate(int patientId,string type)
        {
            var response = await _patientMailListService.GetPatientMailListForLatestCompletedDate(patientId,type);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("details/{id}")]
        public async Task<IActionResult> GetpatientMailListById(int id)
        {
            var response = await _patientMailListService.GetPatientMailListById(id);
            return Ok(response);
        }

        [Produces("application/json")]
        [Authorize(Roles = "Employee")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatientMailList([FromBody] PatientMailListModel model)
        {
            var response = await _patientMailListService.UpdatePatientMailList(model);
            return Ok(response);
        }
        
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePatientMailListById(int id)
        {
            var response = await _patientMailListService.DeletePatientMailListById(id);
            return Ok(response);
        }

    }
}
