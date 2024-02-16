using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("patientMailList")]
    public class PatientMailList
    {

        [Key]
        public int Id { get; set; }
        [Column("patientName")]
        public string PatientName { get; set; }
        [Column("patientId")]
        public int PatientId { get; set; }
        [Column("contactId")]
        public int ContactId { get; set; }
        [Column("pharmacyId")]
        public int PharmacyId { get; set; }
        [Column("addressId")]
        public Address Address { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phoneNumber")]
        public string PhoneNumber { get; set; }
        [Column("lastUpdated")]
        public DateTime LastUpdated { get; set; }
        [Column("createdDate")]
        public DateTime CreatedDate { get; set; }
        [Column("dateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        [Column("deletedDate")]
        public DateTime DeletedDate { get; set; }
        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }
        [Column("type")]
        public string type { get; set; }
        [Column("sentType")]
        public string SentType { get; set; }
        [Column("createdUser")]
        public string CreatedUser { get; set; }
        [Column("createdBy")]
        public string CreatedBy { get; set; }
        [Column("modifiedUser")]
        public string ModifiedUser { get; set; }
        [Column("pharmacyName")]
        public string PharmacyName { get; set; }

        [Column("lastAccessed")]
        public DateTime LastAccessed { get; set; }
    }
}
