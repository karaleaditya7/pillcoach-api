using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OntrackDb.Authentication;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System.Web.Helpers;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using Azure.Storage.Blobs.Specialized;
using Twilio;
using Twilio.Rest.Conversations.V1.Service;
using OntrackDb.Context;
using Microsoft.EntityFrameworkCore;

namespace OntrackDb.Service
{
    public class PharmacyService :IPharmacyService
    {
        private readonly IPharmacyData _pharmacyData;
        private readonly IPatientData _patientData;
        private readonly INoteData _noteData;
        private readonly IMedicationService _medicationService;
        private readonly IMedicationData _medicationData;
        private readonly IPatientService _patientService;
        readonly IUserData _userData;
        readonly IPatientPdcService _patientPdcService;
        readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;

        public PharmacyService(IPharmacyData pharmacyData, INoteData noteData, IMedicationService medicationService, IPatientService patientService, IMedicationData medicationData, IPatientData patientData, IUserData userData, IPatientPdcService patientPdcService, IHttpContextAccessor httpContextAccessor, BlobServiceClient blobServiceClient, IConfiguration configuration,ApplicationDbContext applicationDbContext)
        {
            _pharmacyData = pharmacyData;
            _noteData = noteData;
            _medicationService = medicationService;
            _medicationData = medicationData;
            _patientService = patientService;
            _patientData = patientData;
            _userData = userData;
            _patientPdcService = patientPdcService;
            _httpContextAccessor = httpContextAccessor;
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Response<Pharmacy>> AddPharmacy(PharmacyModel model, string initUserId)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();

            if (model.Address == null)
            {
                response.Message = "Address Info Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                response.Message = "Name is Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PharmacyManager))
            {
                response.Message = "PharmacyManager  is Missing";
                return response;
            }

            if (!string.IsNullOrWhiteSpace(model.TwilioSmsNumber) && !model.TwilioSmsNumber.StartsWith("+1"))
            {
                model.TwilioSmsNumber = $"+1{model.TwilioSmsNumber}";
            }

            if (!string.IsNullOrEmpty(model.NpiNumber))
            {
                var pharma = await _pharmacyData.GetPharmacyByNpiNumber(model.NpiNumber);
                if (pharma != null)
                {
                    response.Message = "NPI number already exists";
                    return response;
                }
            }

            Pharmacy pharmacy = new Pharmacy
            {
                Name = model.Name,
                NcpdpNumber = model.NcpdpNumber,
                NpiNumber = model.NpiNumber,
                Address = model.Address,
                PharmacyManager = model.PharmacyManager,
                Contact = model.Contact,
                Note= new Note { 
                    text = "",
                    LastUpdated = DateTime.Today
                },
                TwilioSmsNumber = model.TwilioSmsNumber
            };         
            var result =await _pharmacyData.AddPharmacy(pharmacy);

            if (result == null)
            {
                response.Message = "Error while creating pharmacy";
                response.Success = false;
                return response;
            }
            else if (!string.IsNullOrWhiteSpace(initUserId) && pharmacy.Id > 0)
            {
                await _userData.AssignPharmacies(initUserId, new List<string>() { pharmacy.Id.ToString() }, false);
            }

            var containerName = _configuration.GetValue<string>("PatientImport:ContainerName");
            var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            var folderPath = $"{pharmacy.NpiNumber}/tempfile";
            var blockBlobClient = blobContainer.GetBlockBlobClient(folderPath);
            await blockBlobClient.UploadAsync(new MemoryStream());
            await blockBlobClient.DeleteAsync();

            response.Success = true;
            response.Data = pharmacy;
            response.Message = "Pharmacy created successfully!";
            return response;
        }

        public async Task<Response<List<string>>> DeletePharmacy(int pharmacyId)
        {
            Response<List<string>> response = new Response<List<string>>();
            _applicationDbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(5)); // 5 minutes timeout
            Pharmacy pharmacy = await _pharmacyData.GetPharmacyById(pharmacyId);

