using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("nonRelatedMedicationToDo")]
    public class NonRelatedMedicationToDo
    {
        [Key]
        public int Id { get; set; }

        [Column("patientId")]
        public Patient Patient { get; set; }

        [Column("actionItemToDoId")]
        public ActionItemToDo ActionItemToDo { get; set; }

        [Column("patientToDo")]
        public string PatientToDo { get; set; }
    }

    internal class NonRelatedMedicationToDoConfiguration : IEntityTypeConfiguration<NonRelatedMedicationToDo>
    {
        public void Configure(EntityTypeBuilder<NonRelatedMedicationToDo> builder)
        {

            builder.HasOne(m => m.Patient)
            .WithMany()
            .HasForeignKey("patientId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
