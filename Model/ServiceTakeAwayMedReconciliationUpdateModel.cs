using OntrackDb.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace OntrackDb.Model
{ 
    public class ServiceTakeAwayMedReconciliationUpdateModel
    {
        public int Id { get; set; }
        #nullable enable
        public String? RecCompleted { get; set; }
        public Boolean? IsVaccination { get; set; }
        public Boolean? IsDiscussExerciseDiet { get; set; }
        public String? PatientTakeawayDeliveryDate { get; set; }

        public Boolean? IsPatientCongnitivelyImpaired { get; set; }
        public Boolean? IsFollowUpAppointment { get; set; }
        public Boolean? IsPatientLongTermFacility { get; set; }
        #nullable disable
        public string VaccineName { get; set; }
        public string RecSendType { get; set; }
        public string RecReceiveType { get; set; }
        public string RecReceiveTypeFirstName { get; set; }
        public string RecReceiveTypeLastName { get; set; }
        public string TakeawayTypeInformationType { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public Patient Patient { get; set; }
        public Address PatientAddress { get; set; }
        public Contact PatientContact { get; set; }
        public AppointmentModel AppointmentModel { get; set; }
        public string LanguageTypeTemplate { get; set; }
        public string DescriptionCognitivelyImpaired { get; set; }
        public Appointment Appointment { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public string TakeawayReceiveType { get; set; }
        public string AdditionalNotes { get; set; }
        public CmrPatient CmrPatient { get; set; }
        public List<string> VaccineReconciliations { get; set; }
        public Boolean IsValidate { get; set; }
        public Boolean IsValidateReview { get; set; }
    }
}
