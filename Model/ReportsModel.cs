using System;

namespace OntrackDb.Model
{
    public class ReportsModel
    {
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyManager { get; set; }
        public string NcpdpNumber { get; set; }
        public string NpiNumber { get; set; }
        public Double CholestrolPDC { get; set; }
        public Double DiabetesPDC { get; set; }
        public Double RASAPDC { get; set; }
        public int NewPatient { get; set; }
        public int UpcomingRefill { get; set; }
    }
}
