using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class AppointmentData :IAppointmentData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public AppointmentData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;

        }
        public async Task<Appointment> AddAppointment(Appointment appointment)
        {
            var result = await _applicationDbcontext.Appointments.AddAsync(appointment);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<Appointment>> GetScheduledAppointmentsByuserId(string userId)
        {
            var appointments = await _applicationDbcontext.Appointments.Where(a => a.User.Id == userId && !a.IsCancel && a.StartDateTime.Date == DateTime.Today).
           Include(p => p.Patient).
           Include(p => p.Patient.Contact).
           ToListAsync();

            return appointments;
        }

        public async Task<Appointment> GetAppointmentByUserIdAndDate(string userId, DateTime date)
        {
            var appointment = await _applicationDbcontext.Appointments.Include(u => u.User).
            Where(p => p.User.Id == userId && p.StartDateTime == date).
            FirstOrDefaultAsync();
            return appointment;
        }

        public async Task<int> UpdateAppointment(Appointment appointment)
        {
            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;
        }

        public async Task DeleteAppointment(Appointment appointment)
        {
             _applicationDbcontext.Appointments
                .Remove(appointment);

           await _applicationDbcontext.SaveChangesAsync();
        }

        public void DeleteAppointmentForPatient(Appointment appointment)
        {
             _applicationDbcontext.Appointments
                .Remove(appointment);

           
        }

        public async Task<List<Appointment>> GetAppointments()
        {

            var appointments = await _applicationDbcontext.Appointments.
            Include(p => p.Patient).
            Include(u => u.User).
            ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetAppointmentsByDate(DateTime startDateTime)
        {

            var appointments = await _applicationDbcontext.Appointments.Where(a => a.StartDateTime.CompareTo(startDateTime) >= 0 && !a.IsCancel && !a.IsDeleted).
            Include(p => p.Patient).
            Include(u => u.User).
            ToListAsync();
            return appointments;
        }

        public async Task<List<Appointment>> GetAppointmentsByUserId(string userId)
        {

            var appointments = await _applicationDbcontext.Appointments.Where(a => a.User.Id == userId && !a.IsCancel).
            Include(p => p.Patient).
            Include(p =>p.Patient.Contact).
            ToListAsync();

            for (int i = 0; i < appointments.Count; i++)
            {
                int val;
                int.TryParse(appointments[i].Duration, out val);
                
                appointments[i].EndDateTime = appointments[i].StartDateTime.AddMinutes(val);
            }
            return appointments;
        }

        public async Task<Appointment> GetAppointmentByAppointmentId(int appointmentId)
        {

            var appointment = await _applicationDbcontext.Appointments.Include(u=>u.User).
            Include(p => p.Patient).
            Where(p => p.ID == appointmentId && !p.IsCancel).
            FirstOrDefaultAsync();

            int val;
            int.TryParse(appointment.Duration, out val);
            appointment.EndDateTime = appointment.StartDateTime.AddMinutes(val);
            return appointment;
        }

        public async Task<Appointment> GetAllAppointmentByAppointmentId(int appointmentId)
        {

            var appointment = await _applicationDbcontext.Appointments.Include(u => u.User).
            Include(p => p.Patient).
            Where(p => p.ID == appointmentId ).
            FirstOrDefaultAsync();
            return appointment;
        }


        public async Task<List<Appointment>> GetAppointmentsByPatientId(int patientId)
        {

            var appointments = await _applicationDbcontext.Appointments.Where(a => a.Patient.Id == patientId).
            Include(u => u.User).
            Include(p=>p.Patient).
            Include(p=>p.Patient.Contact).
            ToListAsync();
            return appointments;
        }
       
    }
}
