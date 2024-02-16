using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("notes")]
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public string text { get; set; }

        public Patient Patient { get; set; }

        public Pharmacy pharmacy { get; set; }
        [Column("lastUpdated")]
        public System.DateTime LastUpdated { get; set; }

        [Column("userId")]
        [Newtonsoft.Json.JsonIgnore]
        public string UserId { get; set; }

        [NotMapped]
        public string LastUpdatedBy { get; set; } = string.Empty;
    }

    internal class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
