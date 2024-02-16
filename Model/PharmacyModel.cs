using OntrackDb.Entities;
using System;
using System.Collections.Generic;

namespace OntrackDb.Model
{
    public class PharmacyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public Contact Contact { get; set; }
        public Note Note { get; set; }
        public string PharmacyManager { get; set; }
        public DateTime LastUpdate { get; set; }
        public string ImageName { get; set; }
        public string NcpdpNumber { get; set; }
        public string NpiNumber { get; set; }
        public List<Patient> Patients { get; set; }
        public ImportData ImportData { get; set; }
        public string TwilioSmsNumber { get; set; }
    }
}
