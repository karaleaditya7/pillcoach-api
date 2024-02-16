using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("serviceTakeawayInformation")]
    public class ServiceTakeawayInformation
    {
        [Key]
        public int Id { get; set; }
         #nullable enable
        [Column("cmrCompleted")]
        public String? CmrCompleted { get; set; }
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
        [Column("takeawayReceiveType")]
        public string? TakeawayReceiveType { get; set; }
        [Column("isPatientLongTermFacility")]
        public Boolean? IsPatientLongTermFacility { get; set; }
        [NotMapped]
        public Boolean? IsValidate { get; set; }
        [NotMapped]
        public Boolean? IsValidateReview { get; set; }
        #nullable disable
        [Column("vaccineName")]
        public string VaccineName { get; set; }
        [Column("cmrSendType")]
        public string CmrSendType { get; set; }
        [Column("cmrReceiveType")]
        public string CmrReceiveType { get; set; }

        [Column("cmrReceiveTypeFirstName")]
        public string CmrReceiveTypeFirstName { get; set; }

        [Column("cmrReceiveTypeLastName")]
        public string CmrReceiveTypeLastName { get; set; }

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
        [Column("additionalNotes")]
        public string AdditionalNotes { get; set; }
        [Column("cmrPatientId")]
        public CmrPatient CmrPatient { get; set; }
        [NotMapped]
        public List<CmrVaccine> CmrVaccines { get; set; }
        [NotMapped]
        public Boolean IsTakeawayBlank { get; set; }

        [NotMapped]
        public Boolean IsReviewBlank { get; set; }
    }

    internal class ServiceTakeawayInformationConfiguration : IEntityTypeConfiguration<ServiceTakeawayInformation>
    {
        public void Configure(EntityTypeBuilder<ServiceTakeawayInformation> builder)
        {
            builder.HasOne(s => s.Patient)
            .WithMany().HasForeignKey("patientId")
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
