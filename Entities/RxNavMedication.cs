using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Amqp.Framing;
using NetTopologySuite.Algorithm;
using OntrackDb.Authentication;

namespace OntrackDb.Entities
{
    [Table("rxNavMedication")]
    public class RxNavMedication
    {
        [Key]
        public int Id { get; set; }
        [Column("genericName")]
        [MaxLength(500)]
        public string GenericName { get; set; }
        [Column("sbdcName")]
        [MaxLength(500)]
        public string SBDCName { get; set; }
        [Column("ndcNumber")]
        public string NDCNumber { get; set; }


    }
    internal class RxNavMedicationConfiguration : IEntityTypeConfiguration<RxNavMedication>
    {
        public void Configure(EntityTypeBuilder<RxNavMedication> builder)
        {
            // Add indexes here
            builder.HasIndex(e => e.GenericName).HasName("IX_GenericName");
            builder.HasIndex(e => e.SBDCName).HasName("IX_SBDCName");

            builder.Property(e => e.GenericName)
                .HasMaxLength(500)
                .HasAnnotation("Relational:ColumnType", "nvarchar(500)")
                .IsUnicode(false)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)")
                .HasConversion(
                    v => TruncateString(v, 500),
                    v => v);

            builder.Property(e => e.SBDCName)
                .HasMaxLength(500)
                .HasAnnotation("Relational:ColumnType", "nvarchar(500)")
                .IsUnicode(false)
                .IsRequired(false)
                .HasColumnType("nvarchar(500)")
                .HasConversion(
                    v => TruncateString(v, 500),
                    v => v);
        }

        private string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }

}
