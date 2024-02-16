using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    [Table("actionItemReconciliationToDo")]
    public class ActionItemReconciliationToDo
    {
        [Key]
        public int Id { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
