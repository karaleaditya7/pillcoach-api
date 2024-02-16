using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface INotificationData
    {
        Task<Notification> AddNotification(Notification notification);
        Task<AdminNotification> AddAdminNotification(AdminNotification adminNotification);
        Task<Notification> UpdateNotification(Notification notification);
        Task<AdminNotification> UpdateAdminNotification(AdminNotification adminNotification);
        Task<Notification> GetNotificationByAppointmentId(int appoinmentId);
        Task<Notification> GetNotificationByUserId(string userId);
        Task<AdminNotification> GetAdminNotificationByUserId(string userId);
        Task<Notification> GetNotificationById(int id);
        Task<AdminNotification> GetAdminNotificationById(int id);
        Task<List<Notification>> GetAllNotificationsByUserId(string userId);
        Task DeleteASingleNotification(int notificationId);
        Task<List<Notification>> GetAllUsersExpiryNotificationsByType(string type);
        Task<List<AdminNotification>> GetAllUsersExpiryAdminNotificationByType(string adminType, string userId);

        Task DeleteNotificationForAppointmentById(int notificationId);
        Task<AdminNotification> GetAdminNotificationByUserIdForDelete(string userId);
        void DeleteNotificationForAdminNotificationById(List<AdminNotification> notifications);
        Task<AdminNotification> GetAdminNotificationByTypeAndUserId(string type, string userId);

        Task<AdminNotification> GetAdminNotificationByAppointmentId(int appoinmentId);

        Task DeleteAdminNotificationForAppointmentById(int adminNotificationId);
        Task<List<Notification>> GetAllNotificationByAppointmentId(int appoinmentId);

        Task<List<Notification>> GetNotificationByUserIdForDelete(string userId);

        void DeleteNotificationForUser(List<Notification> notifications);

    }
}
