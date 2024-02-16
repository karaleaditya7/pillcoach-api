using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OntrackDb.Entities
{
    [Table("reconciliationSideeffect")]
    public class ReconciliationSideeffect
    {

        [Key]
        public int Id { get; set; }

        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("medicationSubstanceId")]
        public MedicationSubstance MedicationSubstance { get; set; }
        [Column("reactionId")]
        public Reaction Reaction { get; set; }
    }

    internal class ReconciliationSideeffectConfiguration : IEntityTypeConfiguration<ReconciliationSideeffect>
    {
        public void Configure(EntityTypeBuilder<ReconciliationSideeffect> builder)
        {
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey("patientId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
