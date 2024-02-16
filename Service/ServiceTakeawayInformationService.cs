using Microsoft.IdentityModel.Tokens;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ServiceTakeawayInformationService : IServiceTakeawayInformationService
    {
        private readonly IPatientData _patientData;
        private readonly IServiceTakeawayInformationData _serviceTakeawayInformationData;
        private readonly IAppointmentService _appointmentService;
        private readonly IPdfStorageService _pdfStorageService;
        private readonly ICmrMedicationData _cmrMedicationData;
        private readonly IAllergyData _allergyData;
        private readonly ISideEffectData _sideEffectData;
        private readonly ISideEffectService _sideEffectService;
        private readonly IAllergyService _allergyService;
        private readonly IMedicationToDoListData _medicationToDoListData;
        private readonly IEmailService _emailService;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly IDoctorMedicationData _doctorMedicationData;

        private static Random random = new Random();



        public ServiceTakeawayInformationService(IPatientData patientData, IDoctorMedicationData doctorMedicationData, IMedicationToDoListData medicationToDoListData, IAllergyService allergyService, ISideEffectService sideEffectService, ISideEffectData sideEffectData, IAllergyData allergyData, ICmrMedicationData cmrMedicationData, IServiceTakeawayInformationData serviceTakeawayInformationData, IAppointmentService appointmentService, IPdfStorageService pdfStorageService, IEmailService emailService, IOtcMedicationData otcMedicationData)
        {
            _patientData = patientData;
            _serviceTakeawayInformationData = serviceTakeawayInformationData;
            _appointmentService = appointmentService;
            _pdfStorageService = pdfStorageService;
            _cmrMedicationData = cmrMedicationData;
            _allergyService = allergyService;
            _sideEffectData = sideEffectData;
            _allergyData = allergyData;
            _sideEffectService = sideEffectService;
            _medicationToDoListData = medicationToDoListData;
            _emailService = emailService;
            _otcMedicationData = otcMedicationData;
            _doctorMedicationData= doctorMedicationData;
        }

        public async Task<Response<ServiceTakeawayInformation>> AddServiceTakeawayInformation(ServiceTakeawayInformationModel model)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            var serviceTakewayInformationDb = await _serviceTakeawayInformationData.GetServiceTakeawayInformationByPatientId(model.PatientId);
            Contact contact = new Contact();
            Address address = new Address();

            if (serviceTakewayInformationDb != null)
            {
                response.Message = "serviceTakewayInformation is already present for that patient";
                response.Success = false;
                return response;
            }
            var cmrPatient = await _serviceTakeawayInformationData.GetCmrPatientById(model.PatientId);
            var patient = await _patientData.GetPatientById(model.PatientId);
            ServiceTakeawayInformation serviceTakeawayInformation = new ServiceTakeawayInformation();
            if (cmrPatient == null)
            {
                Contact contact1 = new Contact();
                Address address1 = new Address();

                address1.State = patient.Address.State;
                address1.AddressLineOne = patient.Address.AddressLineOne;
                address1.AddressLineTwo = patient.Address.AddressLineTwo;
                address1.City = patient.Address.City;
                address1.ZipCode = patient.Address.ZipCode;
                contact1.PrimaryPhone = patient.Contact.PrimaryPhone;
                contact1.SecondaryPhone = patient.Contact.SecondaryPhone;
                contact1.PrimaryEmail = patient.Contact.PrimaryEmail;
                contact1.SecondaryEmail = patient.Contact.SecondaryEmail;
                contact1.FirstName = patient.Contact.FirstName;
                contact1.LastName = patient.Contact.LastName;
                cmrPatient = new CmrPatient
                {
                    Address = address1,
                    Contact = contact1,
                    PatientId = patient.Id,
                    IsCmrType = true,   
                   
                };
                await _serviceTakeawayInformationData.AddCmrPatient(cmrPatient);
            }

            serviceTakeawayInformation.Contact = contact;
            serviceTakeawayInformation.Address = address;
            serviceTakeawayInformation.CmrPatient = cmrPatient;
            serviceTakeawayInformation.Patient = patient;
            var result = await _serviceTakeawayInformationData.AddServiceTakeawayInformation(serviceTakeawayInformation);
            response.Success = true;
            response.Data = result;
            response.Message = "ServiceTakeawayInformation created successfully!";
            return response;
        }


        public async Task<Response<ServiceTakeawayInformation>> GetServiceTakeawayInformationById(int serviceTakeawayInformationId)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            var serviceTakeawayInformation = await _serviceTakeawayInformationData.GetServiceTakeawayInformationById(serviceTakeawayInformationId);


            if (serviceTakeawayInformation == null)
            {
                response.Message = "serviceTakeawayInformation Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "ServiceTakeawayInformation retrived successfully";
            response.Data = serviceTakeawayInformation;
            return response;
        }

        public async Task<Response<ServiceTakeawayInformation>> EditServiceTakeawayInformation(ServiceTakeawayInformationForUpdateModel model)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            response.Success = false;

            var serviceTakeawayInformation = await _serviceTakeawayInformationData.GetServiceTakeawayInformationById(model.Id);
            if (serviceTakeawayInformation.Address == null)
            {
                Address addr = new Address();
                await _serviceTakeawayInformationData.AddAddress(addr);
                serviceTakeawayInformation.Address = addr;
            }
            if (serviceTakeawayInformation.Contact == null)
            {
                Contact cont = new Contact();
                await _serviceTakeawayInformationData.AddContact(cont);
                serviceTakeawayInformation.Contact = cont;
            }
            if (model.TakeawayTypeInformationType == "someone else")
            {
                if (model.Address != null)
                {

                    var address = await _serviceTakeawayInformationData.GetAddressById(serviceTakeawayInformation.Address.Id);
                    address.AddressLineOne = model.Address.AddressLineOne;
                    address.AddressLineTwo = model.Address.AddressLineTwo;
                    address.State = model.Address.State;
                    address.City = model.Address.City;
                    address.ZipCode = model.Address.ZipCode;
                    serviceTakeawayInformation.Address = address;

                }
                if (model.Contact != null)
                {

                    var contact = await _serviceTakeawayInformationData.GetContactById(serviceTakeawayInformation.Contact.Id);
                    contact.FirstName = model.Contact.FirstName;
                    contact.LastName = model.Contact.LastName;
                    contact.PrimaryEmail = model.Contact.PrimaryEmail;
                    contact.PrimaryPhone = model.Contact.PrimaryPhone;
                    contact.DoB = model.Contact.DoB;
                    contact.Fax = model.Contact.Fax;
                    serviceTakeawayInformation.Contact = contact;

                }
            }


            if (model.TakeawayTypeInformationType == "Patient")
            {

                var address = await _serviceTakeawayInformationData.GetAddressById(serviceTakeawayInformation.CmrPatient.Address.Id);
                address.AddressLineOne = model.PatientAddress.AddressLineOne;
                address.AddressLineTwo = model.PatientAddress.AddressLineTwo;
                address.State = model.PatientAddress.State;
                address.City = model.PatientAddress.City;
                address.ZipCode = model.PatientAddress.ZipCode;
                serviceTakeawayInformation.CmrPatient.Address = address;

                var contact = await _serviceTakeawayInformationData.GetContactById(serviceTakeawayInformation.CmrPatient.Contact.Id);
                contact.FirstName = model.PatientContact.FirstName;
                contact.LastName = model.PatientContact.LastName;
                contact.PrimaryEmail = model.PatientContact.PrimaryEmail;
                serviceTakeawayInformation.CmrPatient.Contact = contact;

            }


            if (model.IsFollowUpAppointment == true && model.AppointmentModel != null)
            {
                if (serviceTakeawayInformation.Appointment == null)
                {
                    var resp = await _appointmentService.AddAppointment(model.AppointmentModel);
                    if (resp.Success == false)
                    {
                        response.Success = false;
                        response.Message = resp.Message;
                        return response;
                    }
                    else
                    {
                        serviceTakeawayInformation.Appointment = resp.Data;
                    }

                }
                else
                {
                    var response1 = await _appointmentService.UpdateAppointment(model.AppointmentModel);
                    if (response1.Success == false)
                    {
                        response.Success = false;
                        response.Message = response1.Message;
                        return response;
                    }
                    else
                    {
                        serviceTakeawayInformation.Appointment = response1.Data;
                    }
                }
            }
            if (model.IsPatientCongnitivelyImpaired == true)
            {
                serviceTakeawayInformation.DescriptionCognitivelyImpaired = model.DescriptionCognitivelyImpaired;
            }
            serviceTakeawayInformation.CmrCompleted = model.CmrCompleted;
            serviceTakeawayInformation.IsVaccination = model.IsVaccination;
            serviceTakeawayInformation.IsDiscussExerciseDiet = model.IsDiscussExerciseDiet;
            serviceTakeawayInformation.PatientTakeawayDeliveryDate = model.PatientTakeawayDeliveryDate;
            serviceTakeawayInformation.TakeawayTypeInformationType = model.TakeawayTypeInformationType;
            serviceTakeawayInformation.LanguageTypeTemplate = model.LanguageTypeTemplate;
            serviceTakeawayInformation.TakeawayReceiveType = model.TakeawayReceiveType;
            serviceTakeawayInformation.CmrSendType = model.CmrSendType;
            serviceTakeawayInformation.CmrReceiveType = model.CmrReceiveType;
            serviceTakeawayInformation.CmrReceiveTypeFirstName = model.CmrReceiveTypeFirstName;
            serviceTakeawayInformation.CmrReceiveTypeLastName = model.CmrReceiveTypeLastName;

            if (model.IsFollowUpAppointment != null)
            {
                serviceTakeawayInformation.IsFollowUpAppointment = model.IsFollowUpAppointment;
            }

            serviceTakeawayInformation.VaccineName = model.VaccineName;

            if (model.IsPatientCongnitivelyImpaired != null)
            {
                serviceTakeawayInformation.IsPatientCongnitivelyImpaired = model.IsPatientCongnitivelyImpaired;
            }

            if (model.IsPatientLongTermFacility != null)
            {
                serviceTakeawayInformation.IsPatientLongTermFacility = model.IsPatientLongTermFacility;
            }

            serviceTakeawayInformation.AdditionalNotes = model.AdditionalNotes;
            serviceTakeawayInformation.Email = model.Email;
            serviceTakeawayInformation.Text = model.Text;

            
            List<CmrVaccine> cmrVaccines = await _serviceTakeawayInformationData.GetCmrVaccineByServiceTakeawayInformationById(serviceTakeawayInformation.Id);
            foreach (CmrVaccine cmrVaccine in cmrVaccines)
            {
                 _serviceTakeawayInformationData.PatientDeleteForCmrVaccine(cmrVaccine);

            }

            foreach (string cmrVaccineName in model.CmrVaccines)

            {
                CmrVaccine cmrVaccine1 = new CmrVaccine()
                {
                    Name = cmrVaccineName,
                    ServiceTakeawayInformation = serviceTakeawayInformation,
                };

                var result1 = await _serviceTakeawayInformationData.AddCmrVaccine(cmrVaccine1);
            }
            var result = await _serviceTakeawayInformationData.UpdateServiceTakeawayInformation(serviceTakeawayInformation);



            response.Success = true;
            response.Message = "ServiceTakeawayInformation Updated successfully";
            response.Data = result;
            return response;
        }

        public async Task<Response<ServiceTakeawayInformation>> DeleteServiceTakawayInformationById(int serviceTakeawayInformationId)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            var serviceTakeawayInformation = await _serviceTakeawayInformationData.GetServiceTakeawayInformationById(serviceTakeawayInformationId);
            if (serviceTakeawayInformation == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }

            var cmrMedications = await _cmrMedicationData.GetUniqueCmrMedicationsByPatientId(serviceTakeawayInformation.Patient.Id);

            if (cmrMedications != null)
            {
               
                foreach (CmrMedication cmrMedic in cmrMedications)
                {
                    var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByCmrId(cmrMedic.Id);
                    if (doctorMedications != null)
                    {
                        foreach (DoctorMedication doctorMedication2 in doctorMedications)
                        {
                            _doctorMedicationData.DeleteDoctorMedicationForCmr(doctorMedication2);
                        }
                    }
                    _cmrMedicationData.DeleteCmrMedicationForServiceTakeAwayInformation(cmrMedic);
                }
            }


            var cmrPatient = await _serviceTakeawayInformationData.GetCmrPatientById(serviceTakeawayInformation.CmrPatient.Id);
            if (cmrPatient != null)
            {
                 _serviceTakeawayInformationData.DeleteServiceTakeawayInformationCmrPatient(cmrPatient);
            }

            var patientId = serviceTakeawayInformation.Patient.Id;
            var allergies = await _allergyData.GetAllergyByPatientId(serviceTakeawayInformation.Patient.Id);
            if (allergies != null)
            {
                foreach (Allergy allergy in allergies)
                {
                    _allergyData.DeleteAllergyByServiceTakeAway(allergy);
                }
            }

            var sideeffects = await _sideEffectData.GetSideEffectByPatientId(serviceTakeawayInformation.Patient.Id);
            if (sideeffects != null)
            {
                foreach (SideEffect sideEffect in sideeffects)
                {
                     _sideEffectData.DeleteSideEffectBySideEffectForServiceTakeAway(sideEffect);
                }
            }

            var medicationtodos = await _medicationToDoListData.GetMedicationByPatientId(serviceTakeawayInformation.Patient.Id);
            if (medicationtodos != null)
            {
                foreach (MedicationToDoRelated medicationToDoRelated in medicationtodos)
                {
                     _medicationToDoListData.DeleteMedicationToDoRelatedForServiceTakeAwayInformaction(medicationToDoRelated);
                }
            }


            var nonmedicationtodos = await _medicationToDoListData.GetNonMedicationByPatientId(serviceTakeawayInformation.Patient.Id);
            if (nonmedicationtodos != null)
            {
                foreach (NonRelatedMedicationToDo nonRelatedMedicationToDo in nonmedicationtodos)
                {
                     _medicationToDoListData.DeleteNonMedicationToDoListRelatedForServiceTakeAwayInformaction(nonRelatedMedicationToDo);
                }
            }


            var cmrVaccines = await _serviceTakeawayInformationData.GetCmrVaccineByServiceTakeawayInformationById(serviceTakeawayInformation.Id);
            if (cmrVaccines != null)
            {
                foreach (CmrVaccine cmrVaccine in cmrVaccines)
                {
                     _serviceTakeawayInformationData.PatientDeleteForCmrVaccine(cmrVaccine);
                }
            }
            var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(patientId);

            foreach (OtcMedication otcMedication in otcMedications)
            {
                otcMedication.IsCmrCreated = false;
                await _otcMedicationData.UpdateOtcMedication(otcMedication);
            }

            await _serviceTakeawayInformationData.DeleteServiceTakeawayInformation(serviceTakeawayInformation);
            response.Message = "serviceTakeawayInformation Deleted Succesfully ";
            response.Success = true;
            return response;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<Response<ServiceTakeawayInformation>> GetServiceTakewayInformationByPatientId(int patientId)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            List<CmrVaccine> cmrVaccinesList = new List<CmrVaccine>();
            var serviceTakeawayInformation = await _serviceTakeawayInformationData.GetServiceTakeawayInformationByPatientId(patientId);
            if (serviceTakeawayInformation == null)
            {
                response.Message = "serviceTakeawayInformation Not Found";
                response.Success = false;
                return response;
            }
            if (serviceTakeawayInformation.CmrPatient == null && serviceTakeawayInformation.Patient != null)
            {
                var patient = await _patientData.GetPatientById(serviceTakeawayInformation.Patient.Id);
                Address address = new Address();
                Contact contact = new Contact();
                address.State = patient.Address.State;
                address.AddressLineOne = patient.Address.AddressLineOne;
                address.AddressLineTwo = patient.Address.AddressLineTwo;
                address.City = patient.Address.City;
                address.ZipCode = patient.Address.ZipCode;
                contact.PrimaryPhone = patient.Contact.PrimaryPhone;
                contact.SecondaryPhone = patient.Contact.SecondaryPhone;
                contact.PrimaryEmail = patient.Contact.PrimaryEmail;
                contact.SecondaryEmail = patient.Contact.SecondaryEmail;
                contact.FirstName = patient.Contact.FirstName;
                contact.LastName = patient.Contact.LastName;
                var cmrPatient = new CmrPatient
                {
                    Address = address,
                    Contact = contact,
                    IsCmrType = true,
                };
                await _serviceTakeawayInformationData.AddCmrPatient(cmrPatient);
                serviceTakeawayInformation.CmrPatient = cmrPatient;
            }
            var cmrVaccines = await _serviceTakeawayInformationData.GetCmrVaccineByServiceTakeawayInformationById(serviceTakeawayInformation.Id);
            cmrVaccinesList = cmrVaccines.GroupBy(m => m.Name).Select(m => m.FirstOrDefault()).ToList();
            var isValidate = true;
            var IsValidateReview = true;
            var isTakeawayBlank = true;
            var isReviewBlank = true;


            serviceTakeawayInformation.CmrVaccines = cmrVaccinesList;
            if (serviceTakeawayInformation.Appointment != null)
            {
                DateTime dtt = DateTime.Now.ToUniversalTime();
                String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", serviceTakeawayInformation.Appointment.StartDateTime);

                serviceTakeawayInformation.Appointment.StrStartDateTime = strStartDateTime;
                int val;
                bool result = int.TryParse(serviceTakeawayInformation.Appointment.Duration, out val);
                serviceTakeawayInformation.Appointment.EndDateTime = serviceTakeawayInformation.Appointment.StartDateTime.AddMinutes(val);
                String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", serviceTakeawayInformation.Appointment.EndDateTime);

                serviceTakeawayInformation.Appointment.StrEndDateTime = strEndDateTime;
            }

            if (serviceTakeawayInformation.TakeawayTypeInformationType == null)
            {
                isValidate = false;
            }
            else
            {
                isTakeawayBlank = false;
            }
            if (serviceTakeawayInformation.IsDiscussExerciseDiet == null || serviceTakeawayInformation.IsVaccination == null || string.IsNullOrEmpty(serviceTakeawayInformation.CmrCompleted) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveType) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrSendType) )
            {
                isValidate = false;
            }
            if ((serviceTakeawayInformation.IsDiscussExerciseDiet == null) && (serviceTakeawayInformation.IsVaccination == null) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrCompleted) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveType) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrSendType))
            {
                isTakeawayBlank = true;
            }
            if((serviceTakeawayInformation.IsDiscussExerciseDiet == false || serviceTakeawayInformation.IsDiscussExerciseDiet == true) || !string.IsNullOrEmpty(serviceTakeawayInformation.CmrCompleted) || (serviceTakeawayInformation.IsVaccination == false || serviceTakeawayInformation.IsVaccination == true))
            {
                isTakeawayBlank = false;

            }
            if (serviceTakeawayInformation.TakeawayTypeInformationType == "Patient")
            {
                if ( string.IsNullOrEmpty(serviceTakeawayInformation.LanguageTypeTemplate) || string.IsNullOrEmpty(serviceTakeawayInformation.PatientTakeawayDeliveryDate) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Contact.LastName) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.State) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.City) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.ZipCode))
                {
                    isValidate = false;
                }
                if (string.IsNullOrEmpty(serviceTakeawayInformation.LanguageTypeTemplate) && string.IsNullOrEmpty(serviceTakeawayInformation.PatientTakeawayDeliveryDate) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Contact.FirstName) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Contact.LastName) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.State) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.City) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.AddressLineOne) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrPatient.Address.ZipCode))
                {
                    isTakeawayBlank = false;
                }
            }


            if (serviceTakeawayInformation.TakeawayTypeInformationType == "someone else")
            {
                if (string.IsNullOrEmpty(serviceTakeawayInformation.LanguageTypeTemplate) || serviceTakeawayInformation.PatientTakeawayDeliveryDate == null
                || string.IsNullOrEmpty(serviceTakeawayInformation.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeawayInformation.Contact.LastName) || string.IsNullOrEmpty(serviceTakeawayInformation.Address.ZipCode) || string.IsNullOrEmpty(serviceTakeawayInformation.Address.State) || string.IsNullOrEmpty(serviceTakeawayInformation.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeawayInformation.Address.City))
                {
                    isValidate = false;
                }
                if (string.IsNullOrEmpty(serviceTakeawayInformation.LanguageTypeTemplate) && serviceTakeawayInformation.PatientTakeawayDeliveryDate == null
                || string.IsNullOrEmpty(serviceTakeawayInformation.Contact.FirstName) && string.IsNullOrEmpty(serviceTakeawayInformation.Contact.LastName) && string.IsNullOrEmpty(serviceTakeawayInformation.Address.ZipCode) && string.IsNullOrEmpty(serviceTakeawayInformation.Address.State) && string.IsNullOrEmpty(serviceTakeawayInformation.Address.AddressLineOne) && string.IsNullOrEmpty(serviceTakeawayInformation.Address.City))
                {
                    isTakeawayBlank = false;
                }
            }

            if (serviceTakeawayInformation.CmrReceiveType == "Healthcare Provider" || serviceTakeawayInformation.CmrReceiveType == "Caregiver")
            {
                if (string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveTypeFirstName) || string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveTypeLastName))
                {
                    isValidate = false;
                }
                if (string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveTypeFirstName) && string.IsNullOrEmpty(serviceTakeawayInformation.CmrReceiveTypeLastName))
                {
                    isTakeawayBlank = false;
                }

            }

            if (string.IsNullOrEmpty(serviceTakeawayInformation.TakeawayReceiveType) || serviceTakeawayInformation.IsPatientCongnitivelyImpaired == null || serviceTakeawayInformation.IsFollowUpAppointment == null || serviceTakeawayInformation.IsPatientLongTermFacility == null)
            {
                IsValidateReview = false;
            }
            if (string.IsNullOrEmpty(serviceTakeawayInformation.TakeawayReceiveType) && 
                (serviceTakeawayInformation.IsPatientCongnitivelyImpaired == null && (serviceTakeawayInformation.IsFollowUpAppointment == null )
                && (serviceTakeawayInformation.IsPatientLongTermFacility== null)))
            {
                isReviewBlank = true;
            }
            if ((serviceTakeawayInformation.IsPatientCongnitivelyImpaired == true ||
                serviceTakeawayInformation.IsPatientCongnitivelyImpaired == false) || (serviceTakeawayInformation.IsFollowUpAppointment == true || serviceTakeawayInformation.IsFollowUpAppointment == false) || (serviceTakeawayInformation.IsPatientLongTermFacility == true || serviceTakeawayInformation.IsPatientLongTermFacility == false))
            {
                isReviewBlank = false;
            }
            serviceTakeawayInformation.IsValidate = isValidate;
            serviceTakeawayInformation.IsValidateReview = IsValidateReview;
            serviceTakeawayInformation.IsTakeawayBlank = isTakeawayBlank;
            serviceTakeawayInformation.IsReviewBlank = isReviewBlank;
            response.Success = true;
            response.Message = "ServiceTakeawayInformation retrived successfully";
            response.Data = serviceTakeawayInformation;
            return response;
        }

        public async Task<Response<ServiceTakeawayInformation>> CheckLinkExpired(int patientId, string uuid)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            var takeawayVerify = await _serviceTakeawayInformationData.GetTakeawayVerifyByPatientIdAndUUID(patientId, uuid);
            response.Success = false;
            response.Message = "Link Expired";
            if (takeawayVerify == null)
            {
                return response;
            }
            var hours = DateTime.Now.Subtract(takeawayVerify.CreatedAt).TotalHours;
            if (hours >= 72)
            {
                await _serviceTakeawayInformationData.DeleteTakeawayVerify(takeawayVerify);
                return response;
            }
            response.Message = "Valid Link";
            response.Success = true;
            return response;
        }

        public async Task<Response<byte[]>> PatientVerificationForPdf(string lastname, DateTime dob, int patientId, string uuid)
        {
            Response<byte[]> response = new Response<byte[]>();
            var takeawayVerify = await _serviceTakeawayInformationData.GetTakeawayVerifyByPatientIdAndUUID(patientId, uuid);
            if (takeawayVerify == null)
            {
                response.Message = "Invalid Details";
                response.Success = false;
                return response;
            }
            var hours = DateTime.Now.Subtract(takeawayVerify.CreatedAt).TotalHours;
            if (hours >= 72)
            {
                await _serviceTakeawayInformationData.DeleteTakeawayVerify(takeawayVerify);
                response.Message = "Invalid Details";
                response.Success = false;
                return response;
            }
            var patient = await _serviceTakeawayInformationData.PatientVerificationForPdf(lastname, dob, patientId);

            if (patient != null)
            {
                response.Success = true;
                response.Message = "Verification successfully";
                takeawayVerify.LastModified = DateTime.Now;
                await _serviceTakeawayInformationData.UpdateTakeawayVerify(takeawayVerify);
                response.Data = await _pdfStorageService.GetPDFURIForCmr(patientId);
                return response;

            }
            response.Message = "Please enter the correct Last Name and Date of Birth values.";
            response.Success = false;
            return response;

        }

        public async Task<Response<ServiceTakeawayInformation>> SendCmrPdfEmail(string email, int patientId)
        {
            Response<ServiceTakeawayInformation> response = new Response<ServiceTakeawayInformation>();
            var takeawayDb = await _serviceTakeawayInformationData.GetTakeawayVerifyByPatientId(patientId);
            if (takeawayDb != null)
            {
                var hours = DateTime.Now.Subtract(takeawayDb.CreatedAt).TotalHours;
                if (hours >= 72)
                {
                    takeawayDb.CreatedAt = DateTime.Now;
                    await _serviceTakeawayInformationData.UpdateTakeawayVerify(takeawayDb);
                }
                await _emailService.SendCmrPdfEmail(takeawayDb, email, patientId);
            }
            else
            {
                TakeawayVerify takeawayVerify = new TakeawayVerify
                {
                    UUID = RandomString(8),
                    PatientId = patientId,
                    CreatedAt = DateTime.Now,
                    IsServiceTakeAwayInfo = true,
                };
                await _serviceTakeawayInformationData.AddTakeawayVerify(takeawayVerify);
                await _emailService.SendCmrPdfEmail(takeawayVerify, email, patientId);
            }
            response.Success = true;
            response.Message = "Email send successfully";
            return response;
        }

        public async Task<Response<TakeawayVerify>> GetTakeawayVerifyByPatientId(int patientId)
        {
            Response<TakeawayVerify> response = new Response<TakeawayVerify>();
            var takeawayDb = await _serviceTakeawayInformationData.GetTakeawayVerifyByPatientId(patientId);
            if (takeawayDb != null)
            {
                var hours = DateTime.Now.Subtract(takeawayDb.CreatedAt).TotalHours;
                if (hours >= 72)
                {
                    takeawayDb.CreatedAt = DateTime.Now;
                    await _serviceTakeawayInformationData.UpdateTakeawayVerify(takeawayDb);
                }
                response.Data = takeawayDb;
                return response;
            }
            TakeawayVerify takeawayVerify = new TakeawayVerify
            {
                UUID = RandomString(8),
                PatientId = patientId,
                CreatedAt = DateTime.Now,
                IsServiceTakeAwayInfo = true,
            };
            await _serviceTakeawayInformationData.AddTakeawayVerify(takeawayVerify);
            response.Data = takeawayVerify;
            return response;


        }
    }
}
