using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("takeawayVerify")]
    public class TakeawayVerify
    {
        [Key]
        public int Id { get; set; }
        [Column("uUID")]
        public string UUID { get; set; }
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }
        [Column("patientId")]
        public int PatientId { get; set;}
        [Column("isServiceTakeAwayInfo")]
        public Boolean IsServiceTakeAwayInfo { get; set;}
        [Column("isServiceTakeAwayMedRec")]
        public Boolean IsServiceTakeAwayMedRec { get; set;}
        [Column("lastModified")]
        public DateTime LastModified { get; set; }


    }
}
