using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Model;
using OntrackDb.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {

        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }


        /// <summary>
        /// Getting all messgaes . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting all messages.
        /// </remarks>
        
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllMessages()
        {
            var response = await _messageService.GetAllMessages();
            return Ok(response);
        }


        /// <summary>
        /// Getting all messgaes . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting all messages for a sender.
        /// </remarks>
        [HttpGet]
        [Route("{userId}/all")]
        public async Task<IActionResult> GetAllMessagesforUserId(string userId)
        {
            var response = await _messageService.GetAllMessagesforUser(userId);
            return Ok(response);
        }


        [HttpGet]
        [Route("unReadMessages/all")]
        public async Task<IActionResult> GetUnreadAllMessagesforUser([FromHeader]string authorization)
        {
            var response = await _messageService.GetUnreadMessagesForUser(authorization);
            return Ok(response);
        }


        [HttpGet]
        [Route("{userId}/search")]
        public async Task<IActionResult> GetAllMessagesFromSearch(string userId, [FromQuery]string text)
        {
            var response = await _messageService.GetAllMessageFromSearch(userId, text);
            return Ok(response);
        }


        [HttpGet]
        [Route("{userId}/count")]
        public async Task<IActionResult> GetAllMessagesCountForUser(string fromUserId, string toUserId)
        {
            var response = await _messageService.CountforMessage(fromUserId, toUserId);
            return Ok(response);
        }


        /// <summary>
        /// Updating status for a read message . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API is used to update a status of a read message through its message id.
        /// </remarks>
        [HttpPut]
        [Route("read")]
        public async Task<IActionResult> ReadUnreadMessageUpdate(List<string> messageIds)
        {
            var response = await _messageService.UpdateReadDateTimeForMessage(messageIds);
            return Ok(response);
        }


        /// <summary>
        /// Getting all messgaes . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting all messages between two user.
        /// </remarks>
        [HttpGet]
        [Route("history/{toUserId}")]
        public async Task<IActionResult> GetAllMessagesforUser([FromHeader] string authorization, string toUserId)
        {
            var response = await _messageService.GetMessageHistory(authorization,toUserId);
            return Ok(response);
        }



        //public async Task<IActionResult> GetAllMessages()
        //{
        //    var response = await _messageService.GetAllMessages();
        //    return Ok(response);
        //}

        /// <summary>
        /// Get a count of new message . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Counting a new messages.
        /// </remarks>
        //Implementation of socket.io is remaining for actual new message count.
        [HttpGet]
        [Route("newMessage/count")]
        public async Task<IActionResult> GetNewMessageCount(string userId)
        {
            var response = await _messageService.GetMessagesUnreadCount(userId);
            return Ok(response);
        }

        /// <summary>
        /// Creating a New Message . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Creating a Message.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> sendMessage([FromBody] List<MessageModel> models)
        {
            var response = await _messageService.SendMessage(models);
            return Ok(response);
        }

        /// <summary>
        /// Get all users for related pharmacy . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting all users for related pharmacy.
        /// </remarks>
        [HttpGet]
        [Route("Users/{userId}")]
        public async Task<IActionResult> GetAllUserForSamePharmacy([FromHeader]string authorization, string userId)
        {
            var response = await _messageService.GetAllUserForSamePharmacy(authorization,userId);
            return Ok(response);
        }

        /// <summary>
        /// Get all Messages for sender user . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for get all messgae for sender user.
        /// </remarks>
        [HttpGet]
        [Route("Sender/{userId}")]
        public async Task<IActionResult> GetAllMessagesBySenderUserId(string userId)
        {
            var response = await _messageService.GetMessagesBySenderId(userId);
            return Ok(response);

        }
        /// <summary>
        /// Get all messgaes for recipent user . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for get all messages foir recipent user.
        /// </remarks>
        [HttpGet]
        [Route("Recipient/{userId}")]
        public async Task<IActionResult> GetAllMessagesByRecipientUserId(string userId)
        {
            var response = await _messageService.GetMessagesByRecipientId(userId);
            return Ok(response);

        }


        /// <summary>
        /// Delete all messgaes . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Delete all message.
        /// </remarks>
        [HttpDelete]
        [Route("{toUserId}")]
        public async Task<IActionResult> DeleteMessage([FromHeader] string authorization, string toUserId)
        {
           await _messageService.DeleteMessageWithUserId(authorization,toUserId);
            return Ok();

        }

        /// <summary>
        /// Getting ConnectionId . Role[Admin,Employee]
        /// </summary>
        /// <remarks>
        ///  This API Will be used for Getting the connection Id by userId.
        /// </remarks>
        [HttpGet]
        [Route("{userId}/connection")]
        public async Task<IActionResult> GetConnectIdByUserId(string userId)
        {
            var response = await _messageService.GetConnectIdByUserId(userId);
            return Ok(response);
        }

    }
}
