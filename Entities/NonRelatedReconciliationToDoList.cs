using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    [Table("nonRelatedReconciliationToDoList")]
    public class NonRelatedReconciliationToDoList
    {
        [Key]
        public int Id { get; set; }

        [Column("patientId")]
        public Patient Patient { get; set; }

        [Column("actionItemToDoId")]
        public ActionItemToDo ActionItemToDo { get; set; }

        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }
}
