using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities;

public class AuditLog
{
    public int Id { get; set; }

    public DateTime LogDateUTC { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; }

    public int? PatientId { get; set; }

    public int ActionTypeId { get; set; }

    public int ActionSourceId { get; set; }

    public Patient Patient { get; set; }
    public User User { get; set; }
    public AuditActionType ActionType { get; set; }
    public AuditActionSourceType SourceType { get; set; }
}

internal class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasOne<Patient>(e => e.Patient)
            .WithMany()
            .HasForeignKey(e => e.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AuditActionType>(e => e.ActionType)
            .WithMany()
            .HasForeignKey(e => e.ActionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AuditActionSourceType>(e => e.SourceType)
            .WithMany()
            .HasForeignKey(e => e.ActionSourceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}