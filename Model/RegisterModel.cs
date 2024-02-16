using OntrackDb.Authentication;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Model
{
    public class RegisterModel
    {
        public string Id {  get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string JobPosition { get; set; }
        public Licenses Licenses { get; set; }
        public Address Address { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string TwilioPhoneNumber { get; set; }
        public Boolean IsEnabled { get; set; }
        public Boolean IsDisabled { get; set; }
        public string NpiNumber { get; set; }
        public Boolean ImportEnabled { get; set; }
    }
}
