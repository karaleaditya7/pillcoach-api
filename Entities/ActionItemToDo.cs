using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("actionItemToDo")]
    public class ActionItemToDo
    {
        [Key]
        public int Id { get; set; }

        [Column("description")]
        public string Description { get; set; }
    }
}
