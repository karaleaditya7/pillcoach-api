using Aspose.Pdf.Operators;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using OntrackDb.Authentication;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.WebPages;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Net.Mime.MediaTypeNames;

namespace OntrackDb.Service
{
    public class PatientService: IPatientService
    {
        private readonly IPatientData _patientData;
        private readonly IImportWizardData _importWizardData;
        private readonly IAuditLogData _auditLogData;
        private readonly IDoctorData _doctorData;
        private readonly IAllergyData _allergyData;
        private readonly ISideEffectData _sideEffectData;
        private readonly ICmrMedicationData _cmrMedicationData;
        private readonly IAppointmentData _appointmentData;
        private readonly IPharmacyData _pharmacyData;
        private readonly INoteData _noteData;
        private readonly IMedicationData _medicationData;
        private readonly IMedicationToDoListData _medicationToDoListData;
        private readonly IMedicationConsumptionData _medicationConsumptionData;
        private readonly IMedicationService _medicationService;
        private readonly IAppointmentService _appointmentService;
        private readonly IServiceTakeawayInformationData _serviceTakeawayInformationData;
        private readonly IServiceTakeAwayMedReconciliationData _serviceTakeAwayMedReconciliationData;
        private readonly IAllergyService _allergyService;
        private readonly ICmrMedicationService _cmrMedicationService;
        private readonly ISideEffectService _sideEffectService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserData _userData;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IPrimaryThirdPartyData _primaryThirdPartyData;
        private readonly IPatientPdcData _patientPdcData;
        private readonly IPatientPdcService _patientPdcService;
        private readonly IPatientCallInfoData _patientCallInfoData;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly IReconciliationAllergyData _reconciliationAllergyData;
        private readonly IReconciliationSideEffectData _reconciliationSideEffectData;
        private readonly IReconciliationToDoListData _reconciliationToDoListData;
        private readonly IReconciliationData _reconciliationData;
        private readonly IDoctorMedicationData _doctorMedicationData;
        private readonly IPatientCallInfoService _patientCallInfoService;
        private readonly ApplicationDbContext _dbcontext;

        public PatientService(IPatientData patientData, DbContextOptions<ApplicationDbContext> dbContextOptions, IPatientPdcService patientPdcService, ICmrMedicationService cmrMedicationService, IImportWizardData importWizardData, IPatientCallInfoData patientCallInfoData,IPatientCallInfoService patientCallInfoService, IDoctorMedicationData doctorMedicationData, IReconciliationToDoListData reconciliationToDoListData , IReconciliationSideEffectData reconciliationSideEffectData , IReconciliationAllergyData reconciliationAllergyData , IReconciliationData reconciliationData , IOtcMedicationData otcMedicationData , IPatientPdcData patientPdcData, IAuditLogData auditLogData, IDoctorData doctorData, IAllergyService allergyService, ISideEffectService sideEffectService, ISideEffectData sideEffectData, IAllergyData allergyData, IPharmacyData pharmacyData, INoteData noteData, IMedicationData medicationData, IMedicationService medicationService, IMedicationConsumptionData medicationConsumptionData, IMedicationToDoListData medicationToDoListData, IAppointmentData appointmentData, IAppointmentService appointmentService, ICmrMedicationData cmrMedicationData, IServiceTakeawayInformationData serviceTakeawayInformationData, IServiceTakeAwayMedReconciliationData serviceTakeAwayMedReconciliationData, IHttpContextAccessor httpContextAccessor, IUserData userData, BlobServiceClient blobServiceClient,IPrimaryThirdPartyData primaryThirdPartyData)
        {
            _patientData = patientData;
            _pharmacyData = pharmacyData;
            _noteData = noteData;
            _medicationData = medicationData;
            _medicationService = medicationService;
            _medicationConsumptionData = medicationConsumptionData;
            _appointmentData = appointmentData;
            _appointmentService = appointmentService;
            _cmrMedicationData = cmrMedicationData;
            _serviceTakeawayInformationData = serviceTakeawayInformationData;
            _medicationToDoListData = medicationToDoListData;
            _allergyData = allergyData;
            _sideEffectData = sideEffectData;
            _allergyService = allergyService;
            _sideEffectService = sideEffectService;
            _doctorData = doctorData;
            _serviceTakeAwayMedReconciliationData = serviceTakeAwayMedReconciliationData;
            _auditLogData = auditLogData;
            _httpContextAccessor = httpContextAccessor;
            _userData = userData;
            _blobServiceClient = blobServiceClient;
            _primaryThirdPartyData = primaryThirdPartyData;
            _patientPdcData = patientPdcData;
            _otcMedicationData = otcMedicationData;
            _reconciliationData = reconciliationData;
            _reconciliationAllergyData = reconciliationAllergyData;
            _reconciliationSideEffectData = reconciliationSideEffectData;
            _reconciliationToDoListData = reconciliationToDoListData;
            _doctorMedicationData= doctorMedicationData;
            _patientCallInfoService = patientCallInfoService;
            _patientCallInfoData = patientCallInfoData;
            _importWizardData = importWizardData;
            _cmrMedicationService= cmrMedicationService;
            _patientPdcService = patientPdcService;
            _dbcontext = new ApplicationDbContext(dbContextOptions); 
        }

