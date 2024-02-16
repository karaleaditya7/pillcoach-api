using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("medicationToDoList")]
    public class MedicationToDoList
    {
        [Key]
        public int Id { get; set; }

        [Column("cmrMedication")]
        public CmrMedication CmrMedication { get; set; }

        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }
}
