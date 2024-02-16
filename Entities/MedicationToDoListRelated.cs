using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("medicationToDoListRelated")] 
    public class MedicationToDoListRelated
    {
        [Key]
        public int Id { get; set; }
        [Column("medicationToDoList")]
        public MedicationToDoList MedicationToDoList { get; set; }
        [Column("medicationRelated")]
        public MedicationRelated MedicationRelated { get; set; }
    }
}