        public async Task<Response<Patient>> AddPatient(PatientModel model)
        {
            Response<Patient> response = new Response<Patient>();
            Note note = new Note(); 
            response.Success = false;
            if (string.IsNullOrEmpty(model.Status))
            {
                response.Message = "=Status is Missing";
                return response;
            }

            if (model.Language == null)
            {
                response.Message = "Language is Missing";
                return response;
            }

            if (model.Address == null)
            {
                response.Message = "Address Info is Missing";
                return response;
            }
            if (string.IsNullOrEmpty(model.PharmacyId.ToString()))
            {
                response.Message = "Pharmacy Info is Missing";
                return response;
            }

            Pharmacy pharmacy = await _pharmacyData.GetPharmacyById(model.PharmacyId);
            if (pharmacy == null)
            {
                response.Message = "Pharmacy Not Found";
                return response;
            }

            PrimaryThirdParty primaryThirdParty = await _primaryThirdPartyData.GetPlanById(model.PrimaryThirdPartyId);
            Patient patient = new Patient
            {
                Status = model.Status,
                Pharmacy = pharmacy,
                Address = model.Address,
                Contact =model.Contact,
                ImageName = model.ImageName,
                Note = note,
                primaryThirdParty = primaryThirdParty,
                Language = model.Language
            };
            var result = await _patientData.AddPatient(patient);

            if (result == null)
            {
                response.Message = "Error while creating patient";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = patient;
            response.Message = "Patient created successfully!";
            return response;
        }

        public async Task<Response<Patient>> DeletePatient(int patientId)
        {

            Response<Patient> response = new Response<Patient>();
            Patient patient = await _patientData.GetPatientById(patientId);
            
            if (patient == null)
            {
                response.Message = "Patient not found";
                response.Success = false;
                return response;
            }
            var patients = new List<Patient>();
            if (patient.Pharmacy != null)
            {
                patients = await _pharmacyData.GetPatientByPharmacyId(patient.Pharmacy.Id);
            }
            var pharmacy = await _pharmacyData.GetPharmacybyPatientById(patient.Id);
            //List<Medication> medications = await _medicationData.GetMedicationByPatientId(patient.Id);
            //if(medications != null)
            //{
            //    foreach (Medication medication in medications)
            //    {
            //        var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByMedId(medication.Id);
            //        if (doctorMedications.Count() != 0)
            //        {
            //            //foreach (DoctorMedication doctorMedication2 in doctorMedications)
            //            //{
            //            //     _doctorMedicationData.DeleteDoctorMedicationForMedication(doctorMedication2);
            //            //}
            //            _dbcontext.DoctorMedications.RemoveRange(doctorMedications);
            //        }
            //         //_medicationData.DeleteMedication(medication);                    
            //    }
            //    _dbcontext.Medications.RemoveRange(medications);
            //}
            await _medicationData.DeleteMedicationHardCore(patient.Id);
            var patientPDCs = await _patientPdcData.GetPatientPDCByPatientId(patient.Id);

                if(patientPDCs.Count() != 0)
                {       
                    //foreach(PatientPDC patientPDC in patientPDCs)
                    //{
                    // _patientPdcData.DeletePatientPDCForPatient(patientPDC);
                    //}
                    _dbcontext.PatientPDCs.RemoveRange(patientPDCs);
                }

            var importWizardSourceFile = await _importWizardData.GetDraftSourceFileByPharmacyIdAsync(patient.Pharmacy.Id);

            if (importWizardSourceFile.Count() != 0)
            { 
                //foreach(ImportSourceFile importSourceFile in importWizardSourceFile)
                //{
                //     _importWizardData.DeleteDraftSourceFileAsync(importSourceFile);
                //}
                _dbcontext.ImportSourceFiles.RemoveRange(importWizardSourceFile);
                
            }


            var patientCallInfos = await _patientCallInfoData.GetPatientCallListAsync(patient.Id);

            if (patientCallInfos.Count() != 0)
            {
                //foreach (PatientCallInfo patientCallInfo in patientCallInfos)
                //{
                //     _patientCallInfoData.DeletePatientCallInfoAsync(patientCallInfo);
                //}
                _dbcontext.PatientCallInfo.RemoveRange(patientCallInfos);
            }



            List<MedicationConsumption> medicationConsumptions = await _medicationConsumptionData.GetAllMedicationConsumptionsDataByPatientId(patient.Id);
            if (medicationConsumptions.Count() != 0)
            {
                //foreach (MedicationConsumption medicationConsumption in medicationConsumptions)
                //{

                //    _medicationConsumptionData.DeleteMedicationConsumption(medicationConsumption);

                //}
                _dbcontext.medicationConsumptions.RemoveRange(medicationConsumptions);
            }

            var serviceTakeawayInformation = await _serviceTakeawayInformationData.GetServiceTakeawayInformationByPatientId(patient.Id);
           if(serviceTakeawayInformation != null)
            {
                // _serviceTakeawayInformationData.DeleteServiceTakeawayInformationForPatient(serviceTakeawayInformation);
                _dbcontext.ServiceTakeawayInformations.RemoveRange(serviceTakeawayInformation);
            }

            var serviceTakeawayMedRec = await _serviceTakeAwayMedReconciliationData.GetServiceTakeAwayMedReconciliationByPatientId(patient.Id);
            if (serviceTakeawayMedRec != null)
            {
                // _serviceTakeAwayMedReconciliationData.DeleteServiceTakeawayMedRecForPatient(serviceTakeawayMedRec);
                _dbcontext.ServiceTakeAwayMedReconciliations.RemoveRange(serviceTakeawayMedRec);
            }


            List<MedicationToDoRelated> medicationToDoRelateds = await _medicationToDoListData.getMedicationToDoRelatedsbyPatientId(patient.Id);
            if (medicationToDoRelateds.Count() != 0)
            {
                //foreach (MedicationToDoRelated medicationToDoRelated in medicationToDoRelateds)
                //{
                //     _medicationToDoListData.PatientDeleteMedicationToDoRelated(medicationToDoRelated);
                //}
                _dbcontext.MedicationToDoRelateds.RemoveRange(medicationToDoRelateds);
            }

            List<NonRelatedMedicationToDo> nonMedicationToDoRelateds = await _medicationToDoListData.getNonMedicationToDoRelatedsbyPatientId(patient.Id);
            if (nonMedicationToDoRelateds.Count() != 0)
            {
                //foreach (NonRelatedMedicationToDo nonRelatedMedicationToDo in nonMedicationToDoRelateds)
                //{
                //     _medicationToDoListData.PatientDeleteNonMedicationToDoRelated(nonRelatedMedicationToDo);
                //}
                _dbcontext.NonRelatedMedicationToDos.RemoveRange(nonMedicationToDoRelateds);
            }

            List<Allergy> allergies = await _allergyData.GetAllAllergyByPatientId(patient.Id);
            if(allergies.Count() != 0)
            {

                //foreach (Allergy allergy in allergies)
                //{
                //    _allergyData.PatientDeleteForAllergy(allergy);
                //}
                _dbcontext.Allergies.RemoveRange(allergies);
            }

            List<SideEffect> sideEffects = await _sideEffectData.GetAllSideEffectByPatientId(patient.Id);
            if (sideEffects.Count() != 0)
            {

                //foreach (SideEffect sideEffect in sideEffects)
                //{
                //     _sideEffectData.PatientDeleteForSideEffect(sideEffect);
                //}
                _dbcontext.SideEffects.RemoveRange(sideEffects);
            }

            List<CmrMedication> cmrMedications = await _cmrMedicationData.GetUniqueCmrMedicationsByPatientId(patient.Id);
            if (cmrMedications.Count() != 0)
            {
                foreach (CmrMedication cmrMedication in cmrMedications)
                {
                    var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByCmrId(cmrMedication.Id);
                    if (doctorMedications.Count() != 0)
                    {
                        //foreach (DoctorMedication doctorMedication2 in doctorMedications)
                        //{
                        //    _doctorMedicationData.DeleteDoctorMedicationForCmr(doctorMedication2);
                        //}
                        _dbcontext.DoctorMedications.RemoveRange(doctorMedications);
                    }
                //     _cmrMedicationData.DeleteCmrMedicationForPatient(cmrMedication);
                }
                _dbcontext.CmrMedications.RemoveRange(cmrMedications);
            }
            List <Appointment> appointments =await _appointmentData.GetAppointmentsByPatientId(patient.Id);
           if(appointments.Count() != 0)
            {
                //foreach (Appointment appointment in appointments)
                //{
                //    await _appointmentService.DeleteAppointmentByIdForPatientDelete(appointment.ID);
                //}
                _dbcontext.Appointments.RemoveRange(appointments);
            }

           List <AuditLog> auditLogs = await  _auditLogData.GetAuditLogsBypatientId(patient.Id);
            if(auditLogs.Count() != 0)
            {
                //foreach (AuditLog auditLog in auditLogs)
                //{
                //     _auditLogData.DeleteAuditLogForPatient(auditLog);
                //}
                _dbcontext.AuditLogs.RemoveRange(auditLogs);
            }
            List<CmrVaccine> cmrVaccines = await _serviceTakeawayInformationData.GetCmrVaccineByPatientId(patient.Id);
            if (cmrVaccines.Count() != 0)
            {
                //foreach (CmrVaccine cmrVaccine in cmrVaccines)
                //{
                //     _serviceTakeawayInformationData.PatientDeleteForCmrVaccine(cmrVaccine);
                //}
                _dbcontext.CmrVaccines.RemoveRange(cmrVaccines);
            }

            List<VaccineReconciliation> vaccineReconciliations = await _serviceTakeAwayMedReconciliationData.GetMedRecVaccineReconciliationByPatientId(patient.Id);
            if (vaccineReconciliations.Count() != 0)
            {
                //foreach (VaccineReconciliation vaccineReconciliation in vaccineReconciliations)
                //{
                //     _serviceTakeAwayMedReconciliationData.PatientDeleteForMedRecVaccineReconciliation(vaccineReconciliation);
                //}
                _dbcontext.VaccineReconciliations.RemoveRange(vaccineReconciliations);  
            }

            List<OtcMedication> otcMedications = await _otcMedicationData.GetAllOtcMedicationsByPatientId(patient.Id);
            if (otcMedications.Count() != 0)
            {
                foreach (OtcMedication otcMedication in otcMedications)
                {
                    var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByOtcMedId(otcMedication.Id);
                    if (doctorMedications.Count() != 0)
                    {
                        //foreach (DoctorMedication doctorMedication2 in doctorMedications)
                        //{
                        //    _doctorMedicationData.DeleteDoctorMedicationForOtcMedication(doctorMedication2);
                        //}
                        _dbcontext.DoctorMedications.RemoveRange(doctorMedications);
                    }
                // _otcMedicationData.PatientDeleteForOtcMedication(otcMedication);
                }
                _dbcontext.OtcMedications.RemoveRange(otcMedications);
            }

            List<ReconciliationAllergy> reconciliationAllergies = await _reconciliationAllergyData.GetReconciliationAllergyByPatientId(patient.Id);
            if (reconciliationAllergies.Count() != 0)
            {
                //foreach (ReconciliationAllergy reconciliationAllergy in reconciliationAllergies)
                //{
                //     _reconciliationAllergyData.PatientDeleteForReconciliationAllergy(reconciliationAllergy);
                //}
                _dbcontext.ReconciliationAllergies.RemoveRange(reconciliationAllergies);
            }

            List<ReconciliationSideeffect> reconciliationSideeffects = await _reconciliationSideEffectData.GetMedRecSideEffectByPatientId(patient.Id);
            if (reconciliationSideeffects.Count() != 0)
            {
                //foreach (ReconciliationSideeffect reconciliationSideeffect in reconciliationSideeffects)
                //{
                //     _reconciliationSideEffectData.DeleteMedRecSideEffectByServiceTakeMedRecId(reconciliationSideeffect);
                //}
                _dbcontext.ReconciliationSideeffects.RemoveRange(reconciliationSideeffects);
            }

            List<ReconciliationToDoRelated> reconciliationToDoRelateds = await _reconciliationToDoListData.GetReconciliationToDoRelatedByPatientId(patient.Id);
            if (reconciliationToDoRelateds.Count() != 0)
            {
                //foreach (ReconciliationToDoRelated reconciliationToDoRelated in reconciliationToDoRelateds)
                //{
                //    _reconciliationToDoListData.PatientDeleteForReconciliationToDoRelated(reconciliationToDoRelated);
                //}
                _dbcontext.ReconciliationToDoRelateds.RemoveRange(reconciliationToDoRelateds);
            }

            List<NonRelatedRecocilationToDo> nonRelatedRecocilationToDos = await _reconciliationToDoListData.GetNonRelatedRecocilationToDoRelatedByPatientId(patient.Id);
            if (nonRelatedRecocilationToDos.Count() != 0)
            {
                //foreach (NonRelatedRecocilationToDo nonRelatedRecocilationToDo in nonRelatedRecocilationToDos)
                //{
                //     _reconciliationToDoListData.PatientDeleteForNonRelatedRecocilationToDo(nonRelatedRecocilationToDo);
                //}
                _dbcontext.NonRelatedRecocilationToDos.RemoveRange(nonRelatedRecocilationToDos);
            }

            List<MedicationReconciliation> medicationReconciliations = await _reconciliationData.GetMedReconciliationByPatientId(patient.Id);
            if (medicationReconciliations.Count() != 0)
            {
                foreach (MedicationReconciliation medicationReconciliation in medicationReconciliations)
                {
                    var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByMedRecId(medicationReconciliation.Id);
                    //if (doctorMedications != null)
                    //{
                        //foreach (DoctorMedication doctorMedication2 in doctorMedications)
                        //{
                        //    _doctorMedicationData.DeleteDoctorMedicationForMedRec(doctorMedication2);
                        //}
                        _dbcontext.DoctorMedications.RemoveRange(doctorMedications);
                }
                // _reconciliationData.PatientDeleteForMedicationReconciliation(medicationReconciliation);
                //}
                _dbcontext.MedicationReconciliations.RemoveRange(medicationReconciliations);
            }

            //if (appointments != null)
            //{
            //    //foreach (Appointment appointment in appointments)
            //    //{
            //    //    await _appointmentService.DeleteAppointmentByIdForPatientDelete(appointment.ID);
            //    //}
            //    _dbcontext.Appointments.RemoveRange(appointments);
            //}

            
            await _patientData.DeleteContactById(patient.Contact.Id);
            await _patientData.DeleteAddressById(patient.Address.Id);
            await _patientData.DeleteNotesById(patient.Note.Id);
            await _patientData.DeleteMailListById(patient.Id);
            await  _patientData.DeletePatient(patient);

            var blobContainerCmr = _blobServiceClient.GetBlobContainerClient("cmrpdf");
            string pdfNameCmr = String.Format("{0}.pdf", patient.Id);
            var blobClientCmr = blobContainerCmr.GetBlobClient(pdfNameCmr);
            await blobClientCmr.DeleteIfExistsAsync();

            var blobContainerMedRec = _blobServiceClient.GetBlobContainerClient("medrecpdf");
            string pdfNameMedRec = String.Format("{0}.pdf", patient.Id);
            var blobClientMedRec = blobContainerMedRec.GetBlobClient(pdfNameMedRec);
            await blobClientMedRec.DeleteIfExistsAsync();

            if (patients != null)
            {
                if (patients.Count == 1)
                {
                    await _pharmacyData.DeletePharmacyHardCore(pharmacy);
                }
            }
            response.Message = "Patient Deleted successfully";
            response.Success = true;
            return response;
        }

        public async Task<Response<Patient>> GetPatientByContactNumber(string contactNumber)
        {
            Response<Patient> response = new Response<Patient>();
            Patient patient = await _patientData.GetPatientByContactNumber(contactNumber);
            if (patient == null)
            {
                response.Message = "Patient not found";
                response.Success = false;
                return response;
            }
            response.Data = patient;
            response.Success = true;
            return response;
        }
        

        public async Task<Response<Patient>> DeletePatientForPharmacy(int patientId)
        {
            Response<Patient> response = new Response<Patient>();
            Patient patient = await _patientData.GetPatientById(patientId);
            if (patient == null)
            {
                response.Message = "Patient not found";
                response.Success = false;
                return response;
            }

            try
            {
                List<MedicationReconciliation> medicationReconciliations = await _reconciliationData.GetMedReconciliationByPatientId(patient.Id);

                _dbcontext.MedicationReconciliations.RemoveRange(medicationReconciliations);


                List<MedicationToDoRelated> medicationToDoRelateds = await _medicationToDoListData.getMedicationToDoRelatedsbyPatientId(patient.Id);
                _dbcontext.MedicationToDoRelateds.RemoveRange(medicationToDoRelateds);


                List<OtcMedication> otcMedications = await _otcMedicationData.GetAllOtcMedicationsByPatientId(patient.Id);
                _dbcontext.OtcMedications.RemoveRange(otcMedications);


                List<ReconciliationToDoRelated> reconciliationToDoRelateds = await _reconciliationToDoListData.GetReconciliationToDoRelatedByPatientId(patient.Id);
                _dbcontext.ReconciliationToDoRelateds.RemoveRange(reconciliationToDoRelateds);

                //List<Medication> medications = await _medicationData.GetMedicationByPatientId(patient.Id);
                //_dbcontext.Medications.RemoveRange(medications);

                var blobContainerCmr = _blobServiceClient.GetBlobContainerClient("cmrpdf");
                string pdfNameCmr = String.Format("{0}.pdf", patient.Id);
                var blobClientCmr = blobContainerCmr.GetBlobClient(pdfNameCmr);
                await blobClientCmr.DeleteIfExistsAsync();

                var blobContainerMedRec = _blobServiceClient.GetBlobContainerClient("medrecpdf");
                string pdfNameMedRec = String.Format("{0}.pdf", patient.Id);
                var blobClientMedRec = blobContainerMedRec.GetBlobClient(pdfNameMedRec);
                await blobClientMedRec.DeleteIfExistsAsync();

                await _medicationData.DeleteMedicationHardCore(patient.Id);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception here------>"+ex.Message);
            }
            

            Console.WriteLine(patient.Id);
            await _patientData.DeleteContactById(patient.Contact.Id);
            await _patientData.DeleteAddressById(patient.Address.Id);
            await _patientData.DeleteNotesById(patient.Note.Id);
            await _patientData.DeleteMedicationConsumptionsById(patient.Id);
            await _patientData.DeleteMailListById(patient.Id);
            response.Message = "Patient Deleted successfully";
            response.Success = true;
            return response;
        }


        public async Task<Response<Patient>> GetPatients(int recordNumber ,int pageLimit ,DateTime startDate,DateTime endDate,int month,string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
          
            Response<Patient> response = new Response<Patient>();
            var patients = await _patientData.GetPatients(recordNumber, pageLimit,keywords,sortDirection,filterType,filterValue,filterCategory);
           
            foreach (Patient patient in patients) 
            {
                Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


                patient.CholestrolPDC = cholesterol;
                patient.RASAPDC = rasa;
                patient.DiabetesPDC = diabetes;
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
                                patients = patients.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                patients = patients.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC <= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                patients = patients.Where(p => p.DiabetesPDC >= filterMinValueD && p.DiabetesPDC <= filterMaxValueD).ToList();
                                break;
                        }
                        break;
                }
            }

