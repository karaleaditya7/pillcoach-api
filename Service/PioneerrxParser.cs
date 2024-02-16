using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class PioneerrxParser : IOntrackParser
    {

        //        Medication fields missing:
        //            -condition treated
        //            -nextfilldate
        //            -paydue

        private readonly IWebHookService _webHookService;
        private readonly ICmrMedicationService _cmrMedicationService;
        private readonly INdcApiService _ndcApiService;

        public PioneerrxParser(IWebHookService webHookService, ICmrMedicationService cmrMedicationService, INdcApiService ndcApiService)
        {
            _webHookService = webHookService;
            _cmrMedicationService = cmrMedicationService;
            _ndcApiService= ndcApiService;  
        }

        //public async Task ParsePayload (JObject payload)
        //{
        //    ImportData importData = new ImportData();
        //    try
        //    {
        //        importData = await _webHookService.DumpImportData(payload,"In Progress");
        //        Pharmacy pharmacy = ParsePharmacy(payload);
        //        Patient patient = ParsePatient(payload);
        //        Medication medication = ParseMedication(payload);
        //       // Doctor doctor = ParseDoctor(payload);
        //        pharmacy.ImportData = importData;
        //        patient.ImportData = importData;
        //        medication.ImportData = importData;
        //        Pharmacy pharmacyDb = await _webHookService.AddOrUpdatePharmacy(pharmacy);
        //        Patient patientDb = await _webHookService.AddOrUpdatePatient(patient);
        //        Medication medicationDb = await _webHookService.AddOrUpdateMedication(medication);
        //        importData.status = "Completed";
        //        importData.message = "Data added Successfully";
        //        var importDataUp = _webHookService.UpdateImportData(importData);
        //    }
        //    catch (Exception ex)
        //    {
        //        importData.message = ex.Message;
        //        importData.status = "Failed";
        //        await _webHookService.UpdateImportData(importData);
        //    }
        //}

        public Doctor ParseDoctor(JObject payload)
        {
            JsonDocument document = JsonDocument.Parse(payload.ToString());
            JsonElement root = document.RootElement;
            JsonElement body = root.GetProperty("Body");
            JsonElement prescriberNode = body.GetProperty("Prescribers").GetProperty("Prescriber")[0];

            JsonElement Identification = prescriberNode.GetProperty("Identification");
            JsonElement Name = prescriberNode.GetProperty("Name");
            JsonElement Address = prescriberNode.GetProperty("Addresses").GetProperty("Address");
            JsonElement PhoneNumber = prescriberNode.GetProperty("PhoneNumbers").GetProperty("PhoneNumber");

            Doctor doctor = new Doctor()
            {
                Contact = new Contact()
                {
                    FirstName = Name.GetProperty("FirstName").ToString(),
                    LastName = Name.GetProperty("LastName").ToString(),
                    PrimaryPhone = PhoneNumber[0].GetProperty("Number").ToString(),
                    Fax = PhoneNumber[1].GetProperty("Number").ToString()
                },
                PrescriberVendorRxID = Identification.GetProperty("PrescriberPioneerRxID").ToString(),
                Npi = Identification.GetProperty("NPI").ToString(),

            };
            return doctor;
        }

        public  Medication ParseMedication(JObject payload)
        {
            JsonDocument document = JsonDocument.Parse(payload.ToString());
            JsonElement root = document.RootElement;
            JsonElement body = root.GetProperty("Body");
            JsonElement rxNode = body.GetProperty("Rx");

            JsonElement medicationPrescribed = rxNode.GetProperty("MedicationPrescribed");
            JsonElement medicationDispensed = rxNode.GetProperty("MedicationDispensed");

            var medicationQuantity = medicationDispensed.GetProperty("Quantity").ToString();
            // int quantity = Convert.ToInt32(medicationQuantity.Substring(0, medicationQuantity.IndexOf('.')));
            int quantity = Convert.ToInt32(Math.Round(Convert.ToDouble(medicationQuantity)));
           // Convert.ToInt32(medicationQuantity.Substring(0, medicationQuantity.IndexOf('.')));

            Medication medication = new Medication()
            {
                RxNumber = rxNode.GetProperty("RxNumber").ToString(),
                RxVendorRxID = rxNode.GetProperty("RxPioneerRxID").ToString(),
                RxDate = Convert.ToDateTime(rxNode.GetProperty("RxTransactionStatusChangedOnDateTimeUTC").ToString()).Date,
                DrugName = medicationDispensed.GetProperty("DrugName").ToString(),
                Direction = rxNode.GetProperty("DirectionsTranslatedEnglish").ToString(),
                Quantity = quantity,
                Supply = Convert.ToInt32(rxNode.GetProperty("DaysSupply").ToString()),
                PrescriberName = rxNode.GetProperty("WrittenByPrescriberPioneerRxID").ToString(),
                LastFillDate = Convert.ToDateTime(rxNode.GetProperty("DateFilledUTC").ToString()).Date,
                RfNumber = rxNode.GetProperty("RefillNumber").ToString(),
                RefillsRemaining = rxNode.GetProperty("RefillsRemaining").ToString(),
                NDCNumber = medicationPrescribed.GetProperty("NDC").ToString(),
                SBDCName = _cmrMedicationService.GetSBDCNameForNDCNumber(medicationPrescribed.GetProperty("NDC").ToString()).GetAwaiter().GetResult(),
                GenericName = _ndcApiService.GetRelatedNDCs(medicationPrescribed.GetProperty("NDC").ToString()).GetAwaiter().GetResult(),
            };

            return medication;

        }

        public Patient ParsePatient(JObject payload)
        {
            JsonDocument document = JsonDocument.Parse(payload.ToString());
            JsonElement root = document.RootElement;
            JsonElement bodyNode = root.GetProperty("Body");
            JsonElement patientNode = bodyNode.GetProperty("Patient");

            JsonElement Identification = patientNode.GetProperty("Identification");
            JsonElement Name = patientNode.GetProperty("Name");
            JsonElement Address = patientNode.GetProperty("Addresses").GetProperty("Address");
            JsonElement PhoneNumber = patientNode.GetProperty("PhoneNumbers").GetProperty("PhoneNumber");


            Patient patient = new Patient()
            {
                Address = new Address()
                {
                    AddressLineOne = Address[0].GetProperty("AddressLine").ToString(),
                    City = Address[0].GetProperty("City").ToString(),
                    State = Address[0].GetProperty("StateCode").ToString(),
                    ZipCode = Address[0].GetProperty("ZipCode").ToString()
                },
                PatientVendorRxID = Identification.GetProperty("PatientPioneerRxID").ToString(),
                Contact = new Contact()
                {
                    DoB = Convert.ToDateTime(patientNode.GetProperty("DateOfBirth").ToString()).Date,
                    FirstName = Name.GetProperty("FirstName").ToString(),
                    LastName = Name.GetProperty("LastName").ToString(),
                    PrimaryPhone = "+1"+PhoneNumber[0].GetProperty("Number").ToString(),
                    PrimaryEmail = patientNode.GetProperty("Email").ToString()
                },
                Status = "New Patient",
            };
            return patient;

        }

        public Pharmacy ParsePharmacy(JObject payload)
        {
            JsonDocument document = JsonDocument.Parse(payload.ToString());
            JsonElement root = document.RootElement;
            JsonElement body = root.GetProperty("Body");
            JsonElement pharmacyNode = body.GetProperty("Pharmacy");
            JsonElement employee = body.GetProperty("Employees").GetProperty("Employee");
          
            JsonElement Identification = pharmacyNode.GetProperty("Identification");
            JsonElement Address = pharmacyNode.GetProperty("Addresses").GetProperty("Address");
            JsonElement PhoneNumber = pharmacyNode.GetProperty("PhoneNumbers").GetProperty("PhoneNumber");

            Pharmacy pharmacy = new Pharmacy()
            {
                Name = pharmacyNode.GetProperty("PharmacyName").ToString(),
                Address = new Address()
                {
                    AddressLineOne = Address[0].GetProperty("AddressLine").ToString(),
                    City = Address[0].GetProperty("City").ToString(),
                    State = Address[0].GetProperty("StateCode").ToString(),
                    ZipCode = Address[0].GetProperty("ZipCode").ToString()
                },
                Contact = new Contact()
                {
                    PrimaryPhone = PhoneNumber[0].GetProperty("Number").ToString(),
                    Fax = PhoneNumber[1].GetProperty("Number").ToString(),
                },
                PharmacyVendorRxID = Identification.GetProperty("PharmacyPioneerRxID").ToString(),
                //PharmacyManager = pharmacyNode.GetProperty("PharmacistInChargePioneerRxID").ToString(),

                PharmacyManager = getName(employee),

                LastUpdate = DateTime.Now,
                NcpdpNumber = Identification.GetProperty("NCPDP").ToString(),
                NpiNumber = Identification.GetProperty("NPI").ToString(),

            };
            

            static string getName( JsonElement employee)
            {
                string pharmacyManager = "";
              
                for (int i = 0; i < 3; i++)
                {
                    var type = employee[i].GetProperty("EmployeeTypeText").ToString();
                    if (type== "Pharmacist")
                    {
                        string FirstName = employee[i].GetProperty("Name").GetProperty("FirstName").ToString();
                        string LastName = employee[i].GetProperty("Name").GetProperty("LastName").ToString();
                        pharmacyManager = FirstName + " " + LastName;
                      
                    }
                    else
                    {
                        continue;
                    }
                }
                return pharmacyManager;
            }

            return pharmacy;
        }
    }
}
