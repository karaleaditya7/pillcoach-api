using OntrackDb.Entities;

namespace OntrackDb.Model
{
    public class MedicationToDoListModel
    {
        public int Id { get; set; }
        public int CmrMedicationId { get; set; }
        public int PatientId { get; set; }
        public string PatientToDo { get; set; }
    }
}