            response.Success = true;
            response.DataList = patients;
            return response;
        }

        public async Task<Response<Patient>> UpdatePatient(EditPatientModel model)
        {
            Response<Patient> response = new Response<Patient>();
            Patient patient = await _patientData.GetPatientById(model.Id);
            if(patient == null)
            {
                response.Success = false;
                response.Message = "Patient Not Found";
                return response;
            }           
            var contact = await _patientData.GetContactById(model.Contact.Id);
            contact.FirstName = model.Contact.FirstName;
            contact.LastName = model.Contact.LastName;
            contact.PrimaryPhone = model.Contact.PrimaryPhone;
            contact.SecondaryPhone = model.Contact.SecondaryPhone;
            contact.PrimaryEmail = model.Contact.PrimaryEmail;
            contact.SecondaryEmail = model.Contact.SecondaryEmail;   
            contact.DoB=model.Contact.DoB;
            patient.Contact = contact;
            var address = await _patientData.GetAddressById(model.Address.Id);
            address.AddressLineOne = model.Address.AddressLineOne;
            address.AddressLineTwo = model.Address.AddressLineTwo;
            address.City = model.Address.City;
            address.State = model.Address.State;
            address.ZipCode = model.Address.ZipCode;
            patient.Address = address;
            patient.Language = model.Language;
            PrimaryThirdParty primaryThirdParty = await _primaryThirdPartyData.GetPlanById(model.PrimaryThirdPartyId);
            if(primaryThirdParty != null)
            {
                patient.primaryThirdParty = primaryThirdParty;
            }
            else
            {
                patient.primaryThirdParty = null;
            }
            var result = await _patientData.UpdatePatient(patient);
            var cmrPatient = await _serviceTakeawayInformationData.GetCmrPatientById(patient.Id);
            if (cmrPatient != null)
            {
                var cmrContact = await _patientData.GetContactById(cmrPatient.Contact.Id);
                cmrContact.FirstName = model.Contact.FirstName;
                cmrContact.LastName = model.Contact.LastName;
                cmrContact.PrimaryPhone = model.Contact.PrimaryPhone;
                cmrContact.SecondaryPhone = model.Contact.SecondaryPhone;
                cmrContact.PrimaryEmail = model.Contact.PrimaryEmail;
                cmrContact.SecondaryEmail = model.Contact.SecondaryEmail;
                cmrPatient.Contact = contact;
                var cmrAddress = await _patientData.GetAddressById(cmrPatient.Address.Id);
                cmrAddress.AddressLineOne = model.Address.AddressLineOne;
                cmrAddress.AddressLineTwo = model.Address.AddressLineTwo;
                cmrAddress.City = model.Address.City;
                cmrAddress.State = model.Address.State;
                cmrAddress.ZipCode = model.Address.ZipCode;
                cmrPatient.Address = address;
                await _serviceTakeawayInformationData.UpdateCmrPatient(cmrPatient);
            }
            
            response.Success = true;
            response.Message = "Patient Updated successfully!";
            return response;
        }


