using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;

namespace OntrackDb.Entities;

public class UserPasswordHistory
{
    public UserPasswordHistory()
    {
        CreatedDateUTC = DateTime.UtcNow;
    }

    public string UserID { get; set; }
    public string HashPassword { get; set; }
    public DateTime CreatedDateUTC { get; set; }
    
    public virtual User AppUser { get; set; }
}

internal class UserPasswordHistoryConfiguration : IEntityTypeConfiguration<UserPasswordHistory>
{
    public void Configure(EntityTypeBuilder<UserPasswordHistory> builder)
    {
        builder.HasKey(b => new { b.UserID, b.HashPassword });

        builder.HasOne(b => b.AppUser)
            .WithMany(u => u.PasswordHistory)
            .HasForeignKey(b => b.UserID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
