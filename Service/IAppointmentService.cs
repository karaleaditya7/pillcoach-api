using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IAppointmentService
    {
        Task<Response<Appointment>> GetAppointments();

        Task<Response<Appointment>> AddAppointment(AppointmentModel model);

        Task<Response<Appointment>> GetAppointmentsByUserId(string userId,int month, int year);
        Task<Response<Appointment>> GetAppointmentsByPatientId(int patientId);

        Task<Response<Appointment>> UpdateAppointment(AppointmentModel model);
        Task<Response<Appointment>> DeleteAppointmentById(int appointmentId);
        Task<Response<Appointment>> GetAppointmentByAppointmentId(int appointmentId);

        Task<Response<Appointment>> GetScheduledAppointmentsByuserId(string userId);
        Task DeleteAppointmentByIdForPatientDelete(int appointmentId);

    }
}
