using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace OntrackDb.Entities;

public class RefillDueActivationQueue
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int MedicationId { get; set; }
    public DateTime ActivationDate { get; set; }
}

internal class RefillDueActivationQueueCOnfiguration : IEntityTypeConfiguration<RefillDueActivationQueue>
{
    public void Configure(EntityTypeBuilder<RefillDueActivationQueue> builder)
    {
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.ActivationDate)
            .HasColumnType("date");
    }
}
