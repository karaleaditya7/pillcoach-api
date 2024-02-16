using iTextSharp.text.pdf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace OntrackDb.Service
{
    public class ReconciliationService : IReconciliationService
    {

        private readonly IMedicationData _medicationData;
        private readonly IDoctorData _doctorData;
        private readonly IPatientData _patientData;
        private readonly IReconciliationData _reconciliationData;
        private readonly ApplicationDbContext _applicationDbcontext;
         IConfiguration _configuration;
        private readonly HttpClient clientRx;
        private readonly HttpClient client;
        private readonly INdcApiService _ndcApiService;
        private readonly IMedicationToDoListData _medicationToDoListData;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly IDoctorMedicationData _doctorMedicationData;

        public ReconciliationService(IHttpClientFactory clientFactory, IDoctorMedicationData doctorMedicationData, IMedicationData medicationData, ApplicationDbContext applicationDbcontext, IReconciliationData reconciliationData, IDoctorData doctorData, IConfiguration configuration, INdcApiService ndcApiService, IPatientData patientData, IMedicationToDoListData medicationToDoListData, IOtcMedicationData otcMedicationData)
        {
            _medicationData = medicationData;
            _applicationDbcontext = applicationDbcontext;
            _reconciliationData = reconciliationData;
            _doctorData = doctorData;
            _configuration = configuration;
            clientRx = clientFactory.CreateClient("RxInfoServices");
            client = clientFactory.CreateClient("NdcServices");
            _ndcApiService = ndcApiService;
            _patientData = patientData;
            _medicationToDoListData = medicationToDoListData;
            _otcMedicationData = otcMedicationData;
            _doctorMedicationData = doctorMedicationData; 
        }

        public async Task<Response<MedicationReconciliation>> AddReconciliationMedication(MedicationRecocilationModel model)
        {
            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            response.Success = false;

            var doctor = await _doctorData.GetDoctorById(model.DoctorId);
            var patient = await _patientData.GetPatientById(model.PatientId);

            MedicationReconciliation medicationReconciliation = new MedicationReconciliation
            {
                DoctorPrescribed = doctor,
                Condition = model.Condition,
                Direction = model.Condition,
                SBDCName = model.SBDCName,
                ActionItem = model.ActionItem,
                Patient = patient
            };
            DoctorMedication doctorMedication = new DoctorMedication
            {
                DoctorPrescribed = doctor,
                MedicationReconciliation = medicationReconciliation,
            };
            var result = await _reconciliationData.AddReconciliationMedication(medicationReconciliation);
             await _doctorMedicationData.AddDoctorMedication(doctorMedication);

            if (result == null)
            {
                response.Message = "Error while creating ReconciliationCmrMedication";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = medicationReconciliation;
            response.Message = "ReconciliationCmrMedication created successfully!";
            return response;
        }

        public async Task<Response<MedicationReconciliation>> GetReconciliationMedicationsByPatientId(int patientId)
        {
         

            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            var medicationReconciliations = await _reconciliationData.GetUniqueReconciliationMedicationsByPatientId(patientId);
           
            foreach (MedicationReconciliation medicationReconciliation1 in medicationReconciliations)
            {
                if (string.IsNullOrEmpty(medicationReconciliation1.SBDCName) || (string.IsNullOrEmpty(medicationReconciliation1.Condition) && string.IsNullOrEmpty(medicationReconciliation1.OptionalCondition)) || string.IsNullOrEmpty(medicationReconciliation1.Direction) || string.IsNullOrEmpty(medicationReconciliation1.ActionItem))
                {
                    medicationReconciliation1.IsValidateCmr = false;
                    var result = await _reconciliationData.UpdateReconciliationMedication(medicationReconciliation1);
                }
                else
                {
                    medicationReconciliation1.IsValidateCmr = true;
                    var result = await _reconciliationData.UpdateReconciliationMedication(medicationReconciliation1);
                }
            }

            List<MedicationReconciliation> medicationReconciliationList = new List<MedicationReconciliation>();

            if (medicationReconciliations.Count == 0)
            {
                var medications = await _medicationData.GetUniqueMedicationsByPatientIdForCmrMedication(patientId);
                foreach (Medication medication in medications)
                {
                    JsonElement brandName;
                    var urlRxCui = string.Format(_configuration["Rx-nav-Uri"] + medication.NDCNumber);
                    var response1 = await client.GetAsync(urlRxCui);
                    string SbdcName = "";
                    string fullGenericName = "";
                    if (response1.IsSuccessStatusCode)
                    {

                        var stringResponse1 = await response1.Content.ReadAsStringAsync();
                        JsonDocument document1 = JsonDocument.Parse(stringResponse1);
                        JsonElement root1 = document1.RootElement;
                        JsonElement body1 = root1.GetProperty("ndcStatus");
                        JsonElement rxcui = body1.GetProperty("rxcui");
                        if (rxcui.ToString() != "")
                        {
                            var allRxInfoUrl = string.Format("/REST/rxcui/" + rxcui + "/allrelatedextension.json?caller=RxNav");
                            var allRxInfoResponse = await client.GetAsync(allRxInfoUrl);
                            var stringResponseForallRxInfo = await allRxInfoResponse.Content.ReadAsStringAsync();
                            JsonDocument document2 = JsonDocument.Parse(stringResponseForallRxInfo);
                            JsonElement root2 = document2.RootElement;
                            JsonElement brandNameResponse = root2.GetProperty("allRelatedGroup").GetProperty("conceptGroup")[9];

                            var allRxInfoUrlForGenericName = string.Format("/REST/RxTerms/rxcui/" + rxcui + "/allinfo.json");
                            var allRxInfoResponseForGenericName = await clientRx.GetAsync(allRxInfoUrlForGenericName);
                            var stringResponseForallRxInfoForGenericName = await allRxInfoResponseForGenericName.Content.ReadAsStringAsync();
                            JsonDocument document3 = JsonDocument.Parse(stringResponseForallRxInfoForGenericName);
                            JsonElement root3 = document3.RootElement;
                            var checkValue = root3.TryGetProperty("rxtermsProperties", out JsonElement value);
                            if (checkValue)
                            {
                                JsonElement body3 = root3.GetProperty("rxtermsProperties");
                                JsonElement fullGenericName1 = body3.GetProperty("fullGenericName");
                                fullGenericName = fullGenericName1.ToString();
                            }


                            var check = brandNameResponse.TryGetProperty("conceptProperties", out JsonElement value1);

                            if (check)
                            {
                                brandName = brandNameResponse.GetProperty("conceptProperties")[0].GetProperty("name");
                                SbdcName = brandName.ToString();
                            }
                        }


                    }

                    MedicationReconciliation medicationReconciliation = new MedicationReconciliation()
                    {

                        RxNumber = medication.RxNumber,
                        RxVendorRxID = medication.RxVendorRxID,
                        DoctorPrescribed = medication.DoctorPrescribed,
                        Patient = medication.Patient,
                        RefillsRemaining = medication.RefillsRemaining,
                        Condition = medication.OptionalCondition,
                        DrugSubGroup = medication.DrugSubGroup,
                        GenericName = medication.GenericName,
                        NDCNumber = medication.NDCNumber,
                        LastFillDate = medication.LastFillDate,
                        AdherenceRate = medication.AdherenceRate,
                        ConditionTreated = medication.ConditionTreated,
                        PayDue = medication.PayDue,
                        IsExclude = medication.IsExclude,
                        IsInclude = medication.IsInclude,
                        PrescriberName = medication.PrescriberName,
                        Quantity = medication.Quantity,
                        RelatedPharmacies = medication.RelatedPharmacies,
                        Direction = medication.Direction,
                        DrugName = medication.DrugName,
                        Supply = medication.Supply,
                        SBDCName = SbdcName,
                        OptionalCondition = medication.OptionalCondition,
                    };
                    if (!string.IsNullOrEmpty(SbdcName))
                    {
                        medicationReconciliation.SBDCName = SbdcName;
                    }
                    else if (!string.IsNullOrEmpty(fullGenericName))
                    {
                        medicationReconciliation.SBDCName = fullGenericName;
                    }
                    else
                    {
                        medicationReconciliation.SBDCName = medication.DrugSubGroup;
                    }

                    DoctorMedication doctorMedication = new DoctorMedication
                    {
                        DoctorPrescribed = medicationReconciliation.DoctorPrescribed,
                        MedicationReconciliation = medicationReconciliation,
                    };

                    await _applicationDbcontext.MedicationReconciliations.AddAsync(medicationReconciliation);
                    await _applicationDbcontext.DoctorMedications.AddAsync(doctorMedication);
                    medicationReconciliationList.Add(medicationReconciliation);
                }
                await _applicationDbcontext.SaveChangesAsync();
                var  medicationReconciliations1 =  await UpdateOtcMedication(patientId);
                medicationReconciliationList.AddRange(medicationReconciliations1);

                response.Success = true;
                response.Message = "Unique cmrMedication with Patient Id retrived Successfully";
                response.DataList = medicationReconciliationList;
                return response;
            }
            else
            {

                foreach (MedicationReconciliation medicationReconciliation in medicationReconciliations)
                {
                    var reconciliationToDoRelated = await _reconciliationData.GetMedicationToDoListRelatedByMedicationReconciliationId(medicationReconciliation.Id);

                    var doctorMedication = await _doctorMedicationData.GetDoctorMedicationByMedRecId(medicationReconciliation.Id);
                    if (doctorMedication == null && medicationReconciliation.DoctorPrescribed != null)
                    {
                        DoctorMedication doctorMedication1 = new DoctorMedication
                        {
                            DoctorPrescribed = medicationReconciliation.DoctorPrescribed,
                            MedicationReconciliation = medicationReconciliation,
                        };
                        await _doctorMedicationData.AddDoctorMedication(doctorMedication1);
                    }

                    if (reconciliationToDoRelated != null)
                    {
                        medicationReconciliation.IsAttached = true;
                        var result = await _reconciliationData.UpdateReconciliationMedication(medicationReconciliation);
                    }
                    else
                    {
                        medicationReconciliation.IsAttached = false;
                        var result = await _reconciliationData.UpdateReconciliationMedication(medicationReconciliation);
                    }
                }
                var medicationReconciliations1 = await UpdateOtcMedication(patientId);
                medicationReconciliations.AddRange(medicationReconciliations1);
                response.Success = true;
                response.Message = "Unique medicationReconciliations with Patient Id retrived Successfully";
                response.DataList = medicationReconciliations;

                return response;
            }

        }

        public async Task<List<MedicationReconciliation>> UpdateOtcMedication(int patientId)
        {
            var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(patientId);
            List<MedicationReconciliation> medicationReconciliations = new List<MedicationReconciliation>();
           
            foreach (OtcMedication otcMedication in otcMedications)
            {
                bool check = true;
                if (string.IsNullOrEmpty(otcMedication.SBDCName) || string.IsNullOrEmpty(otcMedication.Condition) || string.IsNullOrEmpty(otcMedication.Direction) || otcMedication.DoctorPrescribed == null )
                {
                    check = false;
                }

                if (!otcMedication.IsCmrCreated && check)
                {
                    CmrMedication cmrMedication = new CmrMedication()
                    {
                        DoctorPrescribed = otcMedication.DoctorPrescribed,
                        Patient = otcMedication.Patient,
                        Condition = otcMedication.Condition,
                        GenericName = otcMedication.GPCKName,
                        Direction = otcMedication.Direction,
                        SBDCName = otcMedication.SBDCName
                    };
                    otcMedication.IsCmrCreated = true;
                    await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    await _applicationDbcontext.CmrMedications.AddAsync(cmrMedication);
                }
                if (!otcMedication.IsRecCreated && check )
                {

                    MedicationReconciliation medicationReconciliation = new MedicationReconciliation()
                    {
                        DoctorPrescribed = otcMedication.DoctorPrescribed,
                        Patient = otcMedication.Patient,
                        SBDCName = otcMedication.SBDCName,
                        Direction = otcMedication.Direction,
                        Condition = otcMedication.Condition
                    };
                    DoctorMedication doctorMedication = new DoctorMedication
                    {
                        DoctorPrescribed = medicationReconciliation.DoctorPrescribed,
                        MedicationReconciliation = medicationReconciliation,
                    };

                    DoctorMedication doctorMedication1 = new DoctorMedication
                    {
                        DoctorPrescribed = otcMedication.DoctorPrescribed,
                        OtcMedication = otcMedication,
                    };

                    otcMedication.IsRecCreated = true;
                    await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    await _reconciliationData.AddReconciliationMedication(medicationReconciliation);

                    await _doctorMedicationData.AddDoctorMedication(doctorMedication);
                    await _doctorMedicationData.AddDoctorMedication(doctorMedication1);
                    medicationReconciliations.Add(medicationReconciliation);
                }
 
            }
            await _applicationDbcontext.SaveChangesAsync();
            return medicationReconciliations;
        }


        public async Task<Response<RxNavMedication>> SaveAllRxNavMedications()
        {
            string fullGenericName = "";
            Response<RxNavMedication> response = new Response<RxNavMedication>();
            var url = string.Format(_configuration["Rx-nav-AllNdcs"]);
            var responseContent = await clientRx.GetAsync(url);
            if (responseContent.IsSuccessStatusCode)
            {
               
                var stringResponse = await responseContent.Content.ReadAsStringAsync();
                JsonDocument document = JsonDocument.Parse(stringResponse);
                JsonElement root = document.RootElement;
                JsonElement body = root.GetProperty("ndcList");
                JsonElement NDCNumbers = body.GetProperty("ndc");
                int count = 0;
                foreach (var ndcNumber in NDCNumbers.EnumerateArray())
                {
                    var urlRxCui = string.Format(_configuration["Rx-nav-Uri"] + ndcNumber);
                    var response1 = await client.GetAsync(urlRxCui);
                    if (response1.IsSuccessStatusCode)
                    {

                        var stringResponse1 = await response1.Content.ReadAsStringAsync();
                        JsonDocument document1 = JsonDocument.Parse(stringResponse1);
                        JsonElement root1 = document1.RootElement;
                        JsonElement body1 = root1.GetProperty("ndcStatus");
                        JsonElement rxcui = body1.GetProperty("rxcui");
                        if (rxcui.ToString() != "")
                        {
                            var allRxInfoUrl = string.Format("/REST/rxcui/" + rxcui + "/allrelatedextension.json?caller=RxNav");
                            var allRxInfoResponse = await client.GetAsync(allRxInfoUrl);
                            var stringResponseForallRxInfo = await allRxInfoResponse.Content.ReadAsStringAsync();
                            JsonDocument document2 = JsonDocument.Parse(stringResponseForallRxInfo);
                            JsonElement root2 = document2.RootElement;
                            JsonElement brandNameResponse = root2.GetProperty("allRelatedGroup").GetProperty("conceptGroup")[9];

                            var allRxInfoUrlForGenericName = string.Format("/REST/RxTerms/rxcui/" + rxcui + "/allinfo.json");
                            var allRxInfoResponseForGenericName = await clientRx.GetAsync(allRxInfoUrlForGenericName);
                            var stringResponseForallRxInfoForGenericName = await allRxInfoResponseForGenericName.Content.ReadAsStringAsync();
                            JsonDocument document3 = JsonDocument.Parse(stringResponseForallRxInfoForGenericName);
                            JsonElement root3 = document3.RootElement;
                            var checkValue = root3.TryGetProperty("rxtermsProperties", out JsonElement value);
                            if (checkValue)
                            {
                                JsonElement body3 = root3.GetProperty("rxtermsProperties");
                                JsonElement fullGenericName1 = body3.GetProperty("fullGenericName");
                                fullGenericName = fullGenericName1.ToString();
                            }

                            var check = brandNameResponse.TryGetProperty("conceptProperties", out JsonElement value1);
                            if (check)
                            {
                                JsonElement brandName = brandNameResponse.GetProperty("conceptProperties")[0].GetProperty("name");
                                RxNavMedication rxNavMedication = new RxNavMedication()
                                {
                                    NDCNumber = ndcNumber.ToString(),
                                    SBDCName = brandName.ToString(),
                                    GenericName = fullGenericName,
                                };
                                await _applicationDbcontext.RxNavMedications.AddAsync(rxNavMedication);
                                count++;
                                if (count == 500)
                                {
                                    await _applicationDbcontext.SaveChangesAsync();
                                    count = 0;
                                }
                            }
                        }

                    }

                }
                await _applicationDbcontext.SaveChangesAsync();

                response.Success = true;
                response.Message = "RxNavMedication Added Successfully!";
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to Add Entity";
                return response;
            }

        }

        public async Task<Response<MedicationReconciliation>> GetRecMedicationById(int id)
        {
            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            var medicationReconciliation = await _reconciliationData.GetReconciliationMedicationsById(id);

            if (medicationReconciliation == null)
            {
                response.Message = "MedicationReconciliation Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "MedicationReconciliation retrived successfully";
            response.Data = medicationReconciliation;
            return response;
        }


        public async Task<Response<MedicationReconciliation>> DeleteReconciliationMedicationsById(int id)
        {
            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            MedicationReconciliation medicationReconciliation = await _reconciliationData.GetReconciliationMedicationsById(id);
           
            if (medicationReconciliation == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            else
            {
                var medRec = await _reconciliationData.GetUniqueReconciliationMedicationsByPatientId(medicationReconciliation.Patient.Id);
                if (medRec.Count == 1)
                {
                    var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(medicationReconciliation.Patient.Id);
                    foreach (OtcMedication otcMedication in otcMedications)
                    {
                        otcMedication.IsRecCreated = false;
                        await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    }
                }
                //TODO : when reconcilation part for medicationrectodo is done
                var reconciliationToDoRelated = await _reconciliationData.GetMedicationToDoListRelatedByMedicationReconciliationId(medicationReconciliation.Id); ;
                var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByMedRecId(id);
                if (reconciliationToDoRelated != null)
                {
                     _reconciliationData.PatientDeleteReconciliationToDoRelated(reconciliationToDoRelated);
                }
                if (doctorMedications != null)
                {
                    foreach (DoctorMedication doctorMedication in doctorMedications)
                    {
                        _doctorMedicationData.DeleteDoctorMedicationForCmr(doctorMedication);
                    }

                }
                await _reconciliationData.DeleteReconciliationMedication(medicationReconciliation);
                
                response.Success = true;
                response.Message = "MedicationReconciliation Deleted Successfully!";
                return response;

            }

        }

        public  Response<string> SearchForMedication(string search)
        {
            Response<string> response = new Response<string>();
            var sbdc = _reconciliationData.SearchForRxNavMedicationSBDC(search);
            var gpck =  _reconciliationData.SearchForRxNavMedicationGPCK(search);
            sbdc.AddRange(gpck);
            response.Success = true;
            response.Message = "RxNavMedications Retrived Successfully!";
            response.DataList = new List<string>(sbdc);
            return response;
        }

        public async Task<Response<Doctor>> GetAllDoctors(int id)
        {

            Response<Doctor> response = new Response<Doctor>();
            var doctors = await _doctorMedicationData.GetAlldoctorsforMedication(id);
            var otcDoctors = await _doctorMedicationData.GetAlldoctorsforOtc(id);
            doctors.AddRange(otcDoctors);
            var medRecDoctors = await _doctorMedicationData.GetAlldoctorsforMedRecMedication(id);
            doctors.AddRange(medRecDoctors);

            doctors = doctors.Distinct().ToList();  

            if (doctors == null)
            {
                response.Success = false;
                response.Message = "doctors Not Found";
                return response;
            }

            response.Success = true;
            response.Message = "doctors retrived successfully";
            response.DataList = doctors;
            return response;
        }


        public async Task<Response<string>> GetAllConditionsforMedRe(int patientId)
        {
            Response<string> response = new Response<string>();
            List<string> medConditions = await _medicationData.GetUniqueConditionByPatientId(patientId);
            List<string> otcConditions = await _otcMedicationData.GetAllConditionsforOtcMedication(patientId);
            medConditions.AddRange(otcConditions);
            List<string> medRecConditions = await _reconciliationData.GetUniqueConditionForMedRecByPatientId(patientId);
            medConditions.AddRange(medRecConditions);


            medConditions = medConditions.Distinct().ToList();
            response.Success = true;
            response.Message = "Conditions Retrived Successfully!";
            response.DataList = new List<string>(medConditions);

            return response;
        }


        public async Task<Response<MedicationReconciliation>> UpdateReconciliationMedication(MedicationRecocilationModel model)
        {
            Response<MedicationReconciliation> response = new Response<MedicationReconciliation>();
            MedicationReconciliation medicationReconciliation = await _reconciliationData.GetReconciliationMedicationsById(model.Id);

            var medRecPrescribedDoc = medicationReconciliation.DoctorPrescribed;
            if (medicationReconciliation == null)
            {
                response.Success = false;
                response.Message = "medicationReconciliation not found";
                return response;
            }

            medicationReconciliation.Direction = model.Direction;
            medicationReconciliation.Condition = model.Condition;
            medicationReconciliation.SBDCName = model.SBDCName;
            medicationReconciliation.ActionItem = model.ActionItem;
            var result = await _reconciliationData.UpdateReconciliationMedication(medicationReconciliation);

            if (medicationReconciliation.DoctorPrescribed != null && medicationReconciliation.DoctorPrescribed != medRecPrescribedDoc)
            {

                DoctorMedication doctorMedication = new DoctorMedication();
                doctorMedication.DoctorPrescribed = medicationReconciliation.DoctorPrescribed;
                doctorMedication.MedicationReconciliation = medicationReconciliation;
                await _doctorMedicationData.AddDoctorMedication(doctorMedication);

            }

            response.Success = true;
            response.Message = "MedicationReconciliation Updated successfully!";
            response.Data = result;
            return response;
        }
    }
}
