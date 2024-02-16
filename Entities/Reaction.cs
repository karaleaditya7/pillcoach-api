using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("reaction")]
    public class Reaction
    {
        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
     
    }
}
