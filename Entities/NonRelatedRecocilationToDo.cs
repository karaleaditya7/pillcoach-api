using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OntrackDb.Entities
{
    [Table("nonRelatedRecocilationToDo")]
    public class NonRelatedRecocilationToDo
    {
        [Key]
        public int Id { get; set; }

        [Column("patientId")]
        public Patient Patient { get; set; }

        [Column("actionItemReconciliationToDoId")]
        public ActionItemReconciliationToDo ActionItemReconciliationToDo { get; set; }

        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }
    internal class NonRelatedRecocilationToDoConfiguration : IEntityTypeConfiguration<NonRelatedRecocilationToDo>
    {
        public void Configure(EntityTypeBuilder<NonRelatedRecocilationToDo> builder)
        {
            builder.HasOne(m => m.Patient)
            .WithMany()
            .HasForeignKey("patientId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
