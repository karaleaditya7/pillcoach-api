using System;

namespace OntrackDb.Dto
{
    public class PharmacyDto
    {
        public int Id { get; set; }
        public int UpcomingRefill { get; set; }
       
        public int NewPatient { get; set; }
       
        public Double CholestrolPDC { get; set; } // PENDING: this will be removed after the UI is updated to use PDC from CholesterolSummary

        public Double DiabetesPDC { get; set; } // PENDING: this will be removed after the UI is updated to use PDC from DiabetesSummary

        public Double RASAPDC { get; set; } // PENDING: this will be removed after the UI is updated to use PDC from RASASummary

        public Boolean IsDeleted { get; set; }


        public PharmacyPdcSummaryDto CholesterolSummary { get; set; }
        public PharmacyPdcSummaryDto DiabetesSummary { get; set; }
        public PharmacyPdcSummaryDto RASASummary { get; set; }
    }

    public class PharmacyPdcSummaryDto
    {
        public double PDC { get; set; }
        public bool DueForRefill { get; set; }
        public int TotalPatient { get; set; }
        public int NonAdherenceCount { get; set; }
    }
}
