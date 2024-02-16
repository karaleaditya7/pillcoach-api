using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("doctor")]
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        [Column("npi")]
        public string Npi { get; set; }

        [Column("prescriberVendorRxID")]
        public string PrescriberVendorRxID { get; set; }

        [Column("contactId")]
        public Contact Contact { get; set; }
        [Column("pharmacyId")]
        public List<Pharmacy> Pharmacies { get;set; }
        [Column("importDataId")]
        public ImportData ImportData { get; set; }

        [Column("importSourceFileId")]
        public int? ImportSourceFileId { get; set; }
    }
}

