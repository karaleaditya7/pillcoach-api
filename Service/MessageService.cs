using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Hub;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageData _messageData;
        private readonly IUserData _userData;
        private readonly IJwtUtils _jwtUtils;
        private readonly IImageService _imageService;
        private readonly INotificationData _notificationData;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly IHubContext<ChatHub> _hubContext;

        public MessageService(IMessageData messageData, IUserData userData, IJwtUtils jwtUtils, IImageService imageService, INotificationData notificationData, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IHubContext<ChatHub> hubContext)
        {
            _messageData = messageData;
            _userData = userData;
            _jwtUtils = jwtUtils;
            _imageService = imageService;
            _notificationData = notificationData;
            _userManager = userManager;
            _roleManager = roleManager;
            _hubContext = hubContext;
        }

        public async Task<Response<Message>> GetAllMessages()
        {
            Response<Message> response = new Response<Message>();
            var messages = await _messageData.GetAllMessages();

            if (messages == null)
            {
                response.Success = false;
                response.Message = "Messages Not Found";
                return response;
            }
            response.Success = true;
            response.DataList = messages;
            return response;
        }



        public async Task<List<MessageUserModel>> GetAllMessagesforUser(string userId)
        {
                
            List<Message> messageList = await _messageData.GetMessagesBySenderUserId(userId);
            List<Message> messageListForSendDateFormat = new List<Message>();
            foreach (Message msg in messageList)
            {
                msg.StrSendDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", msg.SentDateTime);

                //if ((msg.FromUser.Id == userId && msg.FromDeleted == false) || (msg.ToUser.Id == userId && msg.ToDeleted == false))
                //{
                //    messageListForSendDateFormat.Add(msg);
                //}

                messageListForSendDateFormat.Add(msg);

                if (msg.ToUser.Id == userId)
                {
                    msg.ToUser = msg.FromUser;
                }

               

            }
            var messageListForuser = messageListForSendDateFormat.GroupBy(m=>m.ToUser).Select(m=>m.LastOrDefault()).OrderByDescending(m=>m.SentDateTime).ToList();
            List<MessageUserModel> messages = new List<MessageUserModel>();
            foreach (Message message in messageListForuser)
            {
              
                MessageUserModel messageModel = new MessageUserModel();
                message.ToUser.ImageUri = _imageService.GetImageURI(message.ToUser.ImageName);
                messageModel.User = message.ToUser;
                messageModel.MessageBody = message.MessageText;
                messageModel.SendDateTime = message.SentDateTime;
                messageModel.UnReadMessageCount =await _messageData.GetMessagesCountForUser(userId, message.ToUser.Id);
              

                messages.Add(messageModel);
            }
           
            return messages;
        }


        public async Task<List<MessageUserModel>> GetAllMessageFromSearch(string userId,string search)
        {
            List<Message> messageList = await _messageData.GetMessagesBySenderUserId(userId);
            List<Message> messageListForSendDateFormat = new List<Message>();
            foreach (Message msg in messageList)
            {
                msg.StrSendDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", msg.SentDateTime);

                //if ((msg.FromUser.Id == userId && msg.FromDeleted == false) || (msg.ToUser.Id == userId && msg.ToDeleted == false))
                //{
                //    if(msg.MessageText.ToLower().Contains(search.ToLower()))
                //    {
                //        messageListForSendDateFormat.Add(msg);
                //    }

                //}

                if (msg.MessageText.ToLower().Contains(search.ToLower()))
                {
                    messageListForSendDateFormat.Add(msg);
                }

                if (msg.ToUser.Id == userId)
                {
                    msg.ToUser = msg.FromUser;
                }



            }
            var messageListForuser = messageListForSendDateFormat.GroupBy(m => m.ToUser).Select(m => m.LastOrDefault()).OrderByDescending(m => m.SentDateTime).ToList();
            List<MessageUserModel> messages = new List<MessageUserModel>();
            foreach (Message message in messageListForuser)
            {

                MessageUserModel messageModel = new MessageUserModel();
                message.ToUser.ImageUri = _imageService.GetImageURI(message.ToUser.ImageName);
                messageModel.User = message.ToUser;
                messageModel.MessageBody = message.MessageText;
                messageModel.SendDateTime = message.SentDateTime;
                messageModel.UnReadMessageCount = await _messageData.GetMessagesCountForUser(userId, message.ToUser.Id);
                

                messages.Add(messageModel);
            }

            return messages;
        }



        public async Task<Response<Message>> SendMessage(List<MessageModel> models)
        {
            Response<Message> response = new Response<Message>();

            List<Message> MessageList = new List<Message>();

            foreach(MessageModel messageModel in models)
            {
                if (string.IsNullOrEmpty(messageModel.FromUserId))
                {
                    response.Message = "Sender userId is Missing";
                    return response;
                }

                if (string.IsNullOrEmpty(messageModel.ToUserId))
                {
                    response.Message = "Recipient userId missing";
                    return response;
                }

                if (string.IsNullOrEmpty(messageModel.MessageText))
                {
                    response.Message = "MessageText is missing";
                    return response;
                }


                User fromUser = await _userData.GetUserById(messageModel.FromUserId);
                if (fromUser == null)
                {
                    response.Message = "Sender User Not Found";
                    return response;
                }
                User toUser = await _userData.GetUserById(messageModel.ToUserId);
                if (toUser == null)
                {
                    response.Message = "Recipient User Not Found";
                    return response;
                }

                Message message = new Message
                {
                    FromUser = fromUser,
                    ToUser = toUser,
                    MessageText = messageModel.MessageText,
                    SentStatus = messageModel.SentStatus,
                    Status = messageModel.Status,
                    Type = messageModel.Type,
                    SentDateTime = DateTime.Now,

                };
                var result = await _messageData.SendMessage(message);
                MessageList.Add(message);
  
                
                if (! await _userManager.IsInRoleAsync(message.ToUser, "Admin")) 
                { 
                    Notification notification = new Notification
                    {

                        User = message.ToUser,
                        Message = message,  
                        NotificationType = "Message",
                        SendDateTime = DateTime.Now,
                        Status = "Success"
                    };
                    var notiResult = await _notificationData.AddNotification(notification);

                    try
                    {
                        await _hubContext.Clients.Group(message.ToUser.Id).SendAsync("SendNotification", notification);
                    }
                    catch { }
                }
                else
                {
                    AdminNotification adminNotification = new AdminNotification
                    {

                        User = message.ToUser,
                        Message = message,
                        NotificationType = "Message",
                        SendDateTime = DateTime.Now,
                        Status = "Success"
                    };
                    var notiResult = await _notificationData.AddAdminNotification(adminNotification);

                    try
                    {
                        await _hubContext.Clients.Group(message.ToUser.Id).SendAsync("SendNotification", adminNotification);
                    }
                    catch { }
                }
            }


            if (MessageList == null)
            {
                response.Success = false;
                response.Message = "Error while Sending Message";
                return response;
            }

            response.Success = true;
            response.DataList = MessageList;
            response.Message = "Messages sent successfully!";
            return response;
        }

        public async Task<Response<WebSocket>> GetConnectIdByUserId(string userId)
        {
            Response<WebSocket> response = new Response<WebSocket>();
            var websocket = await _userData.GetWebSocketByUser(userId);
            if (websocket == null)
            {
                response.Success = false;
                response.Message = "websocket Not Found for UserId";
                return response;
            }
            response.Success = true;
            response.Data = websocket;
            return response;
        }


        public async Task<Response<Message>> GetMessagesBySenderId(string userId)
        {
            Response<Message> response = new Response<Message>();
            var messgaes = await _messageData.GetMessagesBySenderUserId(userId);
            if (messgaes == null)
            {
                response.Success = false;
                response.Message = "messgaes Not Found for Sender";
                return response;
            }
            response.Success = true;
            response.DataList = messgaes;
            return response;
        }

        public async Task<Response<User>> GetAllUserForSamePharmacy(string authorization, string userId)
        {
            Response<User> response = new Response<User>();
            var pharmacies = await _messageData.GetAllPharmacyByUserId(userId);

            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role = _jwtUtils.GetRole(parameter);
            string adminUserId =  _jwtUtils.ValidateToken(parameter);

            if (role == Roles.SuperAdmin.ToString())
            {

                var usersListForAdmin = await _userData.GetAllUsers();

                User userAdminItSelf = await _userData.GetUserById(adminUserId);

                usersListForAdmin.Remove(userAdminItSelf);

                if (usersListForAdmin == null)
                {
                    response.Success = false;
                    response.Message = "Users Not Found";
                    return response;
                }


                response.Success = true;
                response.DataList = usersListForAdmin;
                return response;


            }

            List<User> users =new List<User>();

            foreach (Pharmacy phamacy in pharmacies)
            {
                var pharmacyusers =await _messageData.GetAllUserForSamePharmacyByUserId(phamacy.Id);
                foreach  (User user in pharmacyusers)
                {
                    user.ImageUri = _imageService.GetImageURI(user.ImageName);
                    users.Add(user);
                }
            }

            var usersofpharmacy= users.GroupBy(x => x.Email).Select(y => y.First()).ToList();

            User userItself = await _userData.GetUserById(userId);

            usersofpharmacy.Remove(userItself);

            if (pharmacies == null)
            {
                response.Success = false;
                response.Message = "PharmacyUsers Not Found for Sender";
                return response;
            }
            response.Success = true;
            response.DataList = usersofpharmacy;
            return response;
        }

        public async Task<Response<Message>> GetMessagesByRecipientId(string userId)
        {
            Response<Message> response = new Response<Message>();
            var messgaes = await _messageData.GetMessagesByRecipientUserId(userId);
            if (messgaes == null)
            {
                response.Success = false;
                response.Message = "messgaes Not Found for Recipient";
                return response;
            }
            response.Success = true;
            response.DataList = messgaes;
            return response;
        }

        public async Task<Response<Message>> GetMessageHistory(string authorizatioin, string toUserId)
        {
            Response<Message> response = new Response<Message>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string fromUserId = _jwtUtils.ValidateToken(parameter);
            var messages = await _messageData.GetMessagesHistory(fromUserId,toUserId);

            
            if (messages == null)
            {
                response.Success = false;
                response.Message = "messgaes Not Found";
                return response;
            }
            response.Success = true;
            response.DataList = messages;
            return response;
        }


        public async Task<Response<Message>> GetUnreadMessagesForUser(string authorizatioin)
        {
            Response<Message> response = new Response<Message>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string userId =  _jwtUtils.ValidateToken(parameter);
            var messgaes = await _messageData.GetUnreadMessagesForUser(userId);
            if (messgaes == null)
            {
                response.Success = false;
                response.Message = "messgaes Not Found";
                return response;
            }

 
            response.Success = true;
            response.DataList = messgaes;
            return response;
        }


        public async Task<int> GetMessagesUnreadCount(string userId)
        {
            return await _messageData.GetCountForNewMessage(userId); // ON-301
        }

        public async Task<Response<Message>> DeleteMessage(int messageId)
        {
            Response<Message> response = new Response<Message>();
            Message message = await _messageData.GetMessageById(messageId);

            if (message == null)
            {
                response.Message = "message is not found";
                response.Success = false;
                return response;
            }
            if (message.IsDeleted)
            {
                response.Message = "message already deleted";
                response.Success = false;
                return response;
            }
            message.IsDeleted = true;
            var result = _messageData.UpdateMessage(message);

            response.Message = "message Deleted successfully";
            response.Success = true;
            return response;
        }


        public async Task<int> CountforMessage(string fromUserId, string toUserId)
        {
            int count = await _messageData.GetMessagesCountForUser(fromUserId, toUserId);

            return count;
        }


        public async Task DeleteMessageWithUserId(string authorizatioin, string toUserId)
        {
            Response<Message> response = new Response<Message>();

            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorizatioin, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            string fromUserId = _jwtUtils.ValidateToken(parameter);
            var messages = await _messageData.GetMessagesHistory(fromUserId,toUserId);

            foreach (Message message in messages)
            {
                if (message.FromUser.Id == fromUserId && message.ToUser.Id == toUserId)
                {
                    message.FromDeleted = true;
                    await _messageData.UpdateMessage(message);
                }
                else if (message.FromUser.Id == toUserId && message.ToUser.Id == fromUserId)
                {
                    message.ToDeleted = true;
                    await _messageData.UpdateMessage(message);
                }


            }

            response.Message = "message Deleted successfully";
            response.Success = true;
           
        }



        public async Task<Response<Message>> UpdateReadDateTimeForMessage(List<string> messageIds)
        {
            Response<Message> response = new Response<Message>();
     
                foreach (string messageId in messageIds)
                {
                    Message message = await _messageData.GetMessageById(Int32.Parse(messageId));
                    if (message == null)
                    {
                        response.Success = false;
                        response.Message = "message not found";
                        return response;
                    }
                    message.ReadDateTime = System.DateTime.Now;
                    var resultUp = await _messageData.UpdateMessage(message);
                }
            
            response.Success = true;
            response.Message = "Message update Successfully";
            return response;
        }

    }
}
