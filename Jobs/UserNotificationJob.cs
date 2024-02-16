using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Hub;
using OntrackDb.Repositories;
using OntrackDb.Service;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Jobs
{
    public class UserNotificationJob : IJob
    {
        private readonly IUserService _userService;
        private readonly IUserData _userData;
        private readonly INotificationData _notificationData;
        private readonly ILogger<UserNotificationJob> _logger;
        private readonly IHubContext<ChatHub> _hubContext;

        public UserNotificationJob(ILogger<UserNotificationJob> logger, IUserData userData, IUserService userService, INotificationData notificationData, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _userData = userData;
            _userService = userService;
            _notificationData = notificationData;
            _hubContext = hubContext;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            List<User> userList = (await _userService.GetAllUsersForLicenseExpiry()).DataList;

            foreach (User user in userList)
            {
                Notification notifi = await _notificationData.GetNotificationByUserId(user.Id);

                if (notifi == null && user.IsEnabled)
                {
                    Notification notification = new Notification
                    {
                        Status = "License Expired",
                        NotificationType = "License Expiry",
                        SendDateTime = DateTime.Now,
                        IsDeleted = false,
                        User = user
                    };

                    await _notificationData.AddNotification(notification);

                    try
                    {
                        await _hubContext.Clients.Groups(notification.User.Id).SendAsync("SendNotification", notification);
                    }
                    catch { }
                }
                else if (user.IsEnabled)
                {
                    notifi.SendDateTime = DateTime.Now;

                    await _notificationData.UpdateNotification(notifi);

                    //try
                    //{
                    //    await _hubContext.Clients.Groups(notifi.User.Id).SendAsync("SendNotification", notifi);
                    //}
                    //catch { }
                }

                var adminNotification = await _notificationData.GetAdminNotificationByTypeAndUserId("License Expiry", user.Id);

                if (adminNotification == null)
                {
                    if (user.IsEnabled)
                    {
                        adminNotification = new AdminNotification
                        {
                            Status = "License Expired",
                            NotificationType = "License Expiry",
                            SendDateTime = DateTime.Now,
                            IsDeleted = false,
                            User = user
                        };
                        
                        await _notificationData.AddAdminNotification(adminNotification);
                    }
                }
                else if (user.IsEnabled)
                {
                    adminNotification.SendDateTime = DateTime.Now;

                    await _notificationData.UpdateAdminNotification(adminNotification);
                }
                else
                {
                    adminNotification = null;
                }

                if (adminNotification != null)
                {
                    var admins = await _userData.GetAdminUsersByUserId(user.Id);

                    foreach (var admin in admins)
                    {
                        try
                        {
                            await _hubContext.Clients.Groups(admin.Id).SendAsync("SendNotification", adminNotification);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
