namespace OntrackDb.Model
{
    public class ReconciliationToDoListModel
    {

        public int Id { get; set; }
        public int MedicationReconciliationId { get; set; }
        public int PatientId { get; set; }
        public string PatientToDo { get; set; }
    }
}
