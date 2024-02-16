using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace OntrackDb.Entities
{
    [Table("otcMedication")]
    public class OtcMedication
    {
        [Key]
        public int Id { get; set; }

        [Column("direction")]
        public string Direction { get; set; }

        [Column("doctorPrescribed")]
        public Doctor DoctorPrescribed { get; set; }
  
        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("condition")]
        public string Condition { get; set; }


        [Column("sbdcName")]
        public string SBDCName { get; set; }

        [Column("gpckName")]
        public string GPCKName { get; set; }

        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }
        [Column("isCmrCreated")]
        public Boolean IsCmrCreated { get; set; }
        [Column("isRecCreated")]
        public Boolean IsRecCreated { get; set; }


    }
}
