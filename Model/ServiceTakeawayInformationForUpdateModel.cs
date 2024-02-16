using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Model
{
    public class ServiceTakeawayInformationForUpdateModel
    {

        public int Id { get; set; }
        #nullable enable
        public String? CmrCompleted { get; set; }
        public Boolean? IsVaccination { get; set; }
        public Boolean? IsDiscussExerciseDiet { get; set; }
        public String? PatientTakeawayDeliveryDate { get; set; }
        public Boolean? IsPatientCongnitivelyImpaired { get; set; }
        public Boolean? IsFollowUpAppointment { get; set; }
        public Boolean? IsPatientLongTermFacility { get; set; }
        #nullable disable
        public string VaccineName { get; set; }
       
        public string CmrSendType { get; set; }
       
        public string CmrReceiveType { get; set; }
        public string CmrReceiveTypeFirstName { get; set; }
        public string CmrReceiveTypeLastName { get; set; }
        public string TakeawayTypeInformationType { get; set; }
        public string DescriptionCognitivelyImpaired { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }

        public Contact Contact { get; set; }
        
        public Address Address { get; set; }
        
        public Patient Patient { get; set; }
        public Address PatientAddress { get; set; }
        public Contact PatientContact { get; set; }
        public AppointmentModel AppointmentModel { get; set; }
        public string LanguageTypeTemplate { get; set; }
        public string TakeawayReceiveType { get; set; }
        public string AdditionalNotes { get; set; }

        public List<string> CmrVaccines { get; set; }
    }
}
