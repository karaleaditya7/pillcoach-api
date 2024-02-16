using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    [Table("recocilationToDoRelated")]
    public class ReconciliationToDoRelated
    {
        [Key]
        public int Id { get; set; }
        [Column("medicationReconciliationId")]
        public MedicationReconciliation MedicationReconciliation { get; set; }
        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }
}
