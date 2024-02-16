using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("patient")]
    public class Patient
    {
        public Patient()
        {

        }
        public Patient(int id, string status, Address address, Contact contact, string name, string npiNumber)
        {
            Id = id;
            Status = status;
            Address = address;
            Contact = contact;
            Name = name;
            NpiNumber = npiNumber;
        }

        [Key]
        public int Id { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("imageName")]
        public string ImageName { get; set; }

        [Column("patientVendorRxID")]
        public string PatientVendorRxID { get; set; }

        [Column("pharmacyId")]
        public Pharmacy Pharmacy { get; set; }
        [Column("addressId")]
        public Address Address { get; set; }
        [Column("contactId")]
        public Contact Contact { get; set; }
        [Column("noteId")]
        public int? NoteId { get; set; }
        public Note Note { get; set; }
        [Column("medicationId")]
        public List<Medication> Medications { get; set; }
        [Column("importDataId")]
        public ImportData ImportData { get; set; }
        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }

        [NotMapped]
        public Double CholestrolPDC { get; set; }
        [NotMapped]
        public Double DiabetesPDC { get; set; }
        [NotMapped]
        public Double RASAPDC { get; set; }
        public string Name { get; }
        public string NpiNumber { get; }

        [Column("importSourceFileId")]
        public int? ImportSourceFileId { get; set; }

        [Column("cholesterolRefillDue")]
        public bool CholesterolRefillDue { get; set; }

        [Column("diabetesRefillDue")]
        public bool DiabetesRefillDue { get; set; }

        [Column("rasaRefillDue")]
        public bool RasaRefillDue { get; set; }
        [Column("primaryThirdPartyId")]
        public PrimaryThirdParty primaryThirdParty { get; set; }
        [Column("language")]
        public string Language { get; set; }
        [NotMapped]
        public string RASAExclusion { get; set; }
        [NotMapped]
        public string DiabetesExclusion { get; set; }
    }

    internal class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasOne(p => p.Pharmacy)
              .WithMany()
              .HasForeignKey("pharmacyId")
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
