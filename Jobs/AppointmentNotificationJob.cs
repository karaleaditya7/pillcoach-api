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
    public class AppointmentNotificationJob : IJob
    {
        private readonly IAppointmentData _appointmentData;
        private readonly INotificationData _notificationData;
        private readonly ILogger<UserNotificationJob> _logger;
        private readonly IHubContext<ChatHub> _hubContext;
        public AppointmentNotificationJob(ILogger<UserNotificationJob> logger, IAppointmentData appointmentData, INotificationData notificationData, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _appointmentData = appointmentData;
            _notificationData = notificationData;
            _hubContext = hubContext;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {
            DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
            
            List<Appointment> appointmentList = await _appointmentData.GetAppointmentsByDate(dateTime);
            foreach (Appointment appointment in appointmentList)
            {
                int days = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalDays);
                int hours = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalHours);
                int minutes = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalMinutes);
                Notification notifi = await _notificationData.GetNotificationByAppointmentId(appointment.ID);
                if (notifi == null)
                {
                    if (days <= 1 && days >= 0 && hours <= 1 && hours >= 0 && minutes <= 60 && minutes > 0)
                    {
                        Notification notification = new Notification
                        {
                            Status = "Appointment",
                            NotificationType = "Appointment",
                            SendDateTime = DateTime.Now,
                            Appointment = appointment,
                            User = appointment.User
                        };
                        await _notificationData.AddNotification(notification);

                        try
                        {
                            await _hubContext.Clients.Groups(notification.User.Id).SendAsync("SendNotification", notification);
                        }
                        catch { }
                    }
                }
                else if (!notifi.IsDeleted)
                {
                    if (days <= 1 && days >= 0 && hours <= 1 && hours >= 0 && minutes <= 60 && minutes > 0)
                    {
                        notifi.SendDateTime = DateTime.Now;
                     //   notifi.ReadDateTime =new DateTime(0001, 01, 01);
                        await _notificationData.UpdateNotification(notifi);

                        try
                        {
                            await _hubContext.Clients.Groups(notifi.User.Id).SendAsync("SendNotification", notifi);
                        }
                        catch { }
                    }
                  
                }
               
            }

            }
            
        }
    }

