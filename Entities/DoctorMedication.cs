using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("doctorMedication")]
    public class DoctorMedication
    {
        [Key]
        public int id { get; set; }

        [Column("doctorPrescribed")]
        public Doctor DoctorPrescribed { get; set; }

        [Column("cmrMedicationId")]
        public CmrMedication CmrMedication { get; set; }
        [Column("medicationId")]
        public Medication Medication { get; set; }
        [Column("otcMedicationId")]
        public OtcMedication OtcMedication { get; set; }
        [Column("medicationReconciliationId")]
        public MedicationReconciliation MedicationReconciliation { get; set; }


    }

    internal class DoctorMedicationConfiguration : IEntityTypeConfiguration<DoctorMedication>
    {
        public void Configure(EntityTypeBuilder<DoctorMedication> builder)
        {
            builder.HasOne(dm => dm.CmrMedication)
                .WithMany()
                .HasForeignKey("cmrMedicationId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.Medication)
                .WithMany()
                .HasForeignKey("medicationId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.OtcMedication)
                .WithMany()
                .HasForeignKey("otcMedicationId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dm => dm.MedicationReconciliation)
                .WithMany()
                .HasForeignKey("medicationReconciliationId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
