using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OntrackDb.Authentication;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio.TwiML.Messaging;

namespace OntrackDb.Hub
{
    [Authorize]
    public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    {
        readonly IUserData _userData;
        readonly IMessageData _messageData;
        readonly INotificationData _notificationData;
        private static Dictionary<string, string> ConnectedUsers = new Dictionary<string, string>();
        public ChatHub(IUserData userData, IMessageData messageData, INotificationData notificationData)
        {
            _userData = userData;
            _messageData = messageData;
            _notificationData = notificationData;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.User?.FindFirst("id")?.Value;
            var validUser = false;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userData.GetUserByUserId(userId);

                if (user != null && user.IsEnabled && !user.IsDeleted)
                {
                    validUser = true;

                    await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                    await Clients.Others.SendAsync("UserOnline", $"{user.FirstName} {user.LastName}");
                }
                ConnectedUsers[userId] = Context.ConnectionId;
            }

            if (!validUser) Context.Abort();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.GetHttpContext()?.User?.FindFirst("id")?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userData.GetUserByUserId(userId);

                if (user == null) return;

                await Clients.Others.SendAsync("UserOffline", $"{user.FirstName} {user.LastName}");
            }
            ConnectedUsers.Remove(userId);
        }

        public async Task SendDirectMessage(ChatMessage chatMessage)
        {
            if (chatMessage == null || !chatMessage.IsValid()) return;

            chatMessage.SentDateTime = DateTime.Now;

            var sender = await _userData.GetUserByUserId(chatMessage.Sender.UserId);
            var recipient = await _userData.GetUserByUserId(chatMessage.Recipient);

            if (sender == null || recipient == null) return;

            var message = new Entities.Message
            {
                FromUser = sender,
                ToUser = recipient,
                MessageText = chatMessage.Message,
                Type = "Message",
                SentDateTime = chatMessage.SentDateTime
            };

            await _messageData.SendMessage(message);

            var notification = new Notification
            {
                User = recipient,
                Message = message,
                NotificationType = "Message",
                SendDateTime = chatMessage.SentDateTime,
                Status = "Success"
            };

            await _notificationData.AddNotification(notification);

            // send the message back to the sender, so that the UI can display it
            await Clients.Caller.SendAsync("ReceiveChatMessage", chatMessage);

            // send the message to the recipient
            await Clients.Groups(chatMessage.Recipient).SendAsync("ReceiveChatMessage", chatMessage);
        }

        public async Task SendNotification(Notification notification)
        {
            if (ConnectedUsers.TryGetValue(notification.User.Id, out var connectionId))
            {
                // Send the notification to the specified user
                await Clients.Client(connectionId).SendAsync("SendNotification", notification);
            }
        }

        public async Task SendAdminNotification(AdminNotification adminNotification)
        {
            if (ConnectedUsers.TryGetValue(adminNotification.User.Id, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("SendNotification", adminNotification);
            }
        }
    }

    public class ChatMessage
    {
        public ChatUser Sender { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public DateTime SentDateTime { get; set; }

        internal bool IsValid() => !(string.IsNullOrWhiteSpace(Sender?.UserId) || string.IsNullOrWhiteSpace(Recipient) || string.IsNullOrWhiteSpace(Message))
            && !Sender.UserId.Equals(Recipient, StringComparison.OrdinalIgnoreCase);
    }

    public class ChatUser
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
}
