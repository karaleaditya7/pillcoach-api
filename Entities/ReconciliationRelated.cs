using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    [Table("reconciliationRelated")]
    public class ReconciliationRelated
    {
        [Key]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
