using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Service;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace OntrackDb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly IOntrackParser _ontrackParser;
        private readonly IWebHookService _webHookService;
        public WebhookController(IOntrackParser ontrackParser, IWebHookService webHookService)
        {
            _ontrackParser = ontrackParser;
            _webHookService = webHookService;
        }


        /// <summary>
        /// For Adding all data with json file. Role[Every onr can access]
        /// </summary>
        /// <remarks>
        /// This API Will be used for Adding all data with json file.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JObject payload)
        {
            Response<string> result = new Response<string>();
            ImportData importData = new ImportData();
            try
            {
                importData = await _webHookService.DumpImportData(payload, "In Progress");
                Pharmacy pharmacy = _ontrackParser.ParsePharmacy(payload);
                Patient patient = _ontrackParser.ParsePatient(payload);
                Medication medication = _ontrackParser.ParseMedication(payload);
                pharmacy.ImportData = importData;
                patient.ImportData = importData;
                medication.ImportData = importData;
                Pharmacy pharmacyDb = await _webHookService.AddOrUpdatePharmacy(pharmacy);
                patient.Pharmacy = pharmacyDb;
                List<Pharmacy> pharmacyList = new List<Pharmacy>();
                pharmacyList.Add(pharmacy);
                Doctor doctor = _ontrackParser.ParseDoctor(payload);
                doctor.ImportData = importData;
                Doctor doctorDb = await _webHookService.AddOrUpdateDoctor(doctor);
                Patient patientDb = await _webHookService.AddOrUpdatePatient(patient);
                medication.Patient = patientDb;
                medication.DoctorPrescribed = doctorDb;
                Medication medicationDb = await _webHookService.AddOrUpdateMedication(medication);
                MedicationConsumption medicationConsumption = await _webHookService.AddMedicationConsumption(medication);
                //var doctorPharmacyDb = await _webHookService.GetDoctorPharmacyByDoctorIdAndPharmacyId(doctorDb.Id, pharmacyDb.Id);
                //if (doctorPharmacyDb == null)
                //{
                //    DoctorPharmacy doctorPharmacy = new DoctorPharmacy();
                //    doctorPharmacy.Doctor = doctorDb;
                //    doctorPharmacy.Pharmacy = pharmacyDb;
                //    await _webHookService.AddDoctorPharmacy(doctorPharmacy);
                //}
                importData.status = "Completed";
                importData.message = "Data added Successfully";
                var importDataUp = await _webHookService.UpdateImportData(importData);
                result.Success = true;
                result.Message = "Data added Successfully";
            }
            catch(Exception ex)
            {
                importData.message = ex.Message;
                importData.status = "Failed";
                await _webHookService.UpdateImportData(importData);
                result.Success = false;
                result.Message = "Failed" + ex.Message;
            }
           
            return Ok(result);
        }


    }
}
