using Microsoft.EntityFrameworkCore;
using OntrackDb.Authentication;
using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class MessageData : IMessageData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly IImageService _imageService;
        public MessageData(ApplicationDbContext applicationDbcontext, IImageService imageService)
        {
            _applicationDbcontext = applicationDbcontext;
            _imageService = imageService;
        }

        public async Task<List<Message>> GetAllMessages()
        {
            var messages = await _applicationDbcontext.Messages.
             Include(u => u.ToUser).
           ToListAsync();
            return messages;
        }

        public async Task<List<Message>> GetAllMessagesForUser(string userId)
        {
            var messages = await _applicationDbcontext.Messages.
             Include(u => u.FromUser).
             Include(u => u.ToUser).
             Where(u=>u.FromUser.Id == userId || u.ToUser.Id == userId).
             ToListAsync();
            return messages;
        }
        public async Task<Message> SendMessage(Message message)
        {
            var result = await _applicationDbcontext.Messages.AddAsync(message);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public void DeleteAllMessageOfUser(List<Message> messages)
        {
             _applicationDbcontext.Messages.RemoveRange(messages);
        }

        public async Task<List<Message>> GetMessagesBySenderUserId(string userId)
        {
            var messages = await _applicationDbcontext.Messages
                .Where(m => (m.FromUser.Id == userId && !m.FromDeleted) || (m.ToUser.Id == userId && !m.ToDeleted))
                .Include(m => m.FromUser)
                .Include(m => m.ToUser)
                .OrderBy(m => m.SentDateTime)
                .ToListAsync();

            return messages;
        }

        public async Task<List<Message>> GetUnreadMessagesForUser(string userId)
        {
            var messages = await _applicationDbcontext.Messages.Include(m => m.FromUser).Include(m => m.ToUser).
                Where(m=>m.ToUser.Id == userId && !m.ToDeleted).ToListAsync();

            return messages;
        }

        public async Task<int> GetMessagesCountForUser(string fromUserId, string toUserId)
        {
            return await _applicationDbcontext.Messages
                .CountAsync(m => m.FromUser.Id == toUserId && m.ToUser.Id == fromUserId && !m.ToDeleted && m.ReadDateTime == new DateTime(0001, 01, 01));
        }
        public async Task<List<Message>> GetMessagesBetweenTwoUsers(string fromUserId, string toUserId)
        {
           
           var messages = await _applicationDbcontext.Messages.Include(m => m.FromUser).
                                                       Include(m => m.ToUser).Where(m => m.FromUser.Id == fromUserId && m.ToUser.Id == toUserId && !m.FromDeleted ||  m.FromUser.Id == toUserId && m.ToUser.Id == fromUserId && !m.ToDeleted).
                                                       ToListAsync();

            return messages;
        }


        public async Task<List<Message>> GetMessagesHistory(string fromUserId,string toUserId)
        {
            var messages = await _applicationDbcontext.Messages.Include(m => m.FromUser).
                                                      Include(m => m.ToUser).OrderBy(m=>m.SentDateTime).
                                                      Where(m=>(m.FromUser.Id == fromUserId && m.ToUser.Id == toUserId && !m.FromDeleted) || (m.FromUser.Id == toUserId && m.ToUser.Id == fromUserId && !m.ToDeleted)).
                                                      ToListAsync();
            List<Message> messageList = new List<Message>();
            foreach(var message in messages)
            {
                message.ToUser.ImageUri = _imageService.GetImageURI(message.ToUser.ImageName);
                message.StrSendDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", message.SentDateTime);
                messageList.Add(message);
            }
            return messageList;
        }


        public async Task<int> UpdateMessage(Message message)
        {

            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;

        }
        public async Task<Message> GetMessageById(int messageId)
        {
            var message = await _applicationDbcontext.Messages.FindAsync(messageId);
            
            return message;
        }



        public async Task<List<Pharmacy>> GetAllPharmacyByUserId(string userId)
        {
           var pharmacies = await _applicationDbcontext.PharmacyUsers.
                                       Include(x => x.Pharmacy.Address).
                                       Include(x => x.Pharmacy.Contact).
                                       Include(x => x.Pharmacy.Patients).
                                       Where(x => x.UserId == userId && !x.User.IsDeleted).
                                       Select(x => x.Pharmacy).ToListAsync();

            return pharmacies;
        }
        public async Task<List<User>>  GetAllUserForSamePharmacyByUserId(int pharmacyId)
        {
            var users = await _applicationDbcontext.PharmacyUsers.
                                        Where(x => x.Pharmacy.Id == pharmacyId && !x.User.IsDeleted).
                                        Select(x=>x.User).ToListAsync();

            return users;
        }

        public async Task<int> GetCountForNewMessage(string userId)
        {
            // ON-301
            return await _applicationDbcontext.Messages
                .CountAsync(x => x.FromUser.Id == userId && x.Status == "1");
        }

        public async Task<List<Message>> GetMessagesByRecipientUserId(string userId)
        {

            var messages = await _applicationDbcontext.Messages.Where(m => m.ToUser.Id == userId && !m.IsDeleted).
            ToListAsync();
            return messages;
        }

        public async Task<Response<Message>> DeleteMessageBymsgId(Message message)
        {
            Response<Message> response = new Response<Message>();
            bool val = message.IsDeleted;
            if (val)
            {
                response.Message = "Message is already deleted";
                response.Success = false;
                return response;
            }
            message.IsDeleted = true;
            await UpdateMessage(message);

            response.Message = "Message deleted successfully";
            response.Success = true;
            return response;
        }
    }

}