using Microsoft.EntityFrameworkCore;
using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class NotificationData : INotificationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly IImageService _imageService;
        private readonly IUserData _userData;

        public NotificationData(ApplicationDbContext applicationDbcontext, IImageService imageService,IUserData userData)
        {
            _applicationDbcontext = applicationDbcontext;
             _imageService= imageService;
            _userData = userData;
        }

        public async Task<Notification> AddNotification(Notification notification) {
            var result = await _applicationDbcontext.Notifications.AddAsync(notification);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<AdminNotification> AddAdminNotification(AdminNotification adminNotification)
        {
            var result = await _applicationDbcontext.AdminNotifications.AddAsync(adminNotification);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<AdminNotification> GetAdminNotificationByTypeAndUserId(string type , string userId)
        {
            var adminNotification = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.User.Id == userId && p.NotificationType == type).
            FirstOrDefaultAsync();
            return adminNotification;
        }

        public async Task<AdminNotification> GetAdminNotificationByAppointmentId(int appoinmentId)
        {
            var appointment = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.Appointment.ID == appoinmentId && !p.IsDeleted).
            FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<Notification> GetNotificationByAppointmentId(int appoinmentId)
        {
            var appointment = await _applicationDbcontext.Notifications.Include(u => u.User).
            Where(p => p.Appointment.ID == appoinmentId && !p.IsDeleted).
            FirstOrDefaultAsync();
            return appointment;
        }
        public async Task<List<Notification>> GetAllNotificationByAppointmentId(int appoinmentId)
        {
            var appointments = await _applicationDbcontext.Notifications.Include(u => u.User).
            Where(p => p.Appointment.ID == appoinmentId).
            ToListAsync();
            return appointments;
        }

        public async Task<Notification> GetNotificationByUserId(string userId)
        {
            var appointment = await _applicationDbcontext.Notifications.Include(u => u.User).
            Where(p => p.User.Id == userId && p.NotificationType == "License Expiry").
            FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<AdminNotification> GetAdminNotificationByUserId(string userId)
        {
            var appointment = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.User.Id == userId).
            FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<AdminNotification> GetAdminNotificationByUserIdForDelete(string userId)
        {
            var notification = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.User.Id == userId).
            FirstOrDefaultAsync();
            return notification;
        }
        public async Task DeleteASingleNotification(int notificationId)
        {
            var result = await GetNotificationById(notificationId);
            _applicationDbcontext.Notifications.Remove(result);
           await _applicationDbcontext.SaveChangesAsync();
        }


        public async Task DeleteNotificationForAppointmentById(int notificationId)
        {
            var result = await GetNotificationById(notificationId);
            _applicationDbcontext.Notifications.Remove(result); 
        }

        public async Task DeleteAdminNotificationForAppointmentById(int adminNotificationId)
        {
            var result = await GetAdminNotificationById(adminNotificationId);
            _applicationDbcontext.AdminNotifications.Remove(result);
        }

        public void DeleteNotificationForAdminNotificationById(List<AdminNotification> notifications)
        {
            foreach (AdminNotification notification in notifications)
            {
                _applicationDbcontext.AdminNotifications.Remove(notification);
            }

        }

        public void DeleteNotificationForUser(List<Notification> notifications)
        {
            foreach (Notification notification in notifications)
            {
                _applicationDbcontext.Notifications.Remove(notification);
            }

        }

        public async Task<List<Notification>> GetNotificationByUserIdForDelete(string userId)
        {
            var notifications = await _applicationDbcontext.Notifications.Include(u => u.User).
            Where(p => p.User.Id == userId).
            ToListAsync();
            return notifications;
        }



        public async Task<Notification> UpdateNotification(Notification notification) {
            
            var result = _applicationDbcontext.Notifications.Update(notification);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<AdminNotification> UpdateAdminNotification(AdminNotification adminNotification)
        {

            var result = _applicationDbcontext.AdminNotifications.Update(adminNotification);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Notification> GetNotificationById(int id) {
            var notification = await _applicationDbcontext.Notifications.Include(u => u.User).
            Where(p => p.id == id).
            FirstOrDefaultAsync();
            return notification;
        }
        public async Task<AdminNotification> GetAdminNotificationById(int id)
        {
            var adminNotification = await _applicationDbcontext.AdminNotifications.Include(u => u.User).
            Where(p => p.id == id).
            FirstOrDefaultAsync();
            return adminNotification;
        }


        public async Task<List<Notification>> GetAllNotificationsByUserId(string userId)
        {
         
    
            var notifications = await _applicationDbcontext.Notifications.Include(n =>n.User).
                                     Include(n=>n.User.Licenses).
                                     Include(m => m.Message).
                                     Include(m => m.Message.FromUser).
                                     Include(m => m.Message.ToUser).
                                     Include(p => p.Appointment).
                                     Include(p => p.Appointment.Patient).
                                     Include(p => p.Appointment.Patient.Contact).
                                    Where(n => n.User.Id == userId && !n.IsDeleted).
                               
                                 ToListAsync();

            foreach (Notification notify in notifications)
            {
              notify.User.ImageUri = _imageService.GetImageURI(notify.User.ImageName);
                if (notify.ReadDateTime == new DateTime(0001, 01, 01))
                {
                    notify.IsRead = false;
                }
                else
                {
                    notify.IsRead = true;
                }
                if (notify.Appointment != null)
                {
                    String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", notify.Appointment.StartDateTime);

                    notify.Appointment.StrStartDateTime = strStartDateTime;
                    int val;
                    int.TryParse(notify.Appointment.Duration, out val);
                    notify.Appointment.EndDateTime = notify.Appointment.StartDateTime.AddMinutes(val);
                    String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", notify.Appointment.EndDateTime);
                    notify.Appointment.StrEndDateTime = strEndDateTime;
                }

                if (notify.Message != null)
                {
                    notify.Message.StrSendDateTime= String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", notify.Message.SentDateTime);
                    
                    if(notify.Message.FromUser.Id != userId)
                    { 
                    notify.Message.FromUser.ImageUri = _imageService.GetImageURI(notify.Message.FromUser.ImageName);
                    }
                    notify.Message.ToUser.ImageUri = _imageService.GetImageURI(notify.Message.ToUser.ImageName);
                }
            }

            return notifications;
        }


        public async Task<List<Notification>> GetAllUsersExpiryNotificationsByType(string type)
        {
            var notifications = await _applicationDbcontext.Notifications.Include(n => n.User).
                                     Include(n=> n.User.Licenses).
                                     Where(n => n.NotificationType == type && !n.IsDeleted).
                                     ToListAsync();
            foreach (Notification notify in notifications)
            {
               
                if (notify.ReadDateTime == new DateTime(0001, 01, 01))
                {
                    notify.IsRead = false;
                }
                else
                {
                    notify.IsRead = true;
                }
            }
            return notifications;
        }

        public async Task<List<AdminNotification>> GetAllUsersExpiryAdminNotificationByType(string adminType, string userId)
        {
            var query = new List<AdminNotification>();

            UserDto user = await _userData.GetUserInfoById(userId);
            if (adminType == Roles.SuperAdmin.ToString())
            {
                if (user.ImportEnabled)
                {
                    query = await _applicationDbcontext.AdminNotifications
                                .Include(n => n.User)
                                .Include(n => n.Message)
                                .Include(n => n.Message.FromUser)
                                .Include(n => n.Message.ToUser)
                                .Include(n => n.User.Licenses)
                                .Where(n => !n.IsDeleted).ToListAsync();
                }
                else
                {
                    query = await _applicationDbcontext.AdminNotifications
                                .Include(n => n.User)
                                .Include(n => n.Message)
                                .Include(n => n.Message.FromUser)
                                .Include(n => n.Message.ToUser)
                                .Include(n => n.User.Licenses)
                                .Where(n => !n.IsDeleted && !n.Status.Contains("Patients Import")).ToListAsync();
                }
                 
            }

            if (adminType == Roles.Admin.ToString())
            {
                 query = await _applicationDbcontext.AdminNotifications
                    .Include(n => n.User)
                    .Include(n => n.Message)
                    .Include(n => n.Message.FromUser)
                    .Include(n => n.Message.ToUser)
                    .Include(n => n.User.Licenses)
                    .Where(n => !n.IsDeleted && !n.ForSuperAdminOnly && 
                        n.User.PharmacyUsers.Any(p => _applicationDbcontext.PharmacyUsers.Where(p => p.UserId == userId).Select(p => p.PharmacyId).Contains(p.PharmacyId))).ToListAsync();
            }

            //var notifications = await query.ToListAsync();
            var notifications = query;

            foreach (AdminNotification notify in notifications.Where(n => n.User != null))
            {
                notify.User.ImageUri = _imageService.GetImageURI(notify.User.ImageName);

                if (notify.ReadDateTime == new DateTime(0001, 01, 01))
                {
                    notify.IsRead = false;
                }
                else
                {
                    notify.IsRead = true;
                }

                if (notify.Message != null)
                {
                    notify.Message.FromUser.ImageUri = _imageService.GetImageURI(notify.Message.FromUser.ImageName);
                    notify.Message.ToUser.ImageUri = _imageService.GetImageURI(notify.Message.ToUser.ImageName);
                }
            }

            return notifications;
        }
    }
}
