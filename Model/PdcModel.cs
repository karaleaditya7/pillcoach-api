using System;

namespace OntrackDb.Model
{
    public class PdcModel
    {
        public DateTime Date { get; set; }
        public Double Value { get; set; }
    }

    public class PdcModelEx : PdcModel
    {
        public string Condition { get; set; }
        public int DurationMonths { get; set; }
        public int TotalPatients { get; set; }
        public int AdherenceCount { get; set; }
        public int NonAdherenceCount => TotalPatients - AdherenceCount;
    }
}
