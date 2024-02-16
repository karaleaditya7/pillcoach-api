using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace OntrackDb.Authentication
{
    public class User : IdentityUser
    {
        public User()
        {
            PasswordHistory = new List<UserPasswordHistory>();
        }

        [Column("firstName")]
        public string FirstName { get; set; }
        [Column("lastName")]
        public string LastName { get; set; }
        [Column("dateOfBirth")] 
        public DateTime DateOfBirth { get; set; }
        [Column("jobPosition")] 
        public string JobPosition { get; set; }
        [Column("licenseId")]
        public Licenses Licenses { get; set; }
        [Column("addressId")]
        public Address Address { get; set; }
        [Column("isDeleted")] 
        public Boolean IsDeleted { get; set; }
        [Column("imageName")] 
        public string ImageName { get; set; }
        [Column("lastLogin")] 
        public DateTime LastLogin {get;set;}

        [Column("twilioPhoneNumber")]
        public string TwilioPhoneNumber { get; set; }

        [Column("twilioNumberAssignedOnUtc")]
        public DateTime? TwilioNumberAssignedOnUtc { get; set; }

        public IList<PharmacyUser> PharmacyUsers { get; set; }

        [NotMapped]
        public string ImageUri { get; set; }
        [NotMapped]
        public IList<string> RoleList { get; set; }
        [NotMapped]
        public int NewPatient { get; set; }
        [NotMapped]
        public int PatientInProgress { get; set; }
        [NotMapped]
        public int DueForRefill { get; set; }
        [NotMapped]
        public int NoRefillRemaining { get; set; }

        [NotMapped]
        public Double CholestrolPDC { get; set; } 
        [NotMapped]
        public Double DiabetesPDC { get; set; } 
        [NotMapped]
        public Double RASAPDC { get; set; } 

        [Column("isNotification")]
        public Boolean IsNotification { get; set; } = true;

        [Column("isEnabled")]
        public Boolean IsEnabled { get; set; } = false;

        [Column("isDisabled")]
        public Boolean IsDisabled { get; set; } = false;

        [Column("createdDate")]
        public DateTime CreatedDate { get; set; }

        [Column("lastDeviceId", TypeName = "varchar"), MaxLength(50)]
        public string LastDeviceId { get; set; }

        [Column("lastVerifiedDateUTC")]
        public DateTime? LastVerifiedDateUTC { get; set; }

        [Column("verificationCode", TypeName = "varchar"), MaxLength(10)]
        public string VerificationCode { get; set; }

        [Column("codeExpiryDateUTC")]
        public DateTime? CodeExpiryDateUTC { get; set; }

        [Column("complianceId")]
        public UserCompliance Compliance { get; set; }

        [Column("passwordSetDateUTC")]
        public DateTime? PasswordSetDateUTC { get; set; }

        [Column("failedLoginCount")]
        public int FailedLoginCount { get; set; }

        [Column("importEnabled")]
        public Boolean ImportEnabled { get; set; } = false;

        [NotMapped]
        public Boolean IsMasterUser { get; set; }
        public virtual IList<UserPasswordHistory> PasswordHistory { get; set; }
    }
}
