using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("nonMedicationRelated")]
    public class NonMedicationRelated
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