            if(pharmacy == null)
            {
                response.Message = "Pharmacy is not found";
                response.Success = false;
                return response;
            }
            var patients = await _patientData.GetPatientsByPharmacyId(pharmacyId);
            List<string> patientPhoneNumbers = new List<string>();
            if(patients != null)
            {
                foreach(Patient patient in patients)
                {
                        patientPhoneNumbers.Add(patient.Contact.Id.ToString());
                        var result =  await _patientService.DeletePatientForPharmacy(patient.Id);
                }
            }
            await this.DeleteTwilioConversations(patientPhoneNumbers);
            await _pharmacyData.DeleteImportNotification(pharmacy.Id);
            var containerName = _configuration.GetValue<string>("PatientImport:ContainerName");
            var blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            var folderName = $"{pharmacy.NpiNumber}";
            var blobs = blobContainer.GetBlobs(prefix : folderName);
            foreach(var blob in blobs)
            {
                if (blob.Name.Contains("/"))
                {
                    var blobClient = blobContainer.GetBlobClient(blob.Name);
                    await blobClient.DeleteIfExistsAsync();
                }
            }
            var folderClient = blobContainer.GetBlobClient(folderName);
            await folderClient.DeleteIfExistsAsync();
            
            await _pharmacyData.DeletePharmacyHardCore(pharmacy);

