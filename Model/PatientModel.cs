using OntrackDb.Entities;
using System.Collections.Generic;

namespace OntrackDb.Model
{
    public class PatientModel
    {
        public int Id { get; set; }
        
        public string Status { get; set; }
       
        public string ImageName { get; set; }
       
        public int PharmacyId { get; set; }
        public string Language { get; set; }
        public int PrimaryThirdPartyId { get; set; }
        public Address Address { get; set; }
      
        public Contact Contact { get; set; }
        
        public Note Note { get; set; }
     
        public List<Medication> Medications { get; set; }
        
        public ImportData ImportData { get; set; }
    }
}
