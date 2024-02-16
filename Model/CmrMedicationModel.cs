namespace OntrackDb.Model
{
    public class CmrMedicationModel
    {
        public int Id { get; set; }
        public string Condition { get; set; }
        public string Direction { get; set; }
        public int DoctorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int PatientId { get; set; }
        public string SBDCName { get; set; }
        
    }
}
