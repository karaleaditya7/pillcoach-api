using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    public class MedicationConsumption
    {
        public int Id { get; set; }
        [Column("rxVendorRxID")]
        public string RxVendorRxID { get; set; }
        [Column("rxNumber")]
        public string RxNumber { get; set; }
        [Column("patientId")]
        public int PatienId { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("ndcNumber")]
        public string NDCNumber { get; set; }
        [Column("drugSubGroup")]
        public string DrugSubGroup { get; set; }
        [Column("condition")]
        public string Condition { get; set; }
        [Column("status")]
        public Boolean Status { get; set; }

        [Column("importSourceFileId")]
        public int? ImportSourceFileId { get; set; }

        [Column("medicationId")]
        public int? MedicationId { get; set; }
    }

    internal class MedicationConsumptionConfiguration : IEntityTypeConfiguration<MedicationConsumption>
    {
        public void Configure(EntityTypeBuilder<MedicationConsumption> builder)
        {
            builder.HasOne<Medication>()
                .WithMany()
                .HasForeignKey(b => b.MedicationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
