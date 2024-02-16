using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Authorize(Roles = "Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
       

        public AppointmentController(IAppointmentService appointmentService)
        {
          _appointmentService = appointmentService;
        }


        /// <summary>
        /// Creating a New Appointment . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Creating a Appointment.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentModel model)
        {
            var response = await _appointmentService.AddAppointment(model);

            return Ok(response);
        }
        /// <summary>
        /// This is a Delete Api . Role[Admin]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Delete a Appointment By AppointmentId.
        /// </remarks>
        [HttpDelete]
        [Route("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            var response = await _appointmentService.DeleteAppointmentById(appointmentId);

            return Ok(response);
        }
        /// <summary>
        /// Update a Appointment Status. Role[Admin]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for getting Updating a Appointment Status.
        /// </remarks>
        [HttpPut]
        public async Task<IActionResult> UpdateAppointment(AppointmentModel model)
        {
            var response = await _appointmentService.UpdateAppointment(model);

            return Ok(response);
        }
        /// <summary>
        /// Get All Appointments  Events (List). Role[Admin]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting list of all Events(Appointment).
        /// </remarks>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllAppointments()
        {
                var response = await _appointmentService.GetAppointments();
                return Ok(response);
            
        }

        /// <summary>
        /// Get Appointment By UserId . Role[Admin]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting a Appointment by UserId.
        /// </remarks>
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        [Route("User/{userId}")]
        public async Task<IActionResult> GetAllAppointmentsByUserId(string userId,[FromQuery]int month, [FromQuery]int year)
        {
            var response = await _appointmentService.GetAppointmentsByUserId(userId,month,year);
            return Ok(response);

        }

        /// <summary>
        /// Get Appointment with Id. Role[Admin]
        /// </summary>
        /// <remarks>
        /// This API Will be used for getting Appointment By a AppointmentId.
        /// </remarks>
        [HttpGet]
        [Route("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentByAppointmentId(int appointmentId)
        {
            var response = await _appointmentService.GetAppointmentByAppointmentId(appointmentId);
            return Ok(response);

        }

        /// <summary>
        /// Get Appointment By PatientId . Role[Admin]
        /// </summary>
        /// <remarks>
        /// For Admin: This API Will be used for getting a Appointment by PatientId.
        /// </remarks>
        [HttpGet]
        [Route("Patient/{patientId}")]
        public async Task<IActionResult> GetAllAppointmentsByPatientId(int patientId)
        {
            var response = await _appointmentService.GetAppointmentsByPatientId(patientId);
            return Ok(response);

        }

        [HttpGet]
        [Route("Schedule/{userId}")]
        public async Task<IActionResult> GetAllAppointmentsByPatientId(string userId)
        {
            var response = await _appointmentService.GetScheduledAppointmentsByuserId(userId);
            return Ok(response);

        }
    }
}