        public async Task<Response<Doctor>> UpdateDoctor(DoctorModel model)
        {
            Response<Doctor> response = new Response<Doctor>();
            Doctor doctor = await _doctorData.GetDoctorById(model.Id);
            var contact = await _patientData.GetDoctorContact(doctor);

            contact.PrimaryPhone = model.PhoneNumber;
            contact.Fax = model.FaxNumber;
            doctor.Contact = contact;
            var result = await _doctorData.UpdateContact(contact);

            response.Success = true;
            response.Message = "Doctor Updated successfully!";
            return response;
        }


        public async Task<Response<Patient>> GetPatientById(int recordNumber, int pageLimit, int id,DateTime startDate,DateTime endDate,int month)
        {
            Response<Patient> response = new Response<Patient>();
            var patient = await  _patientData.GetPatientBypatientId(id);
         
           
            if (patient == null || patient.IsDeleted)
            {
                response.Message = "Patient Not Found";
                response.Success = false;
                return response;
            }

            if (patient.Note != null && !string.IsNullOrWhiteSpace(patient.Note.UserId))
            {
                var user = await _userData.GetUserById(patient.Note.UserId);

                if (user != null) patient.Note.LastUpdatedBy = $"{user.FirstName} {user.LastName}".Trim();
            }

            // return false for consent fields, if they are null
            patient.Contact.ConsentForCall ??= false;
            patient.Contact.ConsentForText ??= false;
            patient.Contact.ConsentForEmail ??= false;

            Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
            Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
            Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


            patient.CholestrolPDC = cholesterol;
            patient.RASAPDC = rasa;
            patient.DiabetesPDC = diabetes;

            patient.RASAExclusion = await _medicationService.GetExlusionMedication(patient.Id,"RASA");
            patient.DiabetesExclusion = await _medicationService.GetExlusionMedication(patient.Id, "Diabetes");
            response.Success = true;
            response.Message = "Patient retrived successfully";
            response.Data = patient;
            return response;
        }

