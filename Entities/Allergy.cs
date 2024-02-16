using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("allergy")]
    public class Allergy
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

    internal class AllergyConfiguration : IEntityTypeConfiguration<Allergy>
    {
        public void Configure(EntityTypeBuilder<Allergy> builder)
        {
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey("patientId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
