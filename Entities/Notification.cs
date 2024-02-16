using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("notification")]
    public class Notification
    {
        [Key]
        public int id { get; set; }

        [Column("userId")]
        public User User { get; set; }

        [Column("messageId")]
        public Message Message { get; set; }

        [Column("appointmentId")]
        public Appointment Appointment { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("sendDateTime")]
        public DateTime SendDateTime { get; set; }

        [Column("readDateTime")]
        public DateTime ReadDateTime { get; set; }

        [Column("notificationType")]
        public string NotificationType { get; set; }

        [NotMapped]
        public Boolean IsRead { get; set; }

        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }
    }

    internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasOne(an => an.User)
                .WithMany()
                .HasForeignKey("userId")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(an => an.Message)
                .WithMany()
                .HasForeignKey("messageId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(an => an.Appointment)
                .WithMany()
                .HasForeignKey("appointmentId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
