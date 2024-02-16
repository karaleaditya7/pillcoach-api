using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OntrackDb.Entities
{
    [Table("vaccineReconciliation")]
    public class VaccineReconciliation
    {

        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("serviceTakeAwayMedReconciliationId")]
        public ServiceTakeAwayMedReconciliation ServiceTakeAwayMedReconciliation { get; set; }
    }

    internal class VaccineReconciliationConfiguration : IEntityTypeConfiguration<VaccineReconciliation>
    {
        public void Configure(EntityTypeBuilder<VaccineReconciliation> builder)
        {
            builder.HasOne(a => a.ServiceTakeAwayMedReconciliation)
                .WithMany()
                .HasForeignKey("serviceTakeAwayMedReconciliationId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
