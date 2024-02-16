using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    // [Authorize(Roles = "Admin,Employee")]

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        /// <summary>
        /// Updating a notification for a appointment. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for updating a notification for appointment.
        /// </remarks>
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateNotification([FromBody] List<string> notificationIds, [FromHeader] string authorization)
        {
            var response = await _notificationService.UpdateNotification(notificationIds, authorization);

            return Ok(response);
        }


        /// <summary>
        /// Get all notification for a userId. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Get all notifications for a userId.
        /// </remarks>
        [Authorize]
        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpGet]
        [Route("all/{userId}")]
        public async Task<IActionResult> GetAllNotificationsForUser(string userId,[FromHeader] string authorization)
        {
            var response = await _notificationService.GetAllNotificationsForUser(userId,authorization);

            return Ok(response);
        }

        /// <summary>
        /// Delete a Notifications. Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Delete a notification.
        /// </remarks>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteNotification([FromBody] List<string> notificationIds, [FromHeader] string authorization)
        {
            await _notificationService.DeleteNotification(notificationIds, authorization);

            return Ok();
        }

       
    }
}
