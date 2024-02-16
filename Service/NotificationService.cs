using OntrackDb.Authorization;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class NotificationService: INotificationService
    {
        private readonly INotificationData _notificationData;
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly IJwtUtils _jwtUtils;
        private readonly IImageService _imageService;

        public NotificationService(INotificationData notificationData, ApplicationDbContext applicationDbcontext, IJwtUtils jwtUtils, IImageService imageService)
        {
            _notificationData = notificationData;
            _applicationDbcontext = applicationDbcontext;
            _jwtUtils = jwtUtils;
            _imageService = imageService;

        }
        public async Task<Response<Notification>> UpdateNotification(List<string> notificationIds,string authorization)
        {
            Response<Notification> response = new Response<Notification>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role =_jwtUtils.GetRole(parameter);

            if (role == Roles.Admin.ToString() || role == Roles.SuperAdmin.ToString())
            {
                foreach (string notificationId in notificationIds)
                {
                    AdminNotification notification = await _notificationData.GetAdminNotificationById(Int32.Parse(notificationId));
                    
                    if (notification != null)
                    {
                        notification.ReadDateTime = System.DateTime.Now;
                        await _notificationData.UpdateAdminNotification(notification);
                    }
                    else
                    {
                        var result1 = await _notificationData.GetNotificationById(Int32.Parse(notificationId));
                        result1.ReadDateTime = System.DateTime.Now;
                        await _notificationData.UpdateNotification(result1);
                    }
                }
            }
            else
            {
                foreach (string notificationId in notificationIds)
                {
                    Notification notification = await _notificationData.GetNotificationById(Int32.Parse(notificationId));
                    if (notification == null)
                    {
                        response.Success = false;
                        response.Message = "Notification not found";
                        return response;
                    }
                    notification.ReadDateTime = System.DateTime.Now;
                    await _notificationData.UpdateNotification(notification);
                    var resultUp = await _notificationData.UpdateNotification(notification);
                }
            }
               
         
            response.Success = true;
            response.Message = "Notification update Successfully";
            return response;
        }


        public async Task DeleteNotification(List<string> notificationIds, string authorization)
        {
            Response<Notification> response = new Response<Notification>();
            var parameter = "";
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role = _jwtUtils.GetRole(parameter);

            if (role == Roles.Admin.ToString() || role == Roles.SuperAdmin.ToString())
            {
                foreach (string notificationId in notificationIds)
                {
                    var result = await _notificationData.GetAdminNotificationById(Int32.Parse(notificationId));
                    if (result != null)
                    {
                        result.IsDeleted = true;
                        await _notificationData.UpdateAdminNotification(result);
                    }
                    else
                    {
                        var result1 = await _notificationData.GetNotificationById(Int32.Parse(notificationId));
                        result1.IsDeleted = true;
                        await _notificationData.UpdateNotification(result1);
                    }
                    

                }
            }
            else
            {
                foreach (string notificationId in notificationIds)
                {
                    var result = await _notificationData.GetNotificationById(Int32.Parse(notificationId));
                    if(result != null)
                    {
                        result.IsDeleted = true;
                        await _notificationData.UpdateNotification(result);
                    }
                   

                }
            }
               

        
        }

      


        public async Task<Response<object>> GetAllNotificationsForUser(string userId, string authorization)
        {
            Response<object> response = new Response<object>();
            List<object> allNotifications = new List<object>();
            var notifications = await _notificationData.GetAllNotificationsByUserId(userId);

            var parameter = "";

            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                parameter = headerValue.Parameter;
            }
            var role = _jwtUtils.GetRole(parameter);

            if (role == Roles.Admin.ToString() || role == Roles.SuperAdmin.ToString())
            {
                var notificationsMessage = await _notificationData.GetAllUsersExpiryAdminNotificationByType(role, userId);
                allNotifications.AddRange(notificationsMessage);
                allNotifications.AddRange(notifications);
                if (notificationsMessage == null)
                {
                    response.Success = false;
                    response.Message = "Notifications Not Found";
                    return response;
                }


                response.Success = true;
                response.DataList = allNotifications;
                return response;
            }

            if (notifications == null)
            {
                response.Success = false;
                response.Message = "Notifications Not Found";
                return response;
            }
    
            response.Success = true;
            response.DataList = notifications.Cast<object>().ToList();
            return response;
        }
    }
}
