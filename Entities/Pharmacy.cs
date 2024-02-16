using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("pharmacy")]
    public class Pharmacy
    {
        public Pharmacy() { }

        public Pharmacy(int id, string name, string pharmacyManager, Address address, Contact contact, string npiNumber, string ncpdpNumber, string twilioSmsNumber)
        {
            Id = id;
            Name = name;
            PharmacyManager = pharmacyManager;
            Address = address;
            Contact = contact;
            NpiNumber = npiNumber;
            NcpdpNumber = ncpdpNumber;
            TwilioSmsNumber = twilioSmsNumber;
        }

        [Key]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        [Column("pharmacyVendorRxId")]
        public string PharmacyVendorRxID { get; set; }

        [Column("pharmacyManager")]
        public string PharmacyManager { get; set; }

        [Column("lastUpdate")]
        public DateTime LastUpdate { get; set; }

        [Column("imageName")]
        public string ImageName { get; set; }
        [Column("ncpdpNumber")]
        public string NcpdpNumber { get; set; }
        [Column("npiNumber")]
        public string NpiNumber { get; set; }
        [Column("contactId")]
        public Contact Contact { get; set; }
        [Column("addressId")]
        public Address Address { get; set; }
        [Column("patientId")]
        public List<Patient> Patients { get; set; }
        [Column("doctorId")]
        public List<Doctor> Doctors { get; set; }
        [Column("importDataId")]
        public ImportData ImportData { get; set; }
        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }

        [Column("noteId")]
        public Note Note { get; set; }

        public IList<PharmacyUser> PharmacyUsers { get; set; }

        [Column("noteId")]
        public int? NoteId { get; set; }

        [Column("twilioSmsNumber")]
        public string TwilioSmsNumber { get; set; }

        [NotMapped]
        public int UpcomingRefill { get; set; }
        [NotMapped]
        public int NewPatient { get; set; }
        [NotMapped]
        public Double CholestrolPDC { get; set; }
        [NotMapped]
        public Double DiabetesPDC { get; set; }
        [NotMapped]
        public Double RASAPDC { get; set; }

        [NotMapped]
        public Dictionary<string, int> PatientCount { get; set; }
        [NotMapped]
        public int TotalCholesterolPatient { get; set; }
        [NotMapped]
        public int TotalDiabetesPatient { get; set; }
        [NotMapped]
        public int TotalRASAPatient { get; set; }
    }

    internal class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
    {
        public void Configure(EntityTypeBuilder<Pharmacy> builder)
        {
            builder.Property(b => b.TwilioSmsNumber)
                .IsUnicode(false)
                .HasMaxLength(15);
        }
    }
}