            response.Message = "Pharmacy Deleted successfully";
            response.Success = true;
            //response.Data = patientPhoneNumbers;
            return response;
        }


        public async Task<Response<Pharmacy>> GetPharmacies(int recordNumber, int pageLimit,DateTime startDate,DateTime endDate,int month,string keywords,string sortDirection,  string filterType, string filterValue,string filterCategory)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            var pharmacies = await _pharmacyData.GetPharmacies(recordNumber,pageLimit,keywords,sortDirection, filterType,filterValue,filterCategory);

            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }
            foreach (Pharmacy pharmacy in pharmacies)
            {
                var cholesterolPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
                var diabetesPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
                var rasaPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

                pharmacy.CholestrolPDC = cholesterolPDC.Value;
                pharmacy.RASAPDC = rasaPDC.Value;
                pharmacy.DiabetesPDC = diabetesPDC.Value;

                //var patients = await _patientService.GetPatientsByPharmacyId(pharmacy.Id);
                //pharmacy.NewPatient = _patientService.countNewPatient(patients.Data);

                pharmacy.NewPatient = _patientService.countNewPatient(pharmacy.Id);

                var medications = await _medicationData.getAllMedicationsByPharmacyIDForAll(pharmacy.Id);

                medications = medications.Where(p => p.Patient != null && p.Patient.IsDeleted == false && p.Condition != null).GroupBy(p => (p.Condition, p.Patient)).Select(p => p.LastOrDefault()).ToList();

                pharmacy.UpcomingRefill = await _medicationService.countDueForRefill(medications);


            }

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "PDC Category and Average":
                        switch (filterCategory)
                        {
                            case "Cholesterol":
                                string[] filterArrayCholesterol = filterValue.Split("-");

                                double filterMinValue = Convert.ToDouble(filterArrayCholesterol[0].Split("%")[0]);
                                double filterMaxValue = Convert.ToDouble(filterArrayCholesterol[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC <= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.CholestrolPDC >= filterMinValueD && p.CholestrolPDC >= filterMaxValueD).ToList();
                                break;
                        }
                        break;
                    case "Due for Refills":
                        if (filterValue == "High")
                        {
                            pharmacies = pharmacies.OrderByDescending(p => p.UpcomingRefill).ToList();
                        }
                        else if (filterValue == "Low")
                        {
                            pharmacies = pharmacies.OrderBy(p => p.UpcomingRefill).ToList();
                        }
                        break;
                    case "New Patients":
                        if (filterValue == "High")
                        {
                            pharmacies = pharmacies.OrderByDescending(p => p.NewPatient).ToList();
                        }
                        else if (filterValue == "Low")
                        {
                            pharmacies = pharmacies.OrderBy(p => p.NewPatient).ToList();
                        }
                        break;
                }
            }


            response.Success = true;
            response.DataList = pharmacies;
            return response;
        }

        public async Task<Response<Pharmacy>> UpdatePharmacy(PharmacyModel model)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            Pharmacy pharmacy = await _pharmacyData.GetPharmacyById(model.Id);
            if (pharmacy == null)
            {
                response.Success = false;
                response.Message = "Pharmacy not found";
                return response;
            }
            pharmacy.Name = model.Name;
            pharmacy.PharmacyManager = model.PharmacyManager;
            pharmacy.NcpdpNumber = model.NcpdpNumber;
            pharmacy.NpiNumber = model.NpiNumber;
            Contact contact = await _pharmacyData.GetContactById(pharmacy.Contact.Id);
            contact.PrimaryPhone = model.Contact.PrimaryPhone;
            contact.Fax = model.Contact.Fax.ToString();
            pharmacy.Contact = contact;
            var address = await _pharmacyData.GetAddressById(pharmacy.Address.Id);
            address.AddressLineOne = model.Address.AddressLineOne;
            address.City = model.Address.City;
            address.State = model.Address.State;
            address.ZipCode = model.Address.ZipCode;
            pharmacy.Address = address;

            if (!string.IsNullOrWhiteSpace(model.TwilioSmsNumber) && !model.TwilioSmsNumber.StartsWith("+1"))
            {
                model.TwilioSmsNumber = $"+1{model.TwilioSmsNumber}";
            }

            if (!string.IsNullOrEmpty(model.NpiNumber))
            {
                var pharma = await _pharmacyData.GetPharmacyByNpiNumber(model.NpiNumber);
                if (pharma != null)
                {
                    response.Message = "NPI number already exists";
                    return response;
                }
            }

            pharmacy.TwilioSmsNumber = model.TwilioSmsNumber;

            var result = await _pharmacyData.UpdatePharmacy(pharmacy);
            
            response.Success = true;
            response.Message = "Pharmacy Updated successfully!";
            return response;
        }


        public async Task<PharmacyDto> CalucalateAvergaePdcForPharmacy(int pharmacyId, DateTime startDate, DateTime endDate, int month)
        {
            var pharmacyDto = await _pharmacyData.GetPharmacyWithNoteByIdForPDCForDto(pharmacyId);
            var patients = await _patientService.GetPatientsByPharmacyId(pharmacyId);

            pharmacyDto.NewPatient =  _patientService.countNewPatient(patients.Data);

            var medications = await _medicationData.getAllMedicationsByPharmacyIDForAll(pharmacyId);
            var medications1 = new List<Medication>();
            medications1 = medications.Where(p => p.Patient.IsDeleted == false && p.Condition != null).GroupBy(p => (p.Condition, p.Patient)).Select(p => p.LastOrDefault()).ToList();

            pharmacyDto.UpcomingRefill = await _medicationService.countDueForRefill(medications1);

            // get PDC data

            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }

            var cholesterolPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
            var diabetesPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
            var rasaPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacyId, PDC.RASA.ToString(), pdcMonth, month, queryType);

            pharmacyDto.CholestrolPDC = cholesterolPDC.Value;
            pharmacyDto.DiabetesPDC = diabetesPDC.Value;
            pharmacyDto.RASAPDC = rasaPDC.Value;

            pharmacyDto.CholesterolSummary = new()
            {
                PDC = cholesterolPDC.Value,
                TotalPatient = cholesterolPDC.TotalPatients,
                NonAdherenceCount = cholesterolPDC.NonAdherenceCount
            };

            pharmacyDto.DiabetesSummary = new()
            {
                PDC = diabetesPDC.Value,
                TotalPatient = diabetesPDC.TotalPatients,
                NonAdherenceCount = diabetesPDC.NonAdherenceCount
            };

            pharmacyDto.RASASummary = new()
            {
                PDC = rasaPDC.Value,
                TotalPatient = rasaPDC.TotalPatients,
                NonAdherenceCount = rasaPDC.NonAdherenceCount
            };

            // get due for refill for each conditions

            for (int i = 0; i < patients.Data.Count; i++)
            {
                var patientId = patients.Data[i].Id;

                if (!pharmacyDto.CholesterolSummary.DueForRefill)
                {
                    var cholesterolMedications = medications1.Where(m => m.Patient.Id == patientId && m.Condition == PDC.Cholesterol.ToString()).ToList();

                    if (cholesterolMedications.Count > 0)
                    {
                        var dueForRefill = await _medicationService.countDueForRefill(cholesterolMedications);
                        pharmacyDto.CholesterolSummary.DueForRefill = dueForRefill > 0;
                    }
                }

                if (!pharmacyDto.DiabetesSummary.DueForRefill)
                {
                    var diabetesMedications = medications1.Where(m => m.Patient.Id == patientId && m.Condition == PDC.Diabetes.ToString()).ToList();

                    if (diabetesMedications.Count > 0)
                    {
                        var dueForRefill = await _medicationService.countDueForRefill(diabetesMedications);
                        pharmacyDto.DiabetesSummary.DueForRefill = dueForRefill > 0;
                    }
                }

                if (!pharmacyDto.RASASummary.DueForRefill)
                {
                    var rasaMedications = medications1.Where(m => m.Patient.Id == patientId && m.Condition == PDC.RASA.ToString()).ToList();

                    if (rasaMedications.Count > 0)
                    {
                        var dueForRefill = await _medicationService.countDueForRefill(rasaMedications);
                        pharmacyDto.RASASummary.DueForRefill = dueForRefill > 0;
                    }
                }
            }

            return pharmacyDto;
        }



        public async Task<Pharmacy> CalucalateAvergaePdcForPharmacyGraph(Pharmacy pharmacy, DateTime startDate, DateTime endDate)
        {

            List<Patient> CholestrolPdcPatients = await _medicationService.CalculatePdcforPatientsGraph(pharmacy.Patients, PDC.Cholesterol.ToString(), startDate, endDate);
            List<Patient> RASAPdcPatients = await _medicationService.CalculatePdcforPatientsGraph(pharmacy.Patients, PDC.RASA.ToString(), startDate, endDate);
            List<Patient> DiabetesPdcPatients = await _medicationService.CalculatePdcforPatientsGraph(pharmacy.Patients, PDC.Diabetes.ToString(), startDate, endDate);

            Double CholestrolSum = 0.00;
            int CholestrolPdcPatient = 0;
            for (int i = 0; i < CholestrolPdcPatients.Count; i++)
            {
                if (CholestrolPdcPatients[i].CholestrolPDC != -1)
                {
                    CholestrolSum = CholestrolSum + CholestrolPdcPatients[i].CholestrolPDC;
                    CholestrolPdcPatient++;
                }
            }
            CholestrolSum = Convert.ToDouble(String.Format("{0:0.0}", CholestrolSum));
            pharmacy.CholestrolPDC = CholestrolPdcPatient == 0 ? -1 : (CholestrolSum / (Double)CholestrolPdcPatient);

            Double RASASum = 0.00;
            int RASAPatient = 0;
            for (int i = 0; i < RASAPdcPatients.Count; i++)
            {
                if (RASAPdcPatients[i].RASAPDC != -1)
                {
                    RASASum = RASASum + RASAPdcPatients[i].RASAPDC;
                    RASAPatient++;
                }
            }
            RASASum = Convert.ToDouble(String.Format("{0:0.0}", RASASum));
            pharmacy.RASAPDC = RASAPatient == 0 ? -1 : (RASASum / (Double)RASAPatient);
            Double DiabetesSum = 0.00;
            int DiabetesPatient = 0;
            for (int i = 0; i < DiabetesPdcPatients.Count; i++)
            {
                if (DiabetesPdcPatients[i].DiabetesPDC != -1)
                {
                    DiabetesSum = DiabetesSum + DiabetesPdcPatients[i].DiabetesPDC;
                    DiabetesPatient++;
                }
            }
            DiabetesSum = Convert.ToDouble(String.Format("{0:0.0}", DiabetesSum));
            pharmacy.DiabetesPDC = DiabetesPatient == 0 ? -1 : (DiabetesSum / (Double)DiabetesPatient);
            pharmacy.DiabetesPDC = Math.Round(pharmacy.DiabetesPDC, 2);
            pharmacy.CholestrolPDC = Math.Round(pharmacy.CholestrolPDC, 2);
            pharmacy.RASAPDC = Math.Round(pharmacy.RASAPDC, 2);
            return pharmacy;
        }

      
        public async Task<Response<Pharmacy>> GetPharmacyById(int id,DateTime startDate,DateTime endDate,int month)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            PatientDiseaseCount patientDiseaseCount = new PatientDiseaseCount();
            var pharmacy = await _pharmacyData.GetPharmacyWithNoteById(id);

            if (pharmacy == null || pharmacy.IsDeleted)
            {

                response.Message = "Error while getting Pharmacy by id";
                response.Success = false;
                return response;
            }

            if (pharmacy.Note != null && !string.IsNullOrWhiteSpace(pharmacy.Note.UserId))
            {
                var user = await _userData.GetUserById(pharmacy.Note.UserId);

                if (user != null) pharmacy.Note.LastUpdatedBy = $"{user.FirstName} {user.LastName}".Trim();
            }
            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }
            var cholesterolPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
            var diabetesPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
            var rasaPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

            pharmacy.CholestrolPDC = cholesterolPDC.Value;
            pharmacy.RASAPDC = rasaPDC.Value;
            pharmacy.DiabetesPDC = diabetesPDC.Value;


            //pharmacy.PatientCount = new Dictionary<string, int>();
            //pharmacy.PatientCount.Add(PDC.Diabetes.ToString(),  _patientService.countPatientByCondition(pharmacy.Patients, PDC.Diabetes.ToString()));
            //pharmacy.PatientCount.Add(PDC.Cholesterol.ToString(), _patientService.countPatientByCondition(pharmacy.Patients, PDC.Cholesterol.ToString()));
            //pharmacy.PatientCount.Add(PDC.RASA.ToString(), _patientService.countPatientByCondition(pharmacy.Patients, PDC.RASA.ToString()));

            //pharmacy.TotalCholesterolPatient = _patientService.GetAllPharmacyPatientsByCondition(0,0,startDate, endDate, id,"Cholesterol", month, "","asc","","","").Result.Count();
            //pharmacy.TotalDiabetesPatient = _patientService.GetAllPharmacyPatientsByCondition(0, 0, startDate, endDate, id, "Diabetes", month, "", "asc", "", "", "").Result.Count();
            //pharmacy.TotalRASAPatient = _patientService.GetAllPharmacyPatientsByCondition(0, 0, startDate, endDate, id, "RASA", month, "", "asc", "", "", "").Result.Count();

            patientDiseaseCount = await _patientService.GetPatientDiseaseCountById(id,startDate,endDate,month,"Cholesterol");
            pharmacy.TotalCholesterolPatient = patientDiseaseCount.PatientCount;
            patientDiseaseCount = await _patientService.GetPatientDiseaseCountById(id, startDate, endDate, month, "Diabetes");
            pharmacy.TotalDiabetesPatient = patientDiseaseCount.PatientCount;
            patientDiseaseCount = await _patientService.GetPatientDiseaseCountById(id, startDate, endDate, month, "RASA");
            pharmacy.TotalRASAPatient = patientDiseaseCount.PatientCount;

            response.Success = true;
            response.Message = "Pharmacy retrived successfully";
            response.Data = pharmacy;
            return response;
            
        }


        public async Task<Response<PharmacyDto>> GetPharmacyByIdForPDCForDto(int id, DateTime startDate, DateTime endDate, int month)
        {
            Response<PharmacyDto> response = new Response<PharmacyDto>();
            var pharmacy = await _pharmacyData.GetPharmacyWithNoteByIdForPDCForDto(id);

            if (pharmacy == null || pharmacy.IsDeleted)
            {
                response.Message = "Error while getting Pharmacy by id";
                response.Success = false;
                return response;
            }


            var calculatedPhamacy = await this.CalucalateAvergaePdcForPharmacy(pharmacy.Id, startDate, endDate, month);
           

            response.Success = true;
            response.Message = "Pharmacy retrived successfully";
            response.Data = calculatedPhamacy;
            return response;

        }

        public async Task<Response<Pharmacy>> GetPharmacyByIdForGraph(int id, DateTime startDate, DateTime endDate)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            var pharmacy = await _pharmacyData.GetPharmacyWithNoteById(id);

            if (pharmacy == null || pharmacy.IsDeleted)
            {

                response.Message = "Error while getting Pharmacy by id";
                response.Success = false;
                return response;
            }


            Pharmacy calculatedPhamacy = await this.CalucalateAvergaePdcForPharmacyGraph(pharmacy, startDate, endDate);
            calculatedPhamacy.PatientCount = new Dictionary<string, int>();
            calculatedPhamacy.PatientCount.Add(PDC.Diabetes.ToString(),  _patientService.countPatientByCondition(pharmacy.Patients, PDC.Diabetes.ToString()));
            calculatedPhamacy.PatientCount.Add(PDC.Cholesterol.ToString(),  _patientService.countPatientByCondition(pharmacy.Patients, PDC.Cholesterol.ToString()));
            calculatedPhamacy.PatientCount.Add(PDC.RASA.ToString(),  _patientService.countPatientByCondition(pharmacy.Patients, PDC.RASA.ToString()));

            response.Success = true;
            response.Message = "Pharmacy retrived successfully";
            response.Data = calculatedPhamacy;
            return response;

        }

        public async Task<Response<Note>> AddPharmacyNote(NoteModel noteModel,int pharmacyId)
        {
            Response<Note> response = new Response<Note>();
            Pharmacy pharmacy = await _pharmacyData.GetPharmacyById(pharmacyId);
            if (pharmacy == null)
            {
                response.Message = "Pharmacy Not Found";
                response.Success = false;
                return response;
            }

            var userId = _httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

            Note note = new Note
            {
                text = noteModel.Text,
                LastUpdated = new DateTime(),
                UserId = userId
            };
            var result = await _noteData.AddPharmacyNote(note);
            if (result == null)
            {
                response.Message = "Error while creating PharmacyNote";
                response.Success = false;
                return response;
            }
            pharmacy.Note = note;
            var resultUp = _pharmacyData.UpdatePharmacy(pharmacy);
            if (resultUp == null)
            {
                response.Message = "Error while creating PharmacyNote";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = note;
            response.Message = "PharmacyNote created successfully!";
            return response;
        }
        public async Task<Response<Note>> GetPharmacyNoteById(int pharmacyId)
        {
            Response<Note> response = new Response<Note>();
            var pharmacy = await _pharmacyData.GetPharmacyWithNoteById(pharmacyId);

            if (pharmacy == null)
            {
                response.Message = "Error while getting PharmacyNote by id";
                response.Success = false;
                return response;
            }
            response.Success = true;
            response.Message = "PharmacyNote retrived successfully";
            response.Data = pharmacy.Note;
            return response;
           
        }
        public async Task<Response<Note>> UpdatePharmacyNote(NoteModel noteModel)
        {
            Response<Note> response = new Response<Note>();
            Note noteDb = await _noteData.GetPharmacyNoteById(noteModel.Id);
            if (noteDb == null)
            {
                response.Message = "Pharmacy Note not found";
                response.Success = false;
                return response;
            }
            noteDb.text = noteModel.Text;
            noteDb.LastUpdated = new DateTime();
            
            var result = await _noteData.UpdatePharmacyNote(noteDb);

            response.Success = true;
            response.Data = noteDb;
            response.Message = "Note Updated Successfully!";
            return response;

        }

        
        public async  Task<Response<Pharmacy>> GetPharmaciesByUserID(string userId) {
            Response<Pharmacy> response = new Response<Pharmacy>();
            List<Pharmacy> pharmacies =  await _pharmacyData.GetPharmaciesByUserId(userId);
            
            response.Success = true;
            response.DataList = pharmacies;
            response.Message = "pharamacie list for user";
            return response;
        }
        public async Task<Response<Pharmacy>> GetPharmaciesByUserIDWithPagination(int pageRecords, int pageLimit, string userId, DateTime startDate, DateTime endDate, int month,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            var pharmacies = await _pharmacyData.GetPharmaciesByUserIdWithPagination(pageRecords,pageLimit, userId, keywords,sortDirection,filterType,filterValue,filterCategory);

            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }
            foreach (Pharmacy pharmacy in pharmacies)
            {
                var cholesterolPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Cholesterol.ToString(), pdcMonth, month, queryType);
                var diabetesPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.Diabetes.ToString(), pdcMonth, month, queryType);
                var rasaPDC = await _patientPdcService.GetPdcForPharmacyAsync(pharmacy.Id, PDC.RASA.ToString(), pdcMonth, month, queryType);

                pharmacy.CholestrolPDC = cholesterolPDC.Value;
                pharmacy.RASAPDC = rasaPDC.Value;
                pharmacy.DiabetesPDC = diabetesPDC.Value;

                var patients = await _patientService.GetPatientsByPharmacyId(pharmacy.Id);

                pharmacy.NewPatient =  _patientService.countNewPatient(patients.Data);

                pharmacy.UpcomingRefill = patients.Data.Count(p => (p.CholesterolRefillDue || p.DiabetesRefillDue || p.RasaRefillDue)
                      && p.Medications.Any(m => m.IsActive && m.RefillDue && new string[] { "Diabetes", "Cholesterol", "RASA" }.Contains(m.Condition) && Convert.ToInt32(m.RefillsRemaining) > 0));


                pharmacy.Patients = null; // patient's list is not required here - gets stuck while serializing for response
            }

            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                        case "PDC Category and Average":
                        switch (filterCategory)
                        {
                            case "Cholesterol":
                                string[] filterArrayCholesterol = filterValue.Split("-");

                                double filterMinValue = Convert.ToDouble(filterArrayCholesterol[0].Split("%")[0]);
                                double filterMaxValue = Convert.ToDouble(filterArrayCholesterol[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC >= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                pharmacies = pharmacies.Where(p => p.DiabetesPDC >= filterMinValueD && p.DiabetesPDC >= filterMaxValueD).ToList();
                                break;

                        }
                                break;
                        case "Due for Refills":
                                if (filterCategory == "High")
                                {
                                    pharmacies = pharmacies.OrderByDescending(p => p.UpcomingRefill).ToList();
                                }
                                else if (filterCategory == "Low")
                                {
                                    pharmacies = pharmacies.OrderBy(p => p.UpcomingRefill).ToList();
                                }
                                break;
                        case "New Patients":
                            if (filterCategory == "High")
                            {
                                pharmacies = pharmacies.OrderByDescending(p => p.NewPatient).ToList();
                            }
                            else if (filterCategory == "Low")
                            {
                                pharmacies = pharmacies.OrderBy(p => p.NewPatient).ToList();
                            }
                            break;

                }
                           
            }

            response.Success = true;
            response.DataList = pharmacies;
            response.Message = "pharamacie list for user";
            return response;
        }

        public async Task<Pharmacy> GetPharmacyByNpiNumber(string npiNumber)
        {
            return await _pharmacyData.GetPharmacyByNpiNumber(npiNumber);
        }
        public async Task<Response<Pharmacy>> GetAllPharmacyNames()
        {
            Response<Pharmacy> response = new Response<Pharmacy>();
            var pharmacies = await _pharmacyData.GetAllPharmacyNames();
            response.Success = true;
            response.DataList = pharmacies;
            return response;
        }

        public async Task DeleteTwilioConversations(List<string> contactId)
        {
            string accountSid = _configuration["twilioAccountSid"];
            string authToken = _configuration["twimlAuthToken"];
            string serviceSid = _configuration["serviceSid"];
            TwilioClient.Init(accountSid, authToken);
            var conversations = ConversationResource.Read(
                pathChatServiceSid: serviceSid
            );

            foreach (var conversation in conversations)
            {
                string[] twilioContactId = conversation.UniqueName.Split('_');
                if (contactId.Any(id => id == twilioContactId[1]))
                {
                    ConversationResource.Delete(pathChatServiceSid: serviceSid, pathSid: conversation.Sid);
                }
            }

        }
    }
}
