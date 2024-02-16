using OntrackDb.Authentication;
using OntrackDb.Entities;
using System;

namespace OntrackDb.Model
{
    public class AppointmentModel
    {
        public int ID { get; set; }
       
        public string Notes { get; set; }

        public DateTime StartDateTime { get; set; }
        
        public string Duration { get; set; }

        public int PatientId { get; set; }

        public string UserId { get; set; }

        public Boolean IsCancel { get; set; }

        public Boolean IsDeleted { get; set; }
        public Boolean IsReschedule { get; set; }

        public DateTime Reschedule { get; set; }
    }
}
