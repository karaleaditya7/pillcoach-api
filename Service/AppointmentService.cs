
using OntrackDb.Authentication;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class AppointmentService : IAppointmentService
    {

        private readonly IAppointmentData _appointmentData;
        private readonly IPatientData _patientData;
        private readonly IUserData _userData;
        private readonly ApplicationDbContext _applicationDbcontext;
        private readonly INotificationData _notificationData;



        public AppointmentService(IAppointmentData appointmentData, IPatientData patientData, IUserData userData, ApplicationDbContext applicationDbcontext, INotificationData notificationData)
        {
           _appointmentData = appointmentData;
            _patientData = patientData;
            _userData = userData;
            _applicationDbcontext = applicationDbcontext;
            _notificationData = notificationData;
        }


        public async Task<Response<Appointment>> GetAppointmentsByUserId(string userId,int month,int year)
        {
            Response<Appointment> response = new Response<Appointment>();
         
            var appointments = await _appointmentData.GetAppointmentsByUserId(userId);
            if (appointments == null)
            {
                response.Success = false;
                response.Message = "Appointments Not Found";
                return response;
            }
            List<Appointment> appointmentList = new List<Appointment>();
            foreach (Appointment appointment in appointments)
            {
               
                DateTime dtt= DateTime.Now.ToUniversalTime();
                String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" +" GMT", appointment.StartDateTime);

                appointment.StrStartDateTime = strStartDateTime;
                int val;
                bool result = int.TryParse(appointment.Duration, out val);
                appointment.EndDateTime = appointment.StartDateTime.AddMinutes(val);
                String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", appointment.EndDateTime);

                appointment.StrEndDateTime = strEndDateTime;
                if (!result)
                {

                    response.Success = false;
                    response.Message = "Something is wrong with duration time format";
                    return response;
                }
                if (!appointment.IsDeleted && !appointment.IsCancel)
                {
                    
                    int dateResult = DateTime.Compare(appointment.StartDateTime, DateTime.Today);

                    if (dateResult >= 0)
                    {
                        appointmentList.Add(appointment);
                    }
                }

            }
            if(month != 0 && year != 0)
            {
                DateTime startDate = new DateTime(year, month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);
                appointmentList = appointmentList.Where(a => a.StartDateTime.Date >= startDate.Date && a.EndDateTime.Date <= endDate.Date).ToList();
                response.Success = true;
                response.DataList = appointmentList;
                return response;
            }
            else
            {
                response.Success = true;
                response.DataList = appointmentList;
                return response;
            }
        
        }



        public async Task<Response<Appointment>> DeleteAppointmentById(int appointmentId)
        {
            Response<Appointment> response = new Response<Appointment>();
            var result = await _appointmentData.GetAppointmentByAppointmentId(appointmentId);
            List<Notification> notifications = await _notificationData.GetAllNotificationByAppointmentId(appointmentId);
            if (result != null)
            {
                if(notifications != null)
                {
                    foreach(var notifcation in notifications)
                    {
                        await _notificationData.DeleteNotificationForAppointmentById(notifcation.id);
                    }
                }
                
               await _appointmentData.DeleteAppointment(result);
                 
            }
            if (result == null)
            {
                response.Message = "AppointmentId is Not Found";
                response.Success = false;
                return response;
            }
            response.Message = "Appointment Deleted successfully";
            response.Success = true;
            return response;

        }


        public async Task DeleteAppointmentByIdForPatientDelete(int appointmentId)
        {
          
            var result = await _appointmentData.GetAllAppointmentByAppointmentId(appointmentId);
            List<Notification> notifications = await _notificationData.GetAllNotificationByAppointmentId(appointmentId);
            if (result != null)
            {
                if (notifications != null)
                {
                    foreach (var notifcation in notifications)
                    {
                        await _notificationData.DeleteNotificationForAppointmentById(notifcation.id);
                    }
                }

                 _appointmentData.DeleteAppointmentForPatient(result);

            }

        }

        public async Task<Response<Appointment>> GetAppointmentsByPatientId(int patientId)
        {
            Response<Appointment> response = new Response<Appointment>();
            var appointments = await _appointmentData.GetAppointmentsByPatientId(patientId);
            if (appointments == null)
            {
                response.Success = false;
                response.Message = "Appointments Not Found";
                return response;
            }
            List<Appointment> appointmentList = new List<Appointment>();

            foreach (Appointment appointment in appointments)
            {
                DateTime dtt = DateTime.Now.ToUniversalTime();
                String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", appointment.StartDateTime);
                
                appointment.StrStartDateTime = strStartDateTime;
                int val;
                bool result = int.TryParse(appointment.Duration, out val);
                appointment.EndDateTime = appointment.StartDateTime.AddMinutes(val);
                String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", appointment.EndDateTime);
                
                appointment.StrEndDateTime = strEndDateTime;
                if (!result)
                {

                    response.Success = false;
                    response.Message = "Something is wrong with duration time format";
                    return response;
                }

                if (!appointment.IsCancel)
                {

                    int dateResult = DateTime.Compare(appointment.StartDateTime, DateTime.Today);

                    if (dateResult < 0)
                    {
                        appointmentList.Add(appointment);
                    }


                }
            }
            response.Success = true;
            response.DataList = appointmentList;
            return response;
        }


        public async Task<Response<Appointment>> UpdateAppointment(AppointmentModel model)
        {
            Response<Appointment> response = new Response<Appointment>();
            var appointment = await _appointmentData.GetAppointmentByAppointmentId(model.ID);
            var appintmentlist = await _appointmentData.GetAppointmentsByUserId(model.UserId);

            foreach (var appoint in appintmentlist)
            {
                DateTime modelEndDateTime = model.Reschedule.AddMinutes(Int32.Parse(model.Duration));
                DateTime endDate = appoint.StartDateTime.AddMinutes(Int32.Parse(appoint.Duration));
                if (!model.IsCancel)
                {
                 
                    if (model.Reschedule.CompareTo(appoint.StartDateTime) < 0 && modelEndDateTime.CompareTo(appoint.StartDateTime) >= 0 && model.ID != appoint.ID)
                    {
                        response.Message = "Cannot book an appointment for this time slot.";
                        return response;
                    }
       
                }

                if (model.Reschedule < DateTime.Now && !model.IsCancel)
                {
                    response.Message = "Cannot book an appointment for this time slot.";
                    return response;
                }

                if (!appoint.IsCancel && model.IsReschedule && !model.IsCancel && model.Reschedule != appoint.StartDateTime && model.ID != appoint.ID)
                {

                    if (model.Reschedule.CompareTo(appoint.StartDateTime) >= 0 && model.Reschedule.CompareTo(endDate) <= 0)
                    {
                        response.Message = "Appointment already exists for this time slot.";
                        return response;
                    }
                }
                if (model.ID == appoint.ID && !model.IsCancel && model.Reschedule != appoint.StartDateTime && !appoint.IsCancel && model.IsReschedule)
                {
                    DateTime endDateForUpdate = model.StartDateTime.AddMinutes(Int32.Parse(model.Duration));
                    if (model.Reschedule.CompareTo(appointment.StartDateTime) < 0 && endDateForUpdate.CompareTo(appointment.StartDateTime) > 0 && endDateForUpdate.CompareTo(appointment.EndDateTime) <= 0)
                    {
                        response.Message = "Cannot book an appointment for this time slot.";
                        return response;
                    }
                }

            }
            if (appointment != null)
            {
                 if(!model.IsCancel && model.IsReschedule)
                {
               
                    if (model.Reschedule < DateTime.Now)
                    {
                        response.Message = "Cannot book an appointment for this time slot.";
                        return response;
                    }

                }
            }
            if (appointment == null)
            {
                response.Success = false;
                response.Message = "Appointment not found";
                return response;
            }
            if (string.IsNullOrEmpty(model.Notes))
            {
                response.Message = "Note is Missing";
                return response;
            }


            if (model.IsCancel)
            {
                appointment.IsCancel = model.IsCancel;
                var result = await _appointmentData.UpdateAppointment(appointment);
                var notifi = await _notificationData.GetNotificationByAppointmentId(appointment.ID);
                if (notifi != null) {
                    notifi.IsDeleted = true;
                    await _notificationData.UpdateNotification(notifi);
                }
                response.Success = true;
                response.Message = "Appointment Updated successfully!";
                return response;
            }
           
            if(model.IsReschedule)
            {
                appointment.StartDateTime = model.Reschedule;
                appointment.Notes = model.Notes;
                appointment.Duration = model.Duration;
                var result = await _appointmentData.UpdateAppointment(appointment);
                var notifi = await _notificationData.GetNotificationByAppointmentId(appointment.ID);
                if (notifi != null)
                {
                    DateTime dateTime = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                    int days = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalDays);
                    int hours = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalHours);
                    int minutes = Convert.ToInt32((appointment.StartDateTime - dateTime).TotalMinutes);
                    if (days <= 1 && days >= 0 && hours <= 1 && hours >= 0 && minutes <= 60 && minutes > 0)
                    {
                        notifi.ReadDateTime = new DateTime(0001, 01, 01);
                        notifi.IsDeleted = false;
                        await _notificationData.UpdateNotification(notifi);
                    }
                    else
                    {
                        notifi.ReadDateTime = new DateTime(0001, 01, 01);
                        notifi.IsDeleted = true;
                        await _notificationData.UpdateNotification(notifi);
                    }
                        
                }
                //var notifi = await _notificationData.GetNotificationByAppointmentId(appointment.ID);
                //if (notifi != null)
                //{
                //    notifi.SendDateTime = DateTime.Now;
                //    notifi.IsRead = false;
                //    await _notificationData.UpdateNotification(notifi);
                //}
                //else
                //{
                //    Notification notification = new Notification
                //    {

                //        User = appointment.User,
                //        NotificationType = "Appointment",
                //        SendDateTime = DateTime.Now,
                //        Appointment = appointment,
                //        Status = "Success"
                //    };
                //    var notiResult = _notificationData.AddNotification(notification);
                //}

                response.Success = true;
                response.Message = "Appointment Updated successfully!";
                response.Data = appointment;
                return response;
            }
            response.Success = false;
            response.Message = "Error While updating the Appointment";
            return response;
        }

        public async Task<Response<Appointment>> GetAppointmentByAppointmentId(int appointmentId)
        {
            Response<Appointment> response = new Response<Appointment>();
            var appointment = await _appointmentData.GetAppointmentByAppointmentId(appointmentId);

            if (appointment == null)
            {
                response.Message = "Appointment Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "Appointment retrived successfully";
            response.Data = appointment;
            return response;
        }


        public async Task<Response<Appointment>> GetScheduledAppointmentsByuserId(string userId)
        {
            Response<Appointment> response = new Response<Appointment>();
            var appointments = await _appointmentData.GetScheduledAppointmentsByuserId(userId);

            List<Appointment> appointmentList = new List<Appointment>();

            foreach (Appointment appointment in appointments)
            {
                DateTime dtt = DateTime.Now.ToUniversalTime();
                String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", appointment.StartDateTime);

                appointment.StrStartDateTime = strStartDateTime;
                int val;
                bool result = int.TryParse(appointment.Duration, out val);
                appointment.EndDateTime = appointment.StartDateTime.AddMinutes(val);
                String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", appointment.EndDateTime);

                appointment.StrEndDateTime = strEndDateTime;
                if (!result)
                {

                    response.Success = false;
                    response.Message = "Something is wrong with duration time format";
                    return response;
                }
                 appointmentList.Add(appointment);
                
            }


            if (appointments == null)
            {
                response.Message = "Appointment Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "Appointment retrived successfully";
            response.DataList = appointmentList;
            return response;
        }

        public async Task<Response<Appointment>> AddAppointment(AppointmentModel model)
        {
            Response<Appointment> response = new Response<Appointment>();
            var appoint = await _appointmentData.GetAppointmentsByUserId(model.UserId);

            if(appoint != null )
            {
                foreach(var appointments in appoint)
                {
                    DateTime modelEndDateTime = model.StartDateTime.AddMinutes(Int32.Parse(model.Duration));
                    if (!model.IsCancel)
                    {
                    
                        if (model.StartDateTime.CompareTo(appointments.StartDateTime) < 0 && modelEndDateTime.CompareTo(appointments.StartDateTime) >= 0)
                        {
                            response.Message = "Cannot book an appointment for this time slot.";
                            return response;
                        }
                    
                    }
                    if (model.StartDateTime < DateTime.Now)
                    {
                        response.Message = "Cannot book an appointment for this time slot.";
                        return response;
                    }
                    DateTime endDateForNewAppoint = model.StartDateTime.AddMinutes(Int32.Parse(model.Duration));
                    if (model.StartDateTime.CompareTo(appointments.StartDateTime) < 0 && endDateForNewAppoint.CompareTo(appointments.StartDateTime) > 0 && endDateForNewAppoint.CompareTo(appointments.EndDateTime) <= 0)
                    {
                        response.Message = "Appointment already exists for this time slot.";
                        return response;
                    }
                    DateTime endDate = appointments.StartDateTime.AddMinutes(Int32.Parse(appointments.Duration));
                    if (model.StartDateTime.CompareTo(appointments.StartDateTime) >= 0 && model.StartDateTime.CompareTo(endDate) <= 0)
                    {
                        response.Message = "Appointment already exists for this time slot.";
                        return response;
                    }


                }
            }
           
            response.Success = false;
            if (string.IsNullOrEmpty(model.Notes))
            {
                response.Message = "Note is Missing";
                return response;
            }

            if (string.IsNullOrEmpty(model.PatientId.ToString()))
            {
                response.Message = "Patient Info is Missing";
                return response;
            }
            if (model.UserId == null)
            {
                response.Message = "User Info is Missing";
                return response;
            }

          
            Patient patient = await _patientData.GetPatientById(model.PatientId);
            if(patient == null)
            {
                response.Message = "Patient Not Found";
                return response;
            }
            User user = await _userData.GetUserById(model.UserId);
            if (user == null)
            {
                response.Message = "User Not Found";
                return response;
            }

            Appointment appointment = new Appointment
            {
                Notes = model.Notes,
                Patient = patient,
                User = user,
                StartDateTime = model.StartDateTime,
                Duration =model.Duration,
                IsCancel = model.IsCancel,      
               
            };


            var result = await _appointmentData.AddAppointment(appointment);
                   response.Success = true;
                   response.Data = appointment;
                   response.Message = "Appointment created successfully!";
                   return response;


        }


        public async Task<Response<Appointment>> GetAppointments()
        {
            Response<Appointment> response = new Response<Appointment>();
            
            var appointments = await _appointmentData.GetAppointments();
           
            List<Appointment> appointmentList = new List<Appointment>();
            
            foreach (Appointment appointment in appointments)
            {
                int val;
                bool result = int.TryParse(appointment.Duration, out val);
                appointment.EndDateTime = appointment.StartDateTime.AddMinutes(val);
                if (!result)
                {

                    response.Success = false;
                    response.Message = "Something is wrong with duration time format";
                    return response;
                }

                if(!appointment.IsCancel)
                {
                    
                    int dateResult = DateTime.Compare(appointment.StartDateTime, DateTime.Today);

                    if (dateResult >= 0)
                    {
                        appointmentList.Add(appointment);
                    }


                }

            }
            if (appointments == null)
            {
                response.Success = false;
                response.Message = "Appointments Not Found";
                return response;
            }
            response.Success = true;
            response.DataList = appointmentList;
            return response;
        }
    }
}
