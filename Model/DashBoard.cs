namespace OntrackDb.Model
{
    public class DashBoard
    {
        public int AssignedPharmacy { get; set; }
        public int NewPatient { get; set; }
        public int PatientInProgress { get; set; }
        public int DueForRefill { get; set; }
        public int NoRefillRemaining { get; set; }
    }
}
