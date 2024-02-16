using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("appointment")]
    public class Appointment
    {
        [Key]
        public int ID { get; set; }
        [Column("notes")]
        public string Notes { get; set; }

        [Column("startDateTime")]
        public DateTime StartDateTime { get; set; }

        [Column("duration")]
        public string Duration { get; set; }

        [Column("patientId")]
        public Patient Patient { get; set; }

        [Column("userId")]
        public User User { get; set; }

        [Column("isCancel")]

        public Boolean IsCancel { get; set; }

        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }

        [NotMapped]
        public DateTime EndDateTime { get; set; }

        [NotMapped]
        public string StrStartDateTime { get; set; }

        [NotMapped]
        public string StrEndDateTime { get; set; }
    }

    internal class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder
                .HasOne(a => a.Patient)
                .WithMany().HasForeignKey("patientId")
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(a => a.User)
                .WithMany().HasForeignKey("userId")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
