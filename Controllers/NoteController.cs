using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public NoteController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Update Note Role[Admin, Employee]
        /// </summary>
        /// <remarks>
        /// This API Will be used for updating Note by note id.
        /// </remarks>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateNote([FromBody] NoteModel model)
        {
            var result = await _patientService.UpdateNote(model);
            return Ok(result);
        }
    }
}
