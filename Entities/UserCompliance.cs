using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities;

[Table("UserCompliances")]
public class UserCompliance
{
    public int Id { get; set; }

    public bool BackgroundCheck { get; set; }
    public bool LiabilityInsurance { get; set; }

    public bool AnnualHIPPATraining { get; set; }
    public DateTime? HippaTrainingRecordedOn { get; set; }

    public bool AnnualFraudTraining { get; set; }
    public DateTime? FraudTrainingRecordedOn { get; set; }

    public bool AnnualCyberTraining { get; set; }
    public DateTime? CyberTrainingRecordedOn { get; set; }
}

internal class UserComplianceConfiguration : IEntityTypeConfiguration<UserCompliance>
{
    public void Configure(EntityTypeBuilder<UserCompliance> builder)
    {
        builder.Property(b => b.BackgroundCheck)
            .HasDefaultValueSql("0");

        builder.Property(b => b.LiabilityInsurance)
            .HasDefaultValueSql("0");

        builder.Property(b => b.AnnualHIPPATraining)
            .HasDefaultValueSql("0");

        builder.Property(b => b.AnnualFraudTraining)
            .HasDefaultValueSql("0");

        builder.Property(b => b.AnnualCyberTraining)
            .HasDefaultValueSql("0");
    }
}
