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
    public class ServiceTakeAwayMedReconciliationService : IServiceTakeAwayMedReconciliationService
    {

        private readonly IPatientData _patientData;
        private readonly IServiceTakeAwayMedReconciliationData  _serviceTakeAwayMedReconciliationData;
        private readonly IAppointmentService _appointmentService;
        private readonly IPdfStorageService _pdfStorageService;
        private readonly ICmrMedicationData _cmrMedicationData;
        private readonly IAllergyData _allergyData;
        private readonly ISideEffectData _sideEffectData;
        private readonly ISideEffectService _sideEffectService;
        private readonly IAllergyService _allergyService;
        private readonly IMedicationToDoListData _medicationToDoListData;
        private readonly IEmailService _emailService;
        private readonly IReconciliationData _reconciliationData;
        private readonly IReconciliationAllergyData _reconciliationAllergyData;
        private readonly IReconciliationSideEffectData _reconciliationSideEffectData;
        private readonly IReconciliationToDoListData _reconciliationToDoListData;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly IDoctorMedicationData _doctorMedicationData;

        private static Random random = new Random();

        public ServiceTakeAwayMedReconciliationService(IPatientData patientData, IDoctorMedicationData doctorMedicationData , IReconciliationToDoListData reconciliationToDoListData , IReconciliationSideEffectData reconciliationSideEffectData ,IReconciliationAllergyData reconciliationAllergyData , IReconciliationData reconciliationData, IMedicationToDoListData medicationToDoListData, IAllergyService allergyService, ISideEffectService sideEffectService, ISideEffectData sideEffectData, IAllergyData allergyData, ICmrMedicationData cmrMedicationData, IServiceTakeAwayMedReconciliationData IServiceTakeAwayMedReconciliationData, IAppointmentService appointmentService, IPdfStorageService pdfStorageService, IEmailService emailService,IOtcMedicationData otcMedicationData)
        {
            _patientData = patientData;
            _serviceTakeAwayMedReconciliationData = IServiceTakeAwayMedReconciliationData;
            _appointmentService = appointmentService;
            _pdfStorageService = pdfStorageService;
            _cmrMedicationData = cmrMedicationData;
            _allergyService = allergyService;
            _sideEffectData = sideEffectData;
            _allergyData = allergyData;
            _sideEffectService = sideEffectService;
            _medicationToDoListData = medicationToDoListData;
            _emailService = emailService;
            _reconciliationData = reconciliationData;
            _reconciliationAllergyData = reconciliationAllergyData;
            _reconciliationSideEffectData = reconciliationSideEffectData;
            _reconciliationToDoListData = reconciliationToDoListData;
            _otcMedicationData = otcMedicationData;
            _doctorMedicationData = doctorMedicationData;

        }


        public async Task<Response<ServiceTakeAwayMedReconciliation>> AddServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliationModel model)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            var serviceTakeawaymedreconciliationdb = await _serviceTakeAwayMedReconciliationData.GetServiceTakeAwayMedReconciliationByPatientId(model.PatientId);
            Contact contact = new Contact();
            Address address = new Address();

            if (serviceTakeawaymedreconciliationdb != null)
            {
                response.Message = "ServiceTakeAwayMedReconciliation is already present for that patient";
                response.Success = false;
                return response;
            }
            var cmrPatient = await _serviceTakeAwayMedReconciliationData.GetCmrPatientById(model.PatientId);
            var patient = await _patientData.GetPatientById(model.PatientId);
            ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation = new ServiceTakeAwayMedReconciliation();
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
                    IsMedRecType = true,
                };
                await _serviceTakeAwayMedReconciliationData.AddCmrPatient(cmrPatient);
            }

            serviceTakeAwayMedReconciliation.Contact = contact;
            serviceTakeAwayMedReconciliation.Address = address;
            serviceTakeAwayMedReconciliation.CmrPatient = cmrPatient;
            serviceTakeAwayMedReconciliation.Patient = patient;
            var result = await _serviceTakeAwayMedReconciliationData.AddServiceTakeawayReconciliation(serviceTakeAwayMedReconciliation);
            response.Success = true;
            response.Data = result;
            response.Message = "ServiceTakeAwayMedReconciliation created successfully!";
            return response;
        }

        public async Task<Response<ServiceTakeAwayMedReconciliation>> GetServiceTakeawayMedReconciliationById(int serviceTakeawaymedreconciliationId)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            var servicetakeawaymedreconciliation = await _serviceTakeAwayMedReconciliationData.GetServiceTakeawayReconciliationById(serviceTakeawaymedreconciliationId);


            if (servicetakeawaymedreconciliation == null)
            {
                response.Message = "servicetakeawaymedreconciliation Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "ServiceTakeawayInformation retrived successfully";
            response.Data = servicetakeawaymedreconciliation;
            return response;
        }


        public async Task<Response<ServiceTakeAwayMedReconciliation>> EditServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliationUpdateModel model)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            response.Success = false;

            var serviceTakeAwayMedReconciliation = await _serviceTakeAwayMedReconciliationData.GetServiceTakeawayReconciliationById(model.Id);
            if (serviceTakeAwayMedReconciliation.Address == null)
            {
                Address addr = new Address();
                await _serviceTakeAwayMedReconciliationData.AddAddress(addr);
                serviceTakeAwayMedReconciliation.Address = addr;
            }
            if (serviceTakeAwayMedReconciliation.Contact == null)
            {
                Contact cont = new Contact();
                await _serviceTakeAwayMedReconciliationData.AddContact(cont);
                serviceTakeAwayMedReconciliation.Contact = cont;
            }
            if (model.TakeawayTypeInformationType == "someone else")
            {
                if (model.Address != null)
                {

                    var address = await _serviceTakeAwayMedReconciliationData.GetAddressById(serviceTakeAwayMedReconciliation.Address.Id);
                    address.AddressLineOne = model.Address.AddressLineOne;
                    address.AddressLineTwo = model.Address.AddressLineTwo;
                    address.State = model.Address.State;
                    address.City = model.Address.City;
                    address.ZipCode = model.Address.ZipCode;
                    serviceTakeAwayMedReconciliation.Address = address;

                }
                if (model.Contact != null)
                {

                    var contact = await _serviceTakeAwayMedReconciliationData.GetContactById(serviceTakeAwayMedReconciliation.Contact.Id);
                    contact.FirstName = model.Contact.FirstName;
                    contact.LastName = model.Contact.LastName;
                    contact.PrimaryEmail = model.Contact.PrimaryEmail;
                    contact.PrimaryPhone = model.Contact.PrimaryPhone;
                    contact.DoB = model.Contact.DoB;
                    contact.Fax = model.Contact.Fax;
                    serviceTakeAwayMedReconciliation.Contact = contact;

                }
            }


            if (model.TakeawayTypeInformationType == "Patient")
            {

                var address = await _serviceTakeAwayMedReconciliationData.GetAddressById(serviceTakeAwayMedReconciliation.CmrPatient.Address.Id);
                address.AddressLineOne = model.PatientAddress.AddressLineOne;
                address.AddressLineTwo = model.PatientAddress.AddressLineTwo;
                address.State = model.PatientAddress.State;
                address.City = model.PatientAddress.City;
                address.ZipCode = model.PatientAddress.ZipCode;
                serviceTakeAwayMedReconciliation.CmrPatient.Address = address;

                var contact = await _serviceTakeAwayMedReconciliationData.GetContactById(serviceTakeAwayMedReconciliation.CmrPatient.Contact.Id);
                contact.FirstName = model.PatientContact.FirstName;
                contact.LastName = model.PatientContact.LastName;
                contact.PrimaryEmail = model.PatientContact.PrimaryEmail;
                serviceTakeAwayMedReconciliation.CmrPatient.Contact = contact;

            }


            if (model.IsFollowUpAppointment == true && model.AppointmentModel != null)
            {
                if (serviceTakeAwayMedReconciliation.Appointment == null)
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
                        serviceTakeAwayMedReconciliation.Appointment = resp.Data;
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
                        serviceTakeAwayMedReconciliation.Appointment = response1.Data;
                    }


                }
            }
            if (model.IsPatientCongnitivelyImpaired == true)
            {
                serviceTakeAwayMedReconciliation.DescriptionCognitivelyImpaired = model.DescriptionCognitivelyImpaired;
            }
            serviceTakeAwayMedReconciliation.RecCompleted = model.RecCompleted;
            serviceTakeAwayMedReconciliation.IsVaccination = model.IsVaccination;
            serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet = model.IsDiscussExerciseDiet;
            serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate = model.PatientTakeawayDeliveryDate;
            serviceTakeAwayMedReconciliation.TakeawayTypeInformationType = model.TakeawayTypeInformationType;
            serviceTakeAwayMedReconciliation.LanguageTypeTemplate = model.LanguageTypeTemplate;
            serviceTakeAwayMedReconciliation.TakeawayReceiveType = model.TakeawayReceiveType;
            serviceTakeAwayMedReconciliation.RecSendType = model.RecSendType;
            serviceTakeAwayMedReconciliation.RecReceiveType = model.RecReceiveType;
            serviceTakeAwayMedReconciliation.RecReceiveTypeFirstName = model.RecReceiveTypeFirstName;
            serviceTakeAwayMedReconciliation.RecReceiveTypeLastName = model.RecReceiveTypeLastName;

            if (model.IsFollowUpAppointment != null)
            {
                serviceTakeAwayMedReconciliation.IsFollowUpAppointment = model.IsFollowUpAppointment;
            }

            serviceTakeAwayMedReconciliation.VaccineName = model.VaccineName;

            if (model.IsPatientCongnitivelyImpaired != null)
            {
                serviceTakeAwayMedReconciliation.IsPatientCongnitivelyImpaired = model.IsPatientCongnitivelyImpaired;
            }

            if (model.IsPatientLongTermFacility != null)
            {
                serviceTakeAwayMedReconciliation.IsPatientLongTermFacility = model.IsPatientLongTermFacility;
            }

            serviceTakeAwayMedReconciliation.AdditionalNotes = model.AdditionalNotes;
            serviceTakeAwayMedReconciliation.Email = model.Email;
            serviceTakeAwayMedReconciliation.Text = model.Text;

            //  var cmrVaccineList = await _serviceTakeawayInformationData.GetCmrVaccineByServiceTakeawayInformationById(serviceTakeawayInformation.Id);

            List<VaccineReconciliation> vaccineReconciliations = await _serviceTakeAwayMedReconciliationData.GetVaccineRecByServiceTakeawayRecById(serviceTakeAwayMedReconciliation.Id);
            foreach (VaccineReconciliation vaccineReconciliation in vaccineReconciliations)
            {
                 _serviceTakeAwayMedReconciliationData.PatientDeleteForVaccineRec(vaccineReconciliation);

            }

            foreach (string cmrVaccineName in model.VaccineReconciliations)

            {
                VaccineReconciliation cmrVaccineReconciliation = new VaccineReconciliation()
                {
                    Name = cmrVaccineName,
                    ServiceTakeAwayMedReconciliation = serviceTakeAwayMedReconciliation,
                };

                var result1 = await _serviceTakeAwayMedReconciliationData.AddCmrVaccine(cmrVaccineReconciliation);
            }
            var result = await _serviceTakeAwayMedReconciliationData.UpdateServiceTakeawayMedReconciliation(serviceTakeAwayMedReconciliation);



            response.Success = true;
            response.Message = "ServiceTakeAwayMedReconciliation Updated successfully";
            response.Data = result;
            return response;
        }


        public async Task<Response<ServiceTakeAwayMedReconciliation>> DeleteServiceTakeAwayMedReconciliationById(int serviceTakeawaymedreconciliationId)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            var serviceTakeAwayMedReconciliation = await _serviceTakeAwayMedReconciliationData.GetServiceTakeawayMedReconciliationById(serviceTakeawaymedreconciliationId);
            if (serviceTakeAwayMedReconciliation == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }

            var medicationReconciliations = await _reconciliationData.GetUniqueReconciliationMedicationsByPatientId(serviceTakeAwayMedReconciliation.Patient.Id);

            if (medicationReconciliations != null)
            {
                foreach (MedicationReconciliation medicationReconciliation1 in medicationReconciliations)
                {
                    var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByMedRecId(medicationReconciliation1.Id);
                    if (doctorMedications != null)
                    {
                        foreach(DoctorMedication doctorMedication2 in doctorMedications)
                        {
                            _doctorMedicationData.DeleteDoctorMedicationForMedRec(doctorMedication2);
                        }
                    }
                     _reconciliationData.DeleteReconciliationMedicationForServiceTakeAwayMedReconciliation(medicationReconciliation1);

                }
            }
 
            var cmrPatient = await _serviceTakeAwayMedReconciliationData.GetCmrPatientById(serviceTakeAwayMedReconciliation.CmrPatient.Id);
            if (cmrPatient != null)
            {
                 _serviceTakeAwayMedReconciliationData.DeleteServiceTakeawayMedReconciliationCmrPatient(cmrPatient);
            }

            var patientId = serviceTakeAwayMedReconciliation.Patient.Id;
            var recAllergies = await _reconciliationAllergyData.GetReconciliationAllergyByPatientId(serviceTakeAwayMedReconciliation.Patient.Id);
            if (recAllergies != null)
            {
                foreach (ReconciliationAllergy reconciliationAllergy in recAllergies)
                {
                     _reconciliationAllergyData.DeleteRecAllergyByServiceTakeAwayMedReconciliation(reconciliationAllergy);
                }
            }

            var recSideeffects = await _reconciliationSideEffectData.GetMedRecSideEffectByPatientId(serviceTakeAwayMedReconciliation.Patient.Id);
            if (recSideeffects != null)
            {
                foreach (ReconciliationSideeffect reconciliationSideeffect in recSideeffects)
                {
                     _reconciliationSideEffectData.DeleteMedRecSideEffectByServiceTakeMedRecId(reconciliationSideeffect);
                }
            }


            var reconciliationMedicationToDos = await _reconciliationToDoListData.GetReconciliationToDoByPatientId(serviceTakeAwayMedReconciliation.Patient.Id);
            if (reconciliationMedicationToDos != null)
            {
                foreach (ReconciliationToDoRelated reconciliationToDoRelated in reconciliationMedicationToDos)
                {
                     _reconciliationToDoListData.DeleteReconciliationToDoRelatedForServiceTakeAwayMedRec(reconciliationToDoRelated);
                }
            }


            var nonreconciliationMedicationToDos = await _reconciliationToDoListData.GetNonReconciliationToDoByPatientId(serviceTakeAwayMedReconciliation.Patient.Id);
            if (nonreconciliationMedicationToDos != null)
            {
                foreach (NonRelatedRecocilationToDo nonRelatedRecocilationToDo in nonreconciliationMedicationToDos)
                {
                     _reconciliationToDoListData.DeleteNonRecocilationToDoForServiceTakeAwayMedRec(nonRelatedRecocilationToDo);
                }
            }


            var vaccineReconciliations = await _serviceTakeAwayMedReconciliationData.GetVaccineRecByServiceTakeawayRecById(serviceTakeAwayMedReconciliation.Id);
            if (vaccineReconciliations != null)
            {
                foreach (VaccineReconciliation vaccineReconciliation in vaccineReconciliations)
                {
                     _serviceTakeAwayMedReconciliationData.PatientDeleteForVaccineRec(vaccineReconciliation);
                }
            }
            var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(patientId);

            foreach (OtcMedication otcMedication in otcMedications)
            {
                otcMedication.IsRecCreated = false;
                await _otcMedicationData.UpdateOtcMedication(otcMedication);
            }

            await _serviceTakeAwayMedReconciliationData.DeleteServiceTakeawayMedReconciliation(serviceTakeAwayMedReconciliation);

            response.Message = "serviceTakeawayMedReconciliation Deleted Succesfully ";
            response.Success = true;
            return response;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<Response<ServiceTakeAwayMedReconciliation>> GetServiceTakewayMedReconciliationByPatientId(int patientId)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            List<VaccineReconciliation> vaccineReconciliations1 = new List<VaccineReconciliation>();
            var serviceTakeAwayMedReconciliation = await _serviceTakeAwayMedReconciliationData.GetServiceTakeAwayMedReconciliationByPatientId(patientId);
            if (serviceTakeAwayMedReconciliation == null)
            {
                response.Message = "serviceTakeAwayMedReconciliation Not Found";
                response.Success = false;
                return response;
            }
            if (serviceTakeAwayMedReconciliation.CmrPatient == null && serviceTakeAwayMedReconciliation.Patient != null)
            {
                var patient = await _patientData.GetPatientById(serviceTakeAwayMedReconciliation.Patient.Id);
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
                    IsMedRecType = true,
                };
                await _serviceTakeAwayMedReconciliationData.AddCmrPatient(cmrPatient);
                serviceTakeAwayMedReconciliation.CmrPatient = cmrPatient;
            }
            var vaccineReconciliations = await _serviceTakeAwayMedReconciliationData.GetVaccineRecByServiceTakeawayRecById(serviceTakeAwayMedReconciliation.Id);
            vaccineReconciliations1 = vaccineReconciliations.GroupBy(m => m.Name).Select(m => m.FirstOrDefault()).ToList();
            var isValidate = true;
            var IsValidateReview = true;
            var isMedRecTakeawayBlank = true;
            var isMedRecReviewBlank = true;

            serviceTakeAwayMedReconciliation.VaccineReconciliations = vaccineReconciliations1;
            if (serviceTakeAwayMedReconciliation.Appointment != null)
            {
                DateTime dtt = DateTime.Now.ToUniversalTime();
                String strStartDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", serviceTakeAwayMedReconciliation.Appointment.StartDateTime);

                serviceTakeAwayMedReconciliation.Appointment.StrStartDateTime = strStartDateTime;
                int val;
                bool result = int.TryParse(serviceTakeAwayMedReconciliation.Appointment.Duration, out val);
                serviceTakeAwayMedReconciliation.Appointment.EndDateTime = serviceTakeAwayMedReconciliation.Appointment.StartDateTime.AddMinutes(val);
                String strEndDateTime = String.Format("{0:ddd MMM dd yyyy HH:mm:ss}" + " GMT", serviceTakeAwayMedReconciliation.Appointment.EndDateTime);

                serviceTakeAwayMedReconciliation.Appointment.StrEndDateTime = strEndDateTime;
            }

            if (serviceTakeAwayMedReconciliation.TakeawayTypeInformationType == null)
            {
                isValidate = false;
            }
            else
            {
                isMedRecTakeawayBlank=false;
            }
            if(serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet==null || serviceTakeAwayMedReconciliation.IsVaccination==null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecCompleted) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveType) ||string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecSendType)) 
            {
                isValidate = false;
            }
            if((serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet==null) && (serviceTakeAwayMedReconciliation.IsVaccination==null) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecCompleted) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveType) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecSendType))
            {
                isMedRecTakeawayBlank = true;
            }
            if ((serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet == false || serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet == true) || !string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecCompleted) || (serviceTakeAwayMedReconciliation.IsVaccination == false || serviceTakeAwayMedReconciliation.IsVaccination == true))
            {
                isMedRecTakeawayBlank = false;

            }
            /*if (serviceTakeAwayMedReconciliation.TakeawayTypeInformationType == "Patient")
            {
                if (serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet == null || serviceTakeAwayMedReconciliation.IsVaccination == null || serviceTakeAwayMedReconciliation.RecCompleted == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveType) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecSendType) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) || serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.LastName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.State) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.City) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.ZipCode))
                {
                    isValidate = false;
                }
            }*/

            if (serviceTakeAwayMedReconciliation.TakeawayTypeInformationType == "Patient")
            {
                if(string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) || serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.LastName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.State) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.City) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.ZipCode))
                {
                    isValidate=false;
                }
                if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) && serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.FirstName) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Contact.LastName) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.State) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.City) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.AddressLineOne) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.CmrPatient.Address.ZipCode))
                {
                    isMedRecTakeawayBlank = false;
                }
            }

            /*  if (serviceTakeAwayMedReconciliation.TakeawayTypeInformationType == "someone else")
          {
              if (serviceTakeAwayMedReconciliation.IsDiscussExerciseDiet == null || serviceTakeAwayMedReconciliation.IsVaccination == null || serviceTakeAwayMedReconciliation.RecCompleted == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveType) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecSendType) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) || serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null
              || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.LastName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.ZipCode) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.State) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.City))
              {
                  isValidate = false;
              }
          }*/
            if (serviceTakeAwayMedReconciliation.TakeawayTypeInformationType == "someone else")
            {
                if(string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) || serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.FirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.LastName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.ZipCode) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.State) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.AddressLineOne) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.City))
                {
                    isValidate = false;
                }
                if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.LanguageTypeTemplate) && serviceTakeAwayMedReconciliation.PatientTakeawayDeliveryDate == null || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.FirstName) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Contact.LastName) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.ZipCode) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.State) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.AddressLineOne) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.Address.City))
                {
                    isMedRecTakeawayBlank = false;
                }

            }

            /* if (serviceTakeAwayMedReconciliation.RecReceiveType == "Healthcare Provider" || serviceTakeAwayMedReconciliation.RecReceiveType == "Caregiver")
         {
             if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeFirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeLastName))
             {
                 isValidate = false;
             }

         }*/

            if (serviceTakeAwayMedReconciliation.RecReceiveType == "Healthcare Provider" || serviceTakeAwayMedReconciliation.RecReceiveType == "Caregiver")
            {
                if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeFirstName) || string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeLastName))
                {
                    isValidate = false;
                }
                if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeFirstName) && string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.RecReceiveTypeLastName))
                {
                    isMedRecTakeawayBlank = false;
                }

            }
            
            if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.TakeawayReceiveType) || serviceTakeAwayMedReconciliation.IsPatientCongnitivelyImpaired == null || serviceTakeAwayMedReconciliation.IsFollowUpAppointment == null || serviceTakeAwayMedReconciliation.IsPatientLongTermFacility == null)
            {
                IsValidateReview = false;
            }
            if (string.IsNullOrEmpty(serviceTakeAwayMedReconciliation.TakeawayReceiveType) && (serviceTakeAwayMedReconciliation.IsPatientCongnitivelyImpaired == null && (serviceTakeAwayMedReconciliation.IsFollowUpAppointment == null) && (serviceTakeAwayMedReconciliation.IsPatientLongTermFacility == null)))
            {
                isMedRecReviewBlank = true;
            }
            if((serviceTakeAwayMedReconciliation.IsPatientCongnitivelyImpaired == true || serviceTakeAwayMedReconciliation.IsPatientCongnitivelyImpaired == false) || (serviceTakeAwayMedReconciliation.IsFollowUpAppointment == true || serviceTakeAwayMedReconciliation.IsFollowUpAppointment == false) || (serviceTakeAwayMedReconciliation.IsPatientLongTermFacility == true || serviceTakeAwayMedReconciliation.IsPatientLongTermFacility == false))
            {
                isMedRecReviewBlank = false;
            }

            serviceTakeAwayMedReconciliation.IsValidate = isValidate;
            serviceTakeAwayMedReconciliation.IsValidateReview = IsValidateReview;
            serviceTakeAwayMedReconciliation.IsMedRecTakeawayBlank = isMedRecTakeawayBlank;
            serviceTakeAwayMedReconciliation.IsMedRecReviewBlank = isMedRecReviewBlank;
            response.Success = true;
            response.Message = "ServiceTakeawayInformation retrived successfully";
            response.Data = serviceTakeAwayMedReconciliation;
            return response;
        }

        public async Task<Response<ServiceTakeAwayMedReconciliation>> SendCmrPdfEmailByMedReconciliation(string email, int patientId)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            var takeawayDb = await _serviceTakeAwayMedReconciliationData.GetTakeawayVerifyByPatientId(patientId);
            if (takeawayDb != null)
            {
                var hours = DateTime.Now.Subtract(takeawayDb.CreatedAt).TotalHours;
                if (hours >= 72)
                {
                    takeawayDb.CreatedAt = DateTime.Now;
                    await _serviceTakeAwayMedReconciliationData.UpdateTakeawayVerify(takeawayDb);
                }
                await _emailService.SendMedRecPdfEmail(takeawayDb, email, patientId);
            }
            else
            {
                TakeawayVerify takeawayVerify = new TakeawayVerify
                {
                    UUID = RandomString(8),
                    PatientId = patientId,
                    CreatedAt = DateTime.Now,
                    IsServiceTakeAwayMedRec = true,
                };
                await _serviceTakeAwayMedReconciliationData.AddTakeawayVerify(takeawayVerify);
                await _emailService.SendMedRecPdfEmail(takeawayVerify, email, patientId);
            }
            response.Success = true;
            response.Message = "Email send successfully";
            return response;
        }

        public async Task<Response<ServiceTakeAwayMedReconciliation>> CheckLinkExpiredByMedReconciliation(int patientId, string uuid)
        {
            Response<ServiceTakeAwayMedReconciliation> response = new Response<ServiceTakeAwayMedReconciliation>();
            var takeawayVerify = await _serviceTakeAwayMedReconciliationData.GetTakeawayVerifyByPatientIdAndUUID(patientId, uuid);
            response.Success = false;
            response.Message = "Link Expired";
            if (takeawayVerify == null)
            {
                return response;
            }
            var hours = DateTime.Now.Subtract(takeawayVerify.CreatedAt).TotalHours;
            if (hours >= 72)
            {
                await _serviceTakeAwayMedReconciliationData.DeleteTakeawayVerify(takeawayVerify);
                return response;
            }
            response.Message = "Valid Link";
            response.Success = true;
            return response;
        }

        public async Task<Response<byte[]>> PatientVerificationForPdfMedReconciliation(string lastname, DateTime dob, int patientId, string uuid)
        {
            Response<byte[]> response = new Response<byte[]>();
            var takeawayVerify = await _serviceTakeAwayMedReconciliationData.GetTakeawayVerifyByPatientIdAndUUID(patientId, uuid);
            if (takeawayVerify == null)
            {
                response.Message = "Invalid Details";
                response.Success = false;
                return response;
            }
            var hours = DateTime.Now.Subtract(takeawayVerify.CreatedAt).TotalHours;
            if (hours >= 72)
            {
                await _serviceTakeAwayMedReconciliationData.DeleteTakeawayVerify(takeawayVerify);
                response.Message = "Invalid Details";
                response.Success = false;
                return response;
            }
            var patient = await _serviceTakeAwayMedReconciliationData.PatientVerificationForPdf(lastname, dob, patientId);

            if (patient != null)
            {
                response.Success = true;
                takeawayVerify.LastModified = DateTime.Now;
                await _serviceTakeAwayMedReconciliationData.UpdateTakeawayVerify(takeawayVerify);
                response.Message = "Verification successfully";
                response.Data = await _pdfStorageService.GetPDFURIForMedRec(patientId);
                return response;

            }
            response.Message = "Please enter the correct Last Name and Date of Birth values.";
            response.Success = false;
            return response;

        }

        public async Task<Response<TakeawayVerify>> GetTakeawayVerifyForMedRecByPatientId(int patientId)
        {
            Response<TakeawayVerify> response = new Response<TakeawayVerify>();
            var takeawayDb = await _serviceTakeAwayMedReconciliationData.GetTakeawayVerifyByPatientId(patientId);
            if (takeawayDb != null)
            {
                var hours = DateTime.Now.Subtract(takeawayDb.CreatedAt).TotalHours;
                if (hours >= 72)
                {
                    takeawayDb.CreatedAt = DateTime.Now;
                    await _serviceTakeAwayMedReconciliationData.UpdateTakeawayVerify(takeawayDb);
                }
                response.Data = takeawayDb;
                return response;
            }
            TakeawayVerify takeawayVerify = new TakeawayVerify
            {
                UUID = RandomString(8),
                PatientId = patientId,
                CreatedAt = DateTime.Now,
                IsServiceTakeAwayMedRec = true,
            };
            await _serviceTakeAwayMedReconciliationData.AddTakeawayVerify(takeawayVerify);
            response.Data = takeawayVerify;
            return response;


        }
    }
}
