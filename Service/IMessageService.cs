using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IMessageService
    {
        Task<Response<Message>> SendMessage(List<MessageModel> models);

        Task<Response<Message>> GetMessagesBySenderId(string userId); 

        Task<Response<Message>> GetMessagesByRecipientId(string userId);

        Task<Response<Message>> DeleteMessage(int messageId);

        Task<Response<Message>> GetAllMessages();

        Task<Response<User>> GetAllUserForSamePharmacy(string authorization, string userId);

        Task<int> GetMessagesUnreadCount(string userId);
        Task<List<MessageUserModel>> GetAllMessagesforUser(string userId);
        Task<Response<WebSocket>> GetConnectIdByUserId(string userId);

        Task<Response<Message>> GetMessageHistory(string authorizatioin, string toUserId);
        Task DeleteMessageWithUserId(string authorizatioin, string toUserId);

        Task<List<MessageUserModel>> GetAllMessageFromSearch(string userId,string search);

        Task<int> CountforMessage(string fromUserId, string toUserId);
        Task<Response<Message>> UpdateReadDateTimeForMessage(List<string> messageIds);

        Task<Response<Message>> GetUnreadMessagesForUser(string authorizatioin);

    }
}
