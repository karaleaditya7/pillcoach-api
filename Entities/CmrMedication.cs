using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("cmrMedication")]
    public class CmrMedication
    {

        [Key]
        public int Id { get; set; }

        [Column("rxNumber")]
        public string RxNumber { get; set; }

        [Column("rxVendorRxID")]
        public string RxVendorRxID { get; set; }

        [Column("rxDate")]
        public DateTime RxDate { get; set; }
        [Column("drugName")]
        public string DrugName { get; set; }
        [Column("direction")]
        public string Direction { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("doctorPrescribed")]
        public Doctor DoctorPrescribed { get; set; }
        [Column("supply")]
        public int Supply { get; set; }
        [Column("prescriberName")]
        public string PrescriberName { get; set; }
        [Column("lastFillDate")]
        public DateTime LastFillDate { get; set; }
        [Column("nextFillDate")]
        public DateTime NextFillDate { get; set; }
        [Column("payDue")]
        public decimal PayDue { get; set; }
        [Column("rfNumber")]
        public string RfNumber { get; set; }
        [Column("conditionTreated")]
        public string ConditionTreated { get; set; }
        [Column("importDataId")]
        public ImportData ImportData { get; set; }
        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("condition")]
        public string Condition { get; set; }

        [Column("optionalCondition")]
        public string OptionalCondition { get; set; }
        [Column("refillsRemaining")]
        public string RefillsRemaining { get; set; }

        [Column("ndcNumber")]
        public string NDCNumber { get; set; }
        [Column("drugSubGroup")]
        public string DrugSubGroup { get; set; }
        [NotMapped]
        public List<Doctor> Doctor { get; set; }

        [NotMapped]
        public int RelatedPharmacies { get; set; }
        [NotMapped]
        public int AssignedPatient { get; set; }

        [NotMapped]
        public Double AdherenceRate { get; set; }

        [Column("genericName")]
        public string GenericName { get; set; }

        [Column("isInclude")]
        public Boolean IsInclude { get; set; }

        [Column("isAttached")]
        public Boolean IsAttached { get; set; }

        [Column("isExclude")]
        public Boolean IsExclude { get; set; }
        [Column("sbdcName")]
        public string SBDCName { get; set; }

        [NotMapped]
        public Boolean IsValidateCmr { get; set; }
    }

    internal class CmrMedicationConfiguration : IEntityTypeConfiguration<CmrMedication>
    {
        public void Configure(EntityTypeBuilder<CmrMedication> builder)
        {
            builder.Property(b => b.PayDue)
               .HasColumnType("decimal(18, 2)");

            builder.HasOne(b => b.Patient)
            .WithMany()
            .HasForeignKey("patientId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
