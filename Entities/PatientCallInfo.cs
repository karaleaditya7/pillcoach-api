using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;

namespace OntrackDb.Entities;

public class PatientCallInfo
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string UserId { get; set; }
    public DateTime CallDate { get; set; }
    public int AttemptNo { get; set; }
    public string CallReason { get; set; }
    public string Notes { get; set; }
    public string MedicationsDiscussedJson { get; set; }

    public User User { get; set; }
}

internal class PatientCallInfoConfiguration : IEntityTypeConfiguration<PatientCallInfo>
{
    public void Configure(EntityTypeBuilder<PatientCallInfo> builder)
    {
        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.CallReason)
            .IsRequired()
            .IsUnicode(false)
            .HasMaxLength(50);
    }
}
