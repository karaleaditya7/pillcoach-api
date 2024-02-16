using OntrackDb.Entities;
using System;
using System.Collections.Generic;

namespace OntrackDb.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
      
        public string LastName { get; set; }
   
        public DateTime DateOfBirth { get; set; }
        
        public string JobPosition { get; set; }
        
        public Licenses Licenses { get; set; }
        
        public Address Address { get; set; }
        
        public Boolean IsDeleted { get; set; }
        public string ImageName { get; set; }
        
        public DateTime LastLogin { get; set; }
        public string TwilioPhoneNumber { get; set; }

        public IList<PharmacyUser> PharmacyUsers { get; set; }
        public string ImageUri { get; set; }
     
        public IList<string> RoleList { get; set; }
        
        public int NewPatient { get; set; }
      
        public int PatientInProgress { get; set; }
        
        public int DueForRefill { get; set; }
        
        public int NoRefillRemaining { get; set; }
        public Double CholestrolPDC { get; set; }
       
        public Double DiabetesPDC { get; set; }
      
        public Double RASAPDC { get; set; }

        public Boolean IsNotification { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public Boolean EmailConfirmed { get; set; }

        public Boolean IsEnabled { get; set; }
        public Boolean IsDisabled { get; set; }
        public Boolean ImportEnabled { get; set; }
    }
}
