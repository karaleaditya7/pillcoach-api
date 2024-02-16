using System;

namespace OntrackDb.Dto
{
    public class UserPDCDto
    {
        public string Id { get; set; }
        public Double CholestrolPDC { get; set; }
      
        public Double DiabetesPDC { get; set; }
     
        public Double RASAPDC { get; set; }

        public UserPdcSummaryDto CholesterolSummary { get; set; }
        public UserPdcSummaryDto DiabetesSummary { get; set; }
        public UserPdcSummaryDto RASASummary { get; set; }
    }

    public class UserPdcSummaryDto
    {
        public double PDC { get; set; }
        public int TotalPatients { get; set; }
        public int NonAdherenceCount { get; set; }
    }
}
