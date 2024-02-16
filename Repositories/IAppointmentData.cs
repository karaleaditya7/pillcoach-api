using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IAppointmentData
    {
        Task<Appointment> AddAppointment(Appointment appointment);
        Task<List<Appointment>> GetAppointments();
        Task<List<Appointment>> GetAppointmentsByDate(DateTime startDateTime);

        Task<List<Appointment>> GetAppointmentsByUserId(string userId);

        Task<List<Appointment>> GetAppointmentsByPatientId(int patientId);

        Task<Appointment> GetAppointmentByAppointmentId(int appointmentId);

        Task<int> UpdateAppointment(Appointment appointment);

        Task DeleteAppointment(Appointment appointment);

        Task<Appointment> GetAppointmentByUserIdAndDate(string userId, DateTime date);

        Task<List<Appointment>> GetScheduledAppointmentsByuserId(string userId);
        void DeleteAppointmentForPatient(Appointment appointment);
        Task<Appointment> GetAllAppointmentByAppointmentId(int appointmentId);



    }
}
