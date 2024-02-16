using OntrackDb.Authentication;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IMessageData
    {
        Task<Message> SendMessage(Message message);
        Task<List<Message>> GetMessagesBySenderUserId(string userId);
        Task<List<Message>> GetMessagesByRecipientUserId(string userId);
        Task<int> UpdateMessage(Message message);
        Task<Message> GetMessageById(int messageId);

        Task<List<Message>> GetAllMessages();

        Task<List<Pharmacy>> GetAllPharmacyByUserId(string userId);

        Task<List<User>> GetAllUserForSamePharmacyByUserId(int pharmacyId);

        Task<int> GetCountForNewMessage(string userId); // ON-301

        Task<List<Message>> GetMessagesHistory(string fromUserId, string toUserId);
        Task<int> GetMessagesCountForUser(string fromUserId, string toUserId);

        Task<List<Message>> GetUnreadMessagesForUser(string userId);

        Task<List<Message>> GetMessagesBetweenTwoUsers(string fromUserId, string toUserId);

        Task<List<Message>> GetAllMessagesForUser(string userId);
        void DeleteAllMessageOfUser(List<Message> messages);

        //Task<List<Message>> GetMessagesForCountMethod(string fromUserId, string toUserId);
        //Task<int> GetUnReadMessagesCountForUsers(string fromUserId, string toUserId);
    }
}
