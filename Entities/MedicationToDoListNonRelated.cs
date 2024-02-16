using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{

    [Table("medicationToDoListNonRelated")]
    public class MedicationToDoListNonRelated
    {
        [Key]
        public int Id { get; set; }
        [Column("medicationToDoList")]
        public MedicationToDoList MedicationToDoList { get; set; }
        [Column("medicationRelated")]
        public MedicationRelated MedicationRelated { get; set; }
    }
}
