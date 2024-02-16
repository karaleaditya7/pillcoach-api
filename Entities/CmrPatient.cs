using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("cmrPatient")]
    public class CmrPatient
    {
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
        [Column("importDataId")]
        public ImportData ImportData { get; set; }
        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }
        [Column("isCmrType")]
        public Boolean IsCmrType { get; set; }
        [Column("isMedRecType")]
        public Boolean IsMedRecType { get; set; }
        [Column("patientId")]
        public int PatientId { get; set; }

        [NotMapped]
        public Double CholestrolPDC { get; set; }
        [NotMapped]
        public Double DiabetesPDC { get; set; }
        [NotMapped]
        public Double RASAPDC { get; set; }
        public string Name { get; }
        public string NpiNumber { get; }
    }
}
