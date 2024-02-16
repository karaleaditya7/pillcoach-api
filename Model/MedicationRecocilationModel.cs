namespace OntrackDb.Model
{
    public class MedicationRecocilationModel
    {
        public int Id { get; set; }
        public string Condition { get; set; }
        public string Direction { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string SBDCName { get; set; }
        public string ActionItem { get; set; }
    }
}
