using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace OntrackDb.Entities;

public class PatientPDC
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime PdcMonth { get; set; }
    public int DurationMonths { get; set; }
    public string Condition { get; set; }
    public decimal PDC { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public int TotalFills { get; set; }
    public bool HasExclusions { get; set; }
    public int Consumptions { get; set; }
}

internal class PatientPDCConfiguration : IEntityTypeConfiguration<PatientPDC>
{
    public void Configure(EntityTypeBuilder<PatientPDC> builder)
    {
        builder.ToTable("patientPDC");

        builder.Property(b => b.PdcMonth)
            .HasColumnType("date");

        builder.Property(b => b.Condition)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(20);

        builder.Property(b => b.PDC)
            .HasColumnType("decimal(5, 2)");

        builder.Property(b => b.StartDate)
            .HasColumnType("date");

        builder.Property(b => b.EndDate)
            .HasColumnType("date");

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.PatientId, b.PdcMonth, b.DurationMonths, b.Condition })
            .IsUnique();
    }
}
