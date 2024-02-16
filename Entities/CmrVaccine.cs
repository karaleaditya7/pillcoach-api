using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("cmrVaccine")]
    public class CmrVaccine
    {
        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("servicetakeawaynformationId")]
        public ServiceTakeawayInformation ServiceTakeawayInformation { get; set; }
    }

    internal class CmrVaccineConfiguration : IEntityTypeConfiguration<CmrVaccine>
    {
        public void Configure(EntityTypeBuilder<CmrVaccine> builder)
        {
            builder.HasOne(a => a.ServiceTakeawayInformation)
                .WithMany()
                .HasForeignKey("servicetakeawaynformationId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
