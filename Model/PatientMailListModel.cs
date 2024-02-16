using OntrackDb.Authentication;
using OntrackDb.Entities;
using System;

namespace OntrackDb.Model
{
    public class PatientMailListModel
    {

        public int Id { get; set; }
        public string PatientName { get; set; }
      
        public int PatientId { get; set; }
      
        public int PharmacyId { get; set; }
        public int ContactId { get; set; }
        
        public Address Address { get; set; }
     
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }
       
        public DateTime LastUpdated { get; set; }
       
        public DateTime CreatedDate { get; set; }
        
        public DateTime DeletedDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        
        public Boolean IsDeleted { get; set; }
      
        public string type { get; set; }
        public string SentType { get; set; }
      
        public string CreatedUser { get; set; }
       
        public string ModifiedUser { get; set; }
      
        public string PharmacyName { get; set; }
        public string CreatedBy { get; set; }
    }
}