        public async Task<Response<Medication>> GetPrescribedMedicationsBypatientId(int recordNumber, int pageLimit, int id)
        {
            Response<Medication> response = new();
            var patient = await _patientData.GetPatientById(id);
            if (recordNumber >= 0 && pageLimit != 0)
            {
                patient.Medications = patient.Medications
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.LastFillDate)
                    .Skip(recordNumber)
                    .Take(pageLimit)
                    .Select(m => new Medication
                    {
                        Id = m.Id,
                        SBDCName = m.SBDCName, // SBDC name will be already updated during import, so we do not need to call RxNav at this point
                        RxNumber = m.RxNumber,
                        RxVendorRxID = m.RxVendorRxID,
                        RxDate = m.RxDate,
                        DrugName = m.DrugName,
                        Direction = m.Direction,
                        Quantity = m.Quantity,
                        Supply = m.Supply,
                        PrescriberName = m.PrescriberName,
                        LastFillDate = m.LastFillDate,
                        NextFillDate = m.NextFillDate,
                        RfNumber = m.RfNumber,
                        RefillsRemaining = m.RefillsRemaining,
                        NDCNumber = m.NDCNumber,
                        DrugSubGroup = m.DrugSubGroup,
                        GenericName = m.GenericName,
                        Condition = m.Condition,
                        OptionalCondition = m.OptionalCondition,
                        DoctorPrescribed = m.DoctorPrescribed,
                        IsActive = m.IsActive,
                        InUse = m.InUse,
                        RefillDue = m.RefillDue,
                        PrimaryThirdParty = m.PrimaryThirdParty
                    })
                    .ToList();
            }
            else
            {
                patient.Medications = patient.Medications
                    .Where(m => m.IsActive)
                    .OrderByDescending(m => m.LastFillDate)
                    .Select(m => new Medication
                    {
                        Id = m.Id,
                        SBDCName = m.SBDCName, // SBDC name will be already updated during import, so we do not need to call RxNav at this point
                        RxNumber = m.RxNumber,
                        RxVendorRxID = m.RxVendorRxID,
                        RxDate = m.RxDate,
                        DrugName = m.DrugName,
                        Direction = m.Direction,
                        Quantity = m.Quantity,
                        Supply = m.Supply,
                        PrescriberName = m.PrescriberName,
                        LastFillDate = m.LastFillDate,
                        NextFillDate = m.NextFillDate,
                        RfNumber = m.RfNumber,
                        RefillsRemaining = m.RefillsRemaining,
                        NDCNumber = m.NDCNumber,
                        DrugSubGroup = m.DrugSubGroup,
                        Condition = m.Condition,
                        OptionalCondition = m.OptionalCondition,
                        DoctorPrescribed = m.DoctorPrescribed,
                        IsActive = m.IsActive,
                        InUse = m.InUse,
                        RefillDue = m.RefillDue
                    })
                    .ToList();
            }

