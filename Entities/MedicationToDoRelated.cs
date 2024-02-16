using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{

    [Table("medicationToDoRelated")]
    public class MedicationToDoRelated
    {
        [Key]
        public int Id { get; set; }
        [Column("cmrMedicationId")]
        public CmrMedication CmrMedication { get; set; }
        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }
}
