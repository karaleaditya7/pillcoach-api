using iTextSharp.text;
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
    public class CmrMedicationService :ICmrMedicationService
    {
        private readonly IMedicationData _medicationData;
        private readonly IDoctorData _doctorData;
        private readonly IPatientData _patientData;
        private readonly ICmrMedicationData _cmrMedicationData;
        private readonly IReconciliationData _reconciliationData;
        private readonly ApplicationDbContext _applicationDbcontext;
        IConfiguration _configuration;
        private readonly HttpClient clientRx;
        private readonly HttpClient client;
        private readonly INdcApiService _ndcApiService;
        private readonly IMedicationToDoListData _medicationToDoListData;
        private readonly IOtcMedicationData _otcMedicationData;
        private readonly IDoctorMedicationData _doctorMedicationData;
        

        public CmrMedicationService(IHttpClientFactory clientFactory, IDoctorMedicationData doctorMedicationData, IReconciliationData reconciliationData, IMedicationData medicationData, ApplicationDbContext applicationDbcontext, ICmrMedicationData cmrMedicationData, IDoctorData doctorData, IConfiguration configuration, INdcApiService ndcApiService, IPatientData patientData , IMedicationToDoListData medicationToDoListData,IOtcMedicationData otcMedicationData)
        {
            _medicationData = medicationData;  
            _applicationDbcontext = applicationDbcontext;
            _cmrMedicationData = cmrMedicationData;
            _doctorData = doctorData; 
            _configuration= configuration;
            clientRx = clientFactory.CreateClient("RxInfoServices");
            client = clientFactory.CreateClient("NdcServices");
            _ndcApiService = ndcApiService;
            _patientData = patientData;
            _medicationToDoListData = medicationToDoListData;
            _otcMedicationData = otcMedicationData;
            _reconciliationData = reconciliationData;
            _doctorMedicationData= doctorMedicationData;  
        }

        public async Task<Response<CmrMedication>> AddCmrMedication(CmrMedicationModel model)
        {
            Response<CmrMedication> response = new Response<CmrMedication>();
            response.Success = false;

            var doctor = await _doctorData.GetDoctorById(model.DoctorId);
            var patient = await _patientData.GetPatientById(model.PatientId);

            CmrMedication cmrMedication = new CmrMedication
            {
                DoctorPrescribed = doctor,
                Condition =model.Condition,
                Direction = model.Condition,
                SBDCName = model.SBDCName,  
                Patient = patient
            };

            DoctorMedication doctorMedication = new DoctorMedication
            {
                DoctorPrescribed = doctor,
                CmrMedication = cmrMedication,
            };
            var result = await _cmrMedicationData.AddCmrMedication(cmrMedication);
            var result1 = await _doctorMedicationData.AddDoctorMedication(doctorMedication);

            if (result == null)
            {
                response.Message = "Error while creating cmrMedication";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Data = cmrMedication;
            response.Message = "cmrMedication created successfully!";
            return response;
        }

        public async Task<Response<CmrMedication>> GetUniqueCmrMedicationsByPatientId(int patientId)
        {
            Response<CmrMedication> response = new Response<CmrMedication>();
            var cmrMedications = await _cmrMedicationData.GetUniqueCmrMedicationsByPatientId(patientId);
           
            foreach(CmrMedication cmrMedication1 in cmrMedications)
            {
                if(string.IsNullOrEmpty(cmrMedication1.SBDCName) || (string.IsNullOrEmpty(cmrMedication1.Condition) && string.IsNullOrEmpty(cmrMedication1.OptionalCondition)) || string.IsNullOrEmpty(cmrMedication1.Direction)|| cmrMedication1.DoctorPrescribed == null)
                {
                    cmrMedication1.IsValidateCmr = false;
                    var result = await _cmrMedicationData.UpdateCmrMedication(cmrMedication1);
                }
                else
                {
                    cmrMedication1.IsValidateCmr = true;
                    var result = await _cmrMedicationData.UpdateCmrMedication(cmrMedication1);
                }
            }

            List<CmrMedication> cmrMedicationList = new List<CmrMedication>();
           
            if (cmrMedications.Count == 0)
            {
                var medications = await _medicationData.GetUniqueMedicationsByPatientIdForCmrMedication(patientId);
                foreach(Medication medication in medications)
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
                    CmrMedication cmrMedication =new CmrMedication()
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
                        OptionalCondition = medication.OptionalCondition,
                    };
                    if (!string.IsNullOrEmpty(SbdcName))
                    {
                        cmrMedication.SBDCName = SbdcName;
                    }
                    else if(!string.IsNullOrEmpty(fullGenericName))
                    {
                        cmrMedication.SBDCName = fullGenericName;
                    }
                    else
                    {
                        cmrMedication.SBDCName = medication.DrugSubGroup;
                    }

                    DoctorMedication doctorMedication = new DoctorMedication
                    {
                        DoctorPrescribed = cmrMedication.DoctorPrescribed,
                        CmrMedication = cmrMedication,
                    };

                    DoctorMedication doctorMedication1 = new DoctorMedication
                    {
                        DoctorPrescribed = cmrMedication.DoctorPrescribed,
                        Medication = medication,
                    };


                    await _applicationDbcontext.CmrMedications.AddAsync(cmrMedication);
                    await _applicationDbcontext.DoctorMedications.AddAsync(doctorMedication);
                    await _applicationDbcontext.DoctorMedications.AddAsync(doctorMedication1);

                    cmrMedicationList.Add(cmrMedication);

                }
                await _applicationDbcontext.SaveChangesAsync();
                var cmrMedications1 = await UpdateOtcMedication(patientId);
                cmrMedicationList.AddRange(cmrMedications1);
                response.Success = true;
                response.Message = "Unique cmrMedication with Patient Id retrived Successfully";   
                response.DataList = cmrMedicationList;
                return response;
            }
            else
            {

                foreach (CmrMedication cmrMedication in cmrMedications)
                {

                    var doctorMedication = await _doctorMedicationData.GetDoctorMedicationByCmrId(cmrMedication.Id);
                    if(doctorMedication == null && cmrMedication.DoctorPrescribed != null)
                    {
                        DoctorMedication doctorMedication1 = new DoctorMedication
                        {
                            DoctorPrescribed = cmrMedication.DoctorPrescribed,
                            CmrMedication = cmrMedication,
                        };
                        await _doctorMedicationData.AddDoctorMedication(doctorMedication1);
                    }
                 var medicationToDoRelated =  await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationId(cmrMedication.Id);

                    if (medicationToDoRelated != null)
                    {
                        cmrMedication.IsAttached = true;
                        var result = await _cmrMedicationData.UpdateCmrMedication(cmrMedication);
                    }
                    else
                    {
                        cmrMedication.IsAttached = false;
                        var result = await _cmrMedicationData.UpdateCmrMedication(cmrMedication);
                    }
                }
                var cmrMedications1 = await UpdateOtcMedication(patientId);
                cmrMedications.AddRange(cmrMedications1);
                response.Success = true;
                response.Message = "Unique cmrMedication with Patient Id retrived Successfully";
                response.DataList = cmrMedications;
               
                return response;
            }

           
        }


        public async Task<Response<CmrMedication>> GetCmrMedicationById(int id)
        {
            Response<CmrMedication> response = new Response<CmrMedication>();
            var cmrMedication = await _cmrMedicationData.GetCmrMedicationsById(id);

            if (cmrMedication == null )
            {
                response.Message = "cmrMedication Not Found";
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.Message = "cmrMedication retrived successfully";
            response.Data = cmrMedication;
            return response;
        }

        public async Task<Response<Doctor>> GetAllDoctors(int id)
        {

            Response<Doctor> response = new Response<Doctor>();
            var doctors = await _doctorMedicationData.GetAlldoctorsforMedication(id);
            var otcDoctors = await _doctorMedicationData.GetAlldoctorsforOtc(id);
            doctors.AddRange(otcDoctors);
            var cmrDoctors = await _doctorMedicationData.GetAlldoctorsforCmrMedication(id);
            doctors.AddRange(cmrDoctors);

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

        public async Task<Response<string>> GetAllConditionsforCmrMedication(int patientId)
        {
            Response<string> response = new Response<string>();
            List<string> medConditions = await _medicationData.GetUniqueConditionByPatientId(patientId);
            List<string> otcConditions = await _otcMedicationData.GetAllConditionsforOtcMedication(patientId);
            medConditions.AddRange(otcConditions);
            List<string> cmrConditions = await _cmrMedicationData.GetUniqueConditionForCMRByPatientId(patientId);
            medConditions.AddRange(cmrConditions);
            

            medConditions = medConditions.Distinct().ToList();
            response.Success = true;
            response.Message = "Conditions Retrived Successfully!";
            response.DataList = new List<string>(medConditions);

            return response;
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

                            RxNavMedication rxNavMedication = new RxNavMedication()
                            {
                                NDCNumber = ndcNumber.ToString()

                            };

                            if (checkValue)
                            {
                                JsonElement body3 = root3.GetProperty("rxtermsProperties");
                                JsonElement fullGenericName1 = body3.GetProperty("fullGenericName");
                                fullGenericName = fullGenericName1.ToString();
                                rxNavMedication.GenericName = fullGenericName;
                            }
                            

                            var check = brandNameResponse.TryGetProperty("conceptProperties",out JsonElement value1);

                            if (check)
                            {
                                JsonElement brandName = brandNameResponse.GetProperty("conceptProperties")[0].GetProperty("name");
                                rxNavMedication.SBDCName = brandName.ToString();
                                
                            }
                            _applicationDbcontext.RxNavMedications.Add(rxNavMedication);
                            count++;
                            if (count == 500)
                            {
                                await _applicationDbcontext.SaveChangesAsync();
                                count = 0;
                            }
                        }

                    }
                    fullGenericName = "";
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

        public async Task<string> GetSBDCNameForNDCNumber(string ndcNumber)
        {
            string fullGenericName = "";
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
                        return brandName.ToString();
                    }
                }
            }
            return null;
        }

        public async Task<Response<string>> SearchForMedication(string search)
        {
            Response<string> response = new Response<string>();
            var sbdc =await _cmrMedicationData.SearchForRxNavMedicationSBDC(search);
            var gpck = await _cmrMedicationData.SearchForRxNavMedicationGPCK(search);
            sbdc.AddRange(gpck);
            
            response.Success = true;
            response.Message = "RxNavMedications Retrived Successfully!";
            response.DataList = new List<string>(sbdc);
            return response;
        }

        public async Task<Response<CmrMedication>> UpdateCmrMedication(CmrMedicationModel model)
        {
            Response<CmrMedication> response = new Response<CmrMedication>();
            CmrMedication cmrMedication = await _cmrMedicationData.GetCmrMedicationsById(model.Id);
            var cmrPrescribedDoc = cmrMedication.DoctorPrescribed;
            if (cmrMedication == null)
            {
                response.Success = false;
                response.Message = "cmrMedication not found";
                return response;
            }

            if (!string.IsNullOrEmpty(model.FirstName) || !string.IsNullOrEmpty(model.LastName))
            {
                var doctor = await _doctorData.GetDoctorByName(model.FirstName, model.LastName);

                if (doctor == null)
                {
                    var contact = new Contact { FirstName = model.FirstName, LastName = model.LastName };
                    var result1 = await _doctorData.AddNewDoctor(new Doctor { Contact = contact });
                    cmrMedication.DoctorPrescribed = result1;
                }
                else
                {
                    cmrMedication.DoctorPrescribed = doctor;
                }
            }

            cmrMedication.Direction = model.Direction;
            cmrMedication.Condition = model.Condition;
            cmrMedication.SBDCName = model.SBDCName;

            var result = await _cmrMedicationData.UpdateCmrMedication(cmrMedication);

            if (cmrMedication.DoctorPrescribed != null && cmrMedication.DoctorPrescribed != cmrPrescribedDoc)
            {
                DoctorMedication doctorMedication = new DoctorMedication();
                doctorMedication.DoctorPrescribed = cmrMedication.DoctorPrescribed;
                doctorMedication.CmrMedication = cmrMedication;
                await _doctorMedicationData.AddDoctorMedication(doctorMedication);
            }
            response.Success = true;
            response.Message = "CmrMedication Updated successfully!";
            response.Data = result;
            return response;
        }

        public async Task<Response<CmrMedication>> DeleteCmrMedicationById(int id)
        {
            Response<CmrMedication> response = new Response<CmrMedication>();
            CmrMedication cmrMedication = await _cmrMedicationData.GetCmrMedicationsById(id);
            if (cmrMedication == null)
            {
                response.Message = "Invalid Id";
                response.Success = false;
                return response;
            }
            else
            {
               var medicationToDoRelated =await _medicationToDoListData.getMedicationToDoListRelatedByCmrMedicationId(id);

                var doctorMedications = await _doctorMedicationData.GetDoctorMedicationByCmrId(id);

                if (medicationToDoRelated != null)
                {
                     _medicationToDoListData.PatientDeleteMedicationToDoRelated(medicationToDoRelated);
                }
                if(doctorMedications != null)
                {
                    foreach(DoctorMedication doctorMedication in doctorMedications)
                    {
                        _doctorMedicationData.DeleteDoctorMedicationForCmr(doctorMedication);
                    }
                    
                }
                var cmrMedications = await _cmrMedicationData.GetUniqueCmrMedicationsByPatientId(cmrMedication.Patient.Id);
                if (cmrMedications.Count == 1)
                {
                    var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(cmrMedication.Patient.Id);
                    foreach (OtcMedication otcMedication in otcMedications)
                    {
                        otcMedication.IsCmrCreated = false;
                        await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    }
                }

                await _cmrMedicationData.DeleteCmrMedication(cmrMedication);
                response.Success = true;
                response.Message = "CmrMedication Deleted Successfully!";
                return response;

            }

        }

        public async Task<List<CmrMedication>> UpdateOtcMedication(int patientId)
        {
            var otcMedications = await _otcMedicationData.GetOtcMedicationsByPatientId(patientId);
            List<CmrMedication> cmrMedications = new List<CmrMedication>();
           
            foreach (OtcMedication otcMedication in otcMedications)
            {
                bool check = true;
                if (string.IsNullOrEmpty(otcMedication.SBDCName) || string.IsNullOrEmpty(otcMedication.Condition) || string.IsNullOrEmpty(otcMedication.Direction) || otcMedication.DoctorPrescribed == null)
                {
                    check = false;
                }

                if (!otcMedication.IsCmrCreated && check )
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

                    DoctorMedication doctorMedication = new DoctorMedication
                    {
                        DoctorPrescribed = cmrMedication.DoctorPrescribed,
                        CmrMedication = cmrMedication,
                    };

                    DoctorMedication doctorMedication1 = new DoctorMedication
                    {
                        DoctorPrescribed = otcMedication.DoctorPrescribed,
                        OtcMedication = otcMedication,
                    };


                    otcMedication.IsCmrCreated = true;
                    await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    await _cmrMedicationData.AddCmrMedication(cmrMedication);

                    await _doctorMedicationData.AddDoctorMedication(doctorMedication);
                    await _doctorMedicationData.AddDoctorMedication(doctorMedication1);
                    cmrMedications.Add(cmrMedication);

                }

                if (!otcMedication.IsRecCreated && check)
                {
                    MedicationReconciliation medicationReconciliation = new MedicationReconciliation()
                    {
                        DoctorPrescribed = otcMedication.DoctorPrescribed,
                        Patient = otcMedication.Patient,
                        SBDCName = otcMedication.SBDCName,
                        Direction = otcMedication.Direction,
                        Condition = otcMedication.Condition
                    };
                    otcMedication.IsRecCreated = true;
                    await _otcMedicationData.UpdateOtcMedication(otcMedication);
                    await _reconciliationData.AddReconciliationMedication(medicationReconciliation);

                }
                   
               
            }
            await _applicationDbcontext.SaveChangesAsync();
            return cmrMedications;
        }
    }
}