            response.Success = true;
            response.Message = "PrescribedMedications retrived Successfully";
            response.DataList = patient.Medications.ToList();
            return response;
        }

        public async Task<Response<Medication>> GetPrescribedMedicationsBypatientId_old(int recordNumber, int pageLimit, int id)
        {
            Response<Medication> response = new Response<Medication>();
            var patient = await _patientData.GetPatientById(id);
            if (recordNumber >= 0 && pageLimit != 0)
            {
                patient.Medications = patient.Medications.Select( m => new Medication
                    {
                        Id= m.Id,
                        SBDCName = m.SBDCName, // SBDC name will be already updated during import, so we do not need to call RxNav at this point
                        RxNumber =m.RxNumber,
                        RxVendorRxID =m.RxVendorRxID,
                        RxDate = m.RxDate,
                        DrugName = m.DrugName,
                        Direction =m.Direction,
                        Quantity = m.Quantity,
                        Supply = m.Supply,
                        PrescriberName = m.PrescriberName,
                        LastFillDate = m.LastFillDate,
                        NextFillDate = m.NextFillDate,
                        RfNumber = m.RfNumber,
                        RefillsRemaining =m.RefillsRemaining,
                        NDCNumber = m.NDCNumber,
                        DrugSubGroup = m.DrugSubGroup,
                        GenericName = m.GenericName,
                        Condition= m.Condition,
                        OptionalCondition= m.OptionalCondition, 
                        DoctorPrescribed= m.DoctorPrescribed,
                        IsActive= m.IsActive,
                }).GroupBy(m => m.SBDCName ?? m.GenericName ?? m.DrugName).Select(m => m.OrderByDescending(m => m.LastFillDate).FirstOrDefault()).Skip(recordNumber).Take(pageLimit).ToList();
            }
            else
            {
                patient.Medications = patient.Medications.Select( m => new Medication
                {       
                        Id = m.Id,
                        SBDCName = m.SBDCName, // SBDC name will be already updated during import, so we do not need to call RxNav at this point
                        RxNumber = m.RxNumber,
                        RxVendorRxID = m.RxVendorRxID,
                        RxDate = m.RxDate,
                        DrugName = m.DrugName,
                        Direction = m.Direction,
                        Quantity = m.Quantity,
                        Supply = m.Supply,
                        PrescriberName = m.PrescriberName,
                        LastFillDate = m.LastFillDate,
                        NextFillDate = m.NextFillDate,
                        RfNumber = m.RfNumber,
                        RefillsRemaining = m.RefillsRemaining,
                        NDCNumber = m.NDCNumber,
                        DrugSubGroup = m.DrugSubGroup,
                        Condition = m.Condition,
                        OptionalCondition = m.OptionalCondition,
                    DoctorPrescribed = m.DoctorPrescribed,
                }).GroupBy(m => m.SBDCName ?? m.GenericName ?? m.DrugName).Select(m => m.OrderByDescending(m => m.LastFillDate).FirstOrDefault()).ToList();
            }

            response.Success = true;
            response.Message = "PrescribedMedications retrived Successfully";
            response.DataList = patient.Medications.ToList();
            return response;
        }


        public async Task<Response<PatientDto>> GetPatientByIdForPDCForDto(int id, DateTime startDate, DateTime endDate, int month)
        {
            Response<PatientDto> response = new Response<PatientDto>();
            var patient = await _patientData.GetPatientByIdForPDCWithDto(id);
            if (patient == null || patient.IsDeleted)
            {
                response.Message = "Patient Not Found";
                response.Success = false;
                return response;
            }

           Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
           Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
           Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


            patient.CholestrolPDC = cholesterol;
            patient.RASAPDC =rasa;
            patient.DiabetesPDC = diabetes;

            response.Success = true;
            response.Message = "Patient retrived successfully";
            response.Data = patient;
            return response;
        }

        public async Task<Response<Patient>> GetPatientByIdGraph(int id, DateTime startDate, DateTime endDate)
        {
            Response<Patient> response = new Response<Patient>();
            var patient = await _patientData.GetPatientById(id);
            patient.Pharmacy = await _pharmacyData.GetPharmacyWithContactByPharmacyId(patient.Pharmacy.Id);

            if (patient == null || patient.IsDeleted)
            {
                response.Message = "Patient Not Found";
                response.Success = false;
                return response;
            }

            patient.CholestrolPDC = await _medicationService.CalculatePdcForGraph(patient, PDC.Cholesterol.ToString(), startDate, endDate);
            patient.RASAPDC = await _medicationService.CalculatePdcForGraph(patient, PDC.RASA.ToString(), startDate, endDate);
            patient.DiabetesPDC = await _medicationService.CalculatePdcForGraph(patient, PDC.Diabetes.ToString(), startDate, endDate);
            List<Medication> medicationList = new List<Medication>();
            foreach (Medication medication in patient.Medications)
            {
                List<Doctor> doctor = await _pharmacyData.GetDoctorPharmacyByPharmacyId(patient.Pharmacy.Id);
                medication.Doctor = doctor;
                medication.Patient = patient;
                medicationList.Add(medication);
            }
            patient.Medications = medicationList;
            response.Success = true;
            response.Message = "Patient retrived successfully";
            response.Data = patient;
            return response;
        }

        public async Task<Response<Medication>> GetMedicationByPatientId(int patientId)
        {
            Response<Medication> response = new Response<Medication>();
            var medications =  await _medicationData.GetMedicationByPatientId(patientId);
            response.Success = true;
            response.DataList = medications;
            return response;
        }
        public async Task<Response<List<Patient>>> GetPatientsByPharmacyId(int pharmacyId)
        {
            Response<List<Patient>> response = new Response<List<Patient>>();
            var patients = await _patientData.GetPatientsByPharmacyId(pharmacyId);

            if (patients == null)
            {
                response.Message = "Error while getting patients by pharmacy Id";
                response.Success = false;
                return response;
            }
            else
            {
                 response.Success = true;
                response .Message = "Patients retrived successfully";
                response.Data = patients;
                return response;
            }
        }

        public async Task<Response<Note>> AddPatientNote(NoteModel noteModel, int patientId)
        {
            Response<Note> response = new Response<Note>();
            Patient patient = await _patientData.GetPatientById(patientId);
            if (patient == null)
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
            var result = await _noteData.AddPatientNote(note);
            if (result == null)
            {
                response.Message = "Error while creating PatientNote";
                response.Success = false;
                return response;
            }
            patient.Note = note;
            var resultUp = _patientData.UpdatePatient(patient);
            if (resultUp == null)
            {
                response.Message = "Error while creating PatientNote";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = note;
            response.Message = "PatientNote created successfully!";
            return response;
        }
        public async Task<Response<Note>> GetPatientNoteById(int patientId)
        {
            Response<Note> response = new Response<Note>();
            Patient patient = await _patientData.GetPatientWithNoteById(patientId);
            patient.Pharmacy = await _pharmacyData.GetPharmacyWithNoteByPharmacyId(patient.Pharmacy.Id);


            if (patient == null)
            {
                response.Message = "Error while getting PatientNote by id";
                response.Success = false;
                return response;
            }
            response.Success = true;
            response.Message = "PatientNote retrived successfully";
            response.Data = patient.Note;
            return response;

        }
        public async Task<Response<Note>> UpdateNote(NoteModel note)
        {
            Response<Note> response = new Response<Note>();
            Note noteDb = await _noteData.GetNoteById(note.Id);

            if (noteDb == null)
            {
                response.Message = "Error while getting Note by id";
                response.Success = false;
                return response;
            }

            var userId = _httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

            noteDb.text = note.Text;
            noteDb.LastUpdated = DateTime.Now;
            noteDb.UserId = userId;

            var result = await _noteData.UpdateNote(noteDb);
            response.Success = true;
            response.Message = "Note Updated Successfully";
            response.Data = result;
            return response;

        }
        public async Task<Response<Note>> UpdatePatientNote(NoteModel noteModel)
        {
            Response<Note> response = new Response<Note>();
            Note noteDb = await _noteData.GetPatientNoteById(noteModel.Id);
            if (noteDb == null)
            {
                response.Message = "Patient Note not found";
                response.Success = false;
                return response;
            }

            var userId = _httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

            noteDb.text = noteModel.Text;
            noteDb.LastUpdated = new DateTime();
            noteDb.UserId = userId;
            
            var result = await _patientData.UpdatePatientNote(noteDb);

            response.Success = true;
            response.Data = noteDb;
            response.Message = "PatientNote created successfully!";
            return response;

        }

        public int countPatientByCondition(List<Patient> patients, string condition) {
            int count = 0;
            for (int i = 0; i < patients.Count; i++) {
                for (int j = 0; j < patients[i].Medications.Count; j++)
                {
                    if (patients[i].Medications[j].Condition == condition) {
                        count++;
                        break;
                    }
                }
               
            }
            return count;
        }

        public async Task<Response<Medication>> UpdateMedicationByCondition(int medicationId, MedicationModel model)
        {
            Response<Medication> response = new Response<Medication>();
            Medication medication = await _medicationData.GetMedicationById(medicationId);
          
            if (medication != null)
            {
              var medicationCondition = await _medicationData.GetMedicationConditionByName(model.Condition);
                    if(medicationCondition == null) 
                    {
                        MedicationCondition medicationCondition1 = new MedicationCondition();

                        medicationCondition1.Name = model.Condition;

                        var medCondition = await _medicationData.AddMedicationCondition(medicationCondition1);
                        medication.OptionalCondition = model.Condition;
                    }
                    else
                    {
                        medication.OptionalCondition = medicationCondition.Name;
                    }

                var result = await _medicationData.UpdateMedication(medication);  
                

                response.Success = true;
                response.Message = "Medication Update Successfully";
                return response;
            }
            response.Success = false;
            response.Message = "Medication Not Found";
            return response;
        }

        public async Task<Response<Medication>> GetMedicationByCondition(string condition)
        {
            Response<Medication> response = new Response<Medication>();
            var medications = await _medicationData.GetMedicationByCondition(condition);
            response.Success = true;
            response.DataList = medications;
            return response;
        }

        public async Task<Response<Patient>> UpdatePatientStatus(PatientModel patientModel)
        {
            Response<Patient> response = new Response<Patient>();
            Patient patient = await _patientData.GetPatientById(patientModel.Id);
            if (patient == null)
            {
                response.Message = "Patient not found";
                response.Success = false;
                return response;
            }
            patient.Status = patientModel.Status;

            var result = await _patientData.UpdatePatient(patient);

            response.Success = true;
            response.Data = patient;
            response.Message = "Patient status updated successfully";
            return response;
        }

        public async Task<List<Patient>> GetPharmacyPatientsByCondition(int recordNumber, int pageLimit,DateTime startDate,DateTime endDate, int pharmacyId, string condition,int month,string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            //DateTime Today = DateTime.Today;
            //DateTime startDate = Today.AddMonths(month*(-1));
            
            List<Patient> patients =await _patientData.GetPharmacyPatientsByCondition(recordNumber, pageLimit,pharmacyId, condition,keywords,sortDirection,filterType,filterValue,filterCategory);

            foreach(Patient patient in patients)
            {
                patient.Medications = patient.Medications.GroupBy(p => p.DrugSubGroup).Select(p => p.LastOrDefault()).ToList();
                Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


                patient.CholestrolPDC = cholesterol;
                patient.RASAPDC = rasa;
                patient.DiabetesPDC = diabetes;

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
                                patients = patients.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC >= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                patients = patients.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC >= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                patients = patients.Where(p => p.DiabetesPDC >= filterMinValueD && p.DiabetesPDC >= filterMaxValueD).ToList();
                                break;
                        }
                        break;
                }
            }

            return patients;
        }

        public async Task<List<Patient>> GetAllPharmacyPatientsByCondition(int recordNumber, int pageLimit, DateTime startDate, DateTime endDate, int pharmacyId, string condition, int month, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            List<Patient> patientList =new List<Patient>();
            bool conditionCheck = false;
            if (sortDirection == "asc")
            {
                conditionCheck = true;
            }
            DateTime searchDateOfBirth;
            bool isDateOfBirthValid = DateTime.TryParseExact(keywords, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out searchDateOfBirth);

            List<Patient> patients = await _patientData.GetAllPharmacyPatientsByCondition(recordNumber, pageLimit, pharmacyId, condition, keywords, sortDirection, filterType, filterValue, filterCategory);
      
            if (!string.IsNullOrEmpty(filterType))
            {
                switch (filterType)
                {
                    case "Status":
                        patientList = patients.Where(p => p.Pharmacy.Id == pharmacyId &&
                         p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Status.ToLower() == filterValue.ToLower())
                        .ToList();
                        break;

                    case "Organization":
                        patientList = patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Pharmacy.Name.ToLower().Contains(filterValue.ToLower()))
                        .ToList();
                        break;
                    case "State":
                        patientList = patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && p.Address.State.ToLower().Contains(filterValue.ToLower()))
                        .ToList();
                        break;

                    case "PDC Category and Average":
                        patientList = patients.Where(p => p.Pharmacy.Id == pharmacyId && p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted)
                        .ToList();
                        break;

                }
                return patientList.Skip(recordNumber).Take(pageLimit).ToList();
                        
            }

            if (condition == "Cholesterol" || condition == "Diabetes" || condition == "RASA")
                {
                      patientList = patients
                     .Where(p => p.Pharmacy.Id == pharmacyId &&
                      p.Medications.Any(med => med.Condition == condition) && !p.IsDeleted && (string.IsNullOrEmpty(keywords) ||
                      p.Contact.FirstName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      p.Contact.LastName.Contains(keywords, StringComparison.OrdinalIgnoreCase) ||
                      (p.Contact.FirstName + " " + p.Contact.LastName).Contains(keywords, StringComparison.OrdinalIgnoreCase) || (isDateOfBirthValid && p.Contact.DoB.Date == searchDateOfBirth.Date)))
                      .OrderBy(p => conditionCheck ? p.Contact.FirstName : null)
                      .ThenByDescending(p => conditionCheck ? null : p.Contact.FirstName)
                      .Skip(recordNumber >= 0 && pageLimit != 0 ? recordNumber : 0)
                      .Take(recordNumber >= 0 && pageLimit != 0 ? pageLimit : int.MaxValue)
                      .ToList();

                foreach (Patient patient in patientList)
                {
                    patient.Medications = patient.Medications.GroupBy(p => p.DrugSubGroup).Select(p => p.LastOrDefault()).ToList();
                    Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                    Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                    Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);

                    patient.CholestrolPDC = cholesterol;
                    patient.RASAPDC = rasa;
                    patient.DiabetesPDC = diabetes;

                   
                }

                return patientList;
                }

            return null;

        }

        public List<Patient> GetPatientsByCondition(List<Patient> patients, string condition)
        {
           
            var conditionPatients = patients.Where(p => p.Medications.Any(Medication => Medication.Condition == condition) && p.IsDeleted == false).ToList();
 
            return conditionPatients;

        }

        
        public async Task<Response<Patient>> GetPatientsByUserId( string userId,DateTime startDate ,DateTime endDate,int month)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserId(userId);

 
            response.Success = true;
            response.DataList = patients;
            return response;
        }
        public async Task<Response<Patient>> GetPatientsByUserIdWithPagination(int recordNumber, int pageLimit, string userId, DateTime startDate, DateTime endDate, int month, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserIdWithPagination(recordNumber, pageLimit, userId,keywords,sortDirection,filterType,filterValue,filterCategory);
            foreach (Patient patient in patients)
            {
                Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


                patient.CholestrolPDC = cholesterol;
                patient.RASAPDC = rasa;
                patient.DiabetesPDC = diabetes;
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
                                patients = patients.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                break;
                            case "RASA":
                                string[] filterArrayRasa = filterValue.Split("-");
                                double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                patients = patients.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC <= filterMaxValueR).ToList();
                                break;
                            case "Diabetes":
                                string[] filterArrayDiabetes = filterValue.Split("-");
                                double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                patients = patients.Where(p => p.DiabetesPDC >= filterMinValueD && p.DiabetesPDC <= filterMaxValueD).ToList();
                                break;
                        }
                        break;
                }
            }

            response.Success = true;
            response.DataList = patients;
            return response;
        }


        public async Task<Response<Patient>> GetNonAdherencePatientsForUser(string userId, String condition, DateTime startDate, DateTime endDate, int month, int recordNumber, int pageLimit, string keywords, string sortDirection, string filterType, string filterValue, string filterCategory)
        {
            Response<Patient> response = new Response<Patient>();
            var pdcMonth = startDate.Date > endDate.Date ? startDate : endDate;
            var queryType = PdcQueryType.ByPdcMonth;

            if (month == 12)
            {
                var range = _patientPdcService.GetMedicationPeriodForGraph(month);
                pdcMonth = range.Item2;
                queryType = PdcQueryType.ByEndDate;
            }

            var pdcList = await _patientPdcData.GetPatientPDCListForUserAsync(userId, condition, pdcMonth, month, queryType);
            pdcList = pdcList.Where(p => p.PDC > 0 && p.PDC < 80).ToList();

            var patients =  _patientData.GetNonAdherencePatientList(pdcList, recordNumber, pageLimit, keywords, sortDirection, filterType, filterValue, filterCategory);
            foreach (Patient patient in patients)
            {
                Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


                patient.CholestrolPDC = cholesterol;
                patient.RASAPDC = rasa;
                patient.DiabetesPDC = diabetes;
            }
            response.Success = true;
            response.Message = "Non adherence patients list retrived";
            response.DataList = patients;

            return response;
        }

        public async Task<Response<Patient>> GetPatientsByUserIdForEmployee(string userId, DateTime startDate, DateTime endDate, int month, bool calculatePDCs = true)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserIdForEmployee(userId);

            if (calculatePDCs)
            {
                foreach (Patient patient in patients)
                {
                    patient.RASAPDC = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                    patient.DiabetesPDC = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);
                    patient.CholestrolPDC = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                }
            }

            response.Success = true;
            response.DataList = patients;
            return response;
        }



        public async Task<Response<Patient>> GetPatientsByUserIdForEmployeeForPDC(string userId, DateTime startDate, DateTime endDate, int month)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserIdForEmployeeForPDC(userId);

            foreach (Patient patient in patients)
            {
                patient.RASAPDC = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                patient.DiabetesPDC = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);
                patient.CholestrolPDC = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
            }

            response.Success = true;
            response.DataList = patients;
            return response;
        }


        public async Task<Response<Patient>> GetPatientsByPatientStatus(string userId, DateTime startDate, DateTime endDate,string patientStatus,int month)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserId(userId);

            List<Patient> patientList = new List<Patient>();
            foreach (Patient patient in patients)
            {
                if (!patient.IsDeleted && patient.Status == patientStatus)
                {
                    patientList.Add(patient);
                }
            }
            List<Patient> calculatedPatientRASA = await _medicationService.CalculatePdcforPatients(patientList, PDC.RASA.ToString(), startDate, endDate,month);
            List<Patient> calculatedPatientDiabetes = await _medicationService.CalculatePdcforPatients(calculatedPatientRASA, PDC.Diabetes.ToString(), startDate, endDate,month);
            List<Patient> calculatedPatientCholestrol = await _medicationService.CalculatePdcforPatients(calculatedPatientDiabetes, PDC.Cholesterol.ToString(), startDate, endDate,month);



            response.Success = true;
            response.DataList = calculatedPatientCholestrol;
            return response;
        }


        public async Task<Response<Patient>> GetPatientsByDueforRefills(string userId, DateTime startDate, DateTime endDate,int month)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserId(userId);
 
            List<Patient> patientList = new List<Patient>();
            foreach (Patient patient in patients)
            {
                List<Medication> medications = await _medicationData.GetMedicationByPatientId(patient.Id);
                int count =await _medicationService.countDueForRefill(medications);
                if (!patient.IsDeleted && count > 0)
                {
                    patientList.Add(patient);
                }
            }
            List<Patient> calculatedPatientRASA = await _medicationService.CalculatePdcforPatients(patientList, PDC.RASA.ToString(), startDate, endDate,month);
            List<Patient> calculatedPatientDiabetes = await _medicationService.CalculatePdcforPatients(calculatedPatientRASA, PDC.Diabetes.ToString(), startDate, endDate,month);
            List<Patient> calculatedPatientCholestrol = await _medicationService.CalculatePdcforPatients(calculatedPatientDiabetes, PDC.Cholesterol.ToString(), startDate, endDate, month);



            response.Success = true;
            response.DataList = calculatedPatientCholestrol;
            return response;
        }

        public async Task<Response<Patient>> GetPatientsByNoRefillRemaining(string userId, DateTime startDate, DateTime endDate,int month)
        {
            Response<Patient> response = new Response<Patient>();
            List<Patient> patients = await _patientData.GetPatientsByUserId(userId);

            List<Patient> patientList = new List<Patient>();
            foreach (Patient patient in patients)
            {
                List<Medication> medications = await _medicationData.GetMedicationByPatientId(patient.Id);
                int count = await _medicationService.countDueForRefill(medications);
                if (!patient.IsDeleted && count > 0)
                {
                    patientList.Add(patient);
                }
            }
            List<Patient> calculatedPatientRASA = await _medicationService.CalculatePdcforPatients(patientList, PDC.RASA.ToString(), startDate, endDate, month);
            List<Patient> calculatedPatientDiabetes = await _medicationService.CalculatePdcforPatients(calculatedPatientRASA, PDC.Diabetes.ToString(), startDate, endDate, month);
            List<Patient> calculatedPatientCholestrol = await _medicationService.CalculatePdcforPatients(calculatedPatientDiabetes, PDC.Cholesterol.ToString(), startDate, endDate, month);



            response.Success = true;
            response.DataList = calculatedPatientCholestrol;
            return response;
        }

        public int countNewPatient(List<Patient> patients)
        {
            int count = 0;    
            foreach (Patient patient in patients)
            {
                if (!patient.IsDeleted)
                {
                    if (patient.Status.Equals("New Patient"))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public int countNewPatient(int id)
        {
            return _patientData.countNewPatient(id);
        }

        public int countInProgressPatient(List<Patient> patients)
        {
            int count = 0;

            foreach (Patient patient in patients)
            {
                if (patient.Status.Equals("In Progress"))
                {
                    count++;
                }
            }

            return count;
        }


        public async Task<Response<Patient>> GetPatientListByStatusAsync(string userId, string patientStatus,int recordNumber,int pageLimit, int month,DateTime startDate,DateTime endDate, string keywords,string sortDirection, string filterType, string filterValue, string filterCategory)

        {
            var response = new Response<Patient>();
            List<Patient> patients = null;

            if (!string.IsNullOrWhiteSpace(patientStatus))
            {
                if (patientStatus.Equals("due-for-refill", StringComparison.OrdinalIgnoreCase))
                {
                     patients = await _patientData.GetPatientsDueForRefillAsync(userId, recordNumber, pageLimit, keywords, sortDirection, filterType, filterValue);
                }
                else if (patientStatus.Equals("no-refills-remaining", StringComparison.OrdinalIgnoreCase))
                {
                    patients = await _patientData.GetPatientsWithNoRefillAsync(userId, recordNumber, pageLimit, keywords, sortDirection, filterType, filterValue);
                }
                else
                {
                    patientStatus = patientStatus.Replace("-", " ");

                     patients =  _patientData.GetPatientListByStatusAsync(userId, patientStatus, recordNumber, pageLimit, keywords, sortDirection, filterType, filterValue, filterCategory);

                    response.DataList = patients;
                }
                foreach (Patient patient in patients)
                {
                    Double cholesterol = await _medicationService.CalculatePdc(patient.Id, PDC.Cholesterol.ToString(), startDate, endDate, month);
                    Double rasa = await _medicationService.CalculatePdc(patient.Id, PDC.RASA.ToString(), startDate, endDate, month);
                    Double diabetes = await _medicationService.CalculatePdc(patient.Id, PDC.Diabetes.ToString(), startDate, endDate, month);


                    patient.CholestrolPDC = cholesterol;
                    patient.RASAPDC = rasa;
                    patient.DiabetesPDC = diabetes;
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
                                    patients = patients.Where(p => p.CholestrolPDC >= filterMinValue && p.CholestrolPDC <= filterMaxValue).ToList();
                                    break;
                                case "RASA":
                                    string[] filterArrayRasa = filterValue.Split("-");
                                    double filterMinValueR = Convert.ToDouble(filterArrayRasa[0].Split("%")[0]);
                                    double filterMaxValueR = Convert.ToDouble(filterArrayRasa[1].Split("%")[0]);
                                    patients = patients.Where(p => p.RASAPDC >= filterMinValueR && p.RASAPDC <= filterMaxValueR).ToList();
                                    break;
                                case "Diabetes":
                                    string[] filterArrayDiabetes = filterValue.Split("-");
                                    double filterMinValueD = Convert.ToDouble(filterArrayDiabetes[0].Split("%")[0]);
                                    double filterMaxValueD = Convert.ToDouble(filterArrayDiabetes[1].Split("%")[0]);
                                    patients = patients.Where(p => p.DiabetesPDC >= filterMinValueD && p.DiabetesPDC <= filterMaxValueD).ToList();
                                    break;
                            }
                            break;
                    }
                }

                response.Success = true;
                response.DataList = patients;
            }

            return response;
        }

        public async Task<bool> UpdatePatientConsentAsync(int patientId, string consentType, bool allow)
        {
            var consentTypes = new string[] { "email", "call", "text", "birthday-sms" };

            if (!consentTypes.Contains(consentType)) return false;

            return await _patientData.UpdatePatientConsentAsync(patientId, consentType, allow);
        }

        public async Task<PatientDiseaseCount> GetPatientDiseaseCountById(int id, DateTime startDate, DateTime endDate,int month, string condition)
        {
            PatientDiseaseCount patientDiseaseCount = new PatientDiseaseCount();
            patientDiseaseCount.PatientCount = _patientData.GetPatientDiseaseCountById(id, startDate, endDate, month, condition);
            patientDiseaseCount.Condition = condition;
            return patientDiseaseCount;
        }
    }
}
