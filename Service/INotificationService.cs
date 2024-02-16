using OntrackDb.Dto;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface INotificationService
    {
        Task<Response<Notification>> UpdateNotification(List<string> notificationIds,string authorization);
        Task<Response<object>> GetAllNotificationsForUser(string userId, string authorization);
        Task DeleteNotification(List<string> notificationIds, string authorization);

       
    }
}
