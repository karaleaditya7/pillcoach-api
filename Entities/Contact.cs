using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("contact")]
    public class Contact
    {
        [Key]
        public int Id {  get; set; }
        [Column("firstName")]
        public string FirstName { get; set; }
        [Column("lastName")]
        public string LastName { get; set; }
        [Column("primaryPhone")]
        public string PrimaryPhone { get; set; }
        [Column("secondaryPhone")]
        public string SecondaryPhone { get; set; }
        [Column("primaryEmail")]
        public string PrimaryEmail { get; set; }
        [Column("secondaryEmail")]
        public string SecondaryEmail { get; set; }
        [Column("fax")]
        public string Fax { get; set; }
        [Column("dob")] 
        public DateTime DoB { get; set; }

        [Column("consentForEmail")]
        public bool? ConsentForEmail { get; set; }
        
        [Column("consentForText")]
        public bool? ConsentForText { get; set; }

        [Column("consentForCall")]
        public bool? ConsentForCall { get; set; }

        [Column("consentForBirthdaySms")]
        public bool? ConsentForBirthdaySms { get; set; }
    }
}
