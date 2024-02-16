using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    public class DoctorPharmacy
    {
        [Key]
        public int Id { get; set; }
        [Column("doctorsId")]
        public int DoctorId { get; set; }
       
        public Doctor Doctor { get; set; }
        [Column("pharmaciesId")]
        public int PharmacyId { get; set; }
        
        public Pharmacy Pharmacy { get; set; }
       
    }
}
