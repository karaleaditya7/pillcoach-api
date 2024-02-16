using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("medicationRelated")]
    public class MedicationRelated
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
