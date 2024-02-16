using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OntrackDb.Entities
{
    [Table("serviceTakeAwayMedReconciliation")]
    public class ServiceTakeAwayMedReconciliation
    {

        [Key]
        public int Id { get; set; }
        #nullable enable
        [Column("recCompleted")]
        public String? RecCompleted { get; set; }
        [Column("isVaccination")]
        public Boolean? IsVaccination { get; set; }
        [Column("isDiscussExerciseDiet")]
        public Boolean? IsDiscussExerciseDiet { get; set; }
        [Column("patientTakeawayDeliveryDate")]
        public String? PatientTakeawayDeliveryDate { get; set; }
        [Column("isPatientCongnitivelyImpaired")]
        public Boolean? IsPatientCongnitivelyImpaired { get; set; }
        [Column("isFollowUpAppointment")]
        public Boolean? IsFollowUpAppointment { get; set; }
        [Column("isPatientLongTermFacility")]
        public Boolean? IsPatientLongTermFacility { get; set; }
        [NotMapped]
        public Boolean? IsValidate { get; set; }

        [NotMapped]
        public Boolean? IsValidateReview { get; set; }
        #nullable disable    
        [Column("vaccineName")]
        public string VaccineName { get; set; }
        [Column("recSendType")]
        public string RecSendType { get; set; }
        [Column("recReceiveType")]
        public string RecReceiveType { get; set; }

        [Column("recReceiveTypeFirstName")]
        public string RecReceiveTypeFirstName { get; set; }

        [Column("recReceiveTypeLastName")]
        public string RecReceiveTypeLastName { get; set; }

        [Column("takeawayTypeInformationType")]
        public string TakeawayTypeInformationType { get; set; }
        [Column("contactId")]
        public Contact Contact { get; set; }
        [Column("addressId")]
        public Address Address { get; set; }
        [Column("patientId")]
        public Patient Patient { get; set; }
        [Column("languageTypeTemplate")]
        public string LanguageTypeTemplate { get; set; }
     
        [Column("descriptionCognitivelyImpaired")]
        public string DescriptionCognitivelyImpaired { get; set; }
     
        [Column("appointmentId")]
        public Appointment Appointment { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Column("takeawayReceiveType")]
        public string TakeawayReceiveType { get; set; }
    
        [Column("additionalNotes")]
        public string AdditionalNotes { get; set; }
        [Column("cmrPatientId")]
        public CmrPatient CmrPatient { get; set; }
        [NotMapped]
        public List<VaccineReconciliation> VaccineReconciliations { get; set; }
        [NotMapped]
        public Boolean IsMedRecTakeawayBlank { get; set; }
        [NotMapped]
        public Boolean IsMedRecReviewBlank { get; set; }
    }

    internal class ServiceTakeAwayMedReconciliationConfiguration : IEntityTypeConfiguration<ServiceTakeAwayMedReconciliation>
    {
        public void Configure(EntityTypeBuilder<ServiceTakeAwayMedReconciliation> builder)
        {
            builder.HasOne(s => s.Patient)
            .WithMany()
            .HasForeignKey("patientId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
