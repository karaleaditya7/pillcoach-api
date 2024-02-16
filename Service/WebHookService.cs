using Newtonsoft.Json.Linq;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class WebHookService: IWebHookService
    {
        private readonly IWebhookData _webhookData;
        private readonly IPharmacyData _pharmacyData;
        private readonly IPatientData _patientData;
        private readonly IMedicationData _medicationData;
        private readonly IMedicationConsumptionData _medicationConsumptionData;
        private readonly IMedicationConsumptionService _medicationConsumptionService;
        private readonly IDoctorData _doctorData;
        private readonly INdcApiService _ndcApiService;
        public WebHookService(IWebhookData webhookData, IPharmacyData pharmacyData, IPatientData patientData, IMedicationData medicationData, IDoctorData doctorData, IMedicationConsumptionData medicationConsumptionData, IMedicationConsumptionService medicationConsumptionService, INdcApiService ndcApiService)
        {
            _webhookData = webhookData;
            _pharmacyData = pharmacyData;
            _patientData = patientData;
            _medicationData = medicationData;
            _doctorData = doctorData;
            _medicationConsumptionData = medicationConsumptionData;
            _ndcApiService = ndcApiService;
            _medicationConsumptionService = medicationConsumptionService;

        }

        public async Task<Medication> AddOrUpdateMedication(Medication medication)
        {
            Medication medicationExits = await _medicationData.GetMedicationByVendorRxID(medication.RxVendorRxID);
            Pdc_Medication pdc_Medication = await _medicationData.GetPdcMedicationWithNdcNumber(medication.NDCNumber);
            if (pdc_Medication != null)
            {
                if(pdc_Medication.category == "Statins")
                {
                    medication.Condition = "Cholesterol";
                }
                else
                {
                    medication.Condition = pdc_Medication.category;
                }

                if(pdc_Medication.value_set_item != null)
                {
                medication.DrugSubGroup = pdc_Medication.value_set_item;
                }
                else
                {
                medication.DrugSubGroup = pdc_Medication.value_set_subgroup;
                }
                    
    
            }
            else
            {
                medication.DrugSubGroup = medication.DrugName;
            }
            if(medicationExits == null)
            {
                
                medication.NextFillDate = GetRefillDate(medication);
                var result = await _medicationData.AddMedication(medication);
                return result;
            }

 
            ImportData importData = await _webhookData.GetImportDataById(medication.ImportData.id);
            medicationExits.ImportData = importData;
            medicationExits.RfNumber = medication.RfNumber;
            medicationExits.RxDate = medication.RxDate;
            medicationExits.Direction = medication.Direction;
            medicationExits.DrugName = medication.DrugName;
            medicationExits.Quantity = medication.Quantity;
            medicationExits.Supply = medication.Supply;
            medicationExits.PrescriberName = medication.PrescriberName;
            medicationExits.LastFillDate = medication.LastFillDate;
            medicationExits.RxNumber = medication.RxNumber;
            medicationExits.RefillsRemaining = medication.RefillsRemaining;
            medicationExits.NDCNumber = medication.NDCNumber;
            medicationExits.NextFillDate = GetRefillDate(medicationExits);
            medicationExits.Condition = medication.Condition;
            medicationExits.DrugSubGroup = medication.DrugSubGroup;

            var resultUp = _medicationData.UpdateMedicationWebhook(medicationExits);
            return medicationExits;
        }

        public async Task<MedicationConsumption> AddMedicationConsumption(Medication medication)
        {
            var recentMedicationConsumption = new MedicationConsumption();
            Pdc_Medication pdc_medication = new Pdc_Medication();
            pdc_medication = await _medicationData.GetPdcMedicationWithNdcNumber(medication.NDCNumber);
            if (pdc_medication == null)
            {
                pdc_medication = await _ndcApiService.ForPdcMedicationAddition(medication.NDCNumber);

                recentMedicationConsumption = await _medicationConsumptionData.GetRecentMedicationConsumptionByGenericDrugNameAndPatientId(pdc_medication.value_set_subgroup, medication.Patient.Id);
            }else
            {
                recentMedicationConsumption = await _medicationConsumptionData.GetRecentMedicationConsumptionByGenericDrugNameAndPatientId(pdc_medication.value_set_subgroup, medication.Patient.Id);
            }
            
            var medications = await _medicationData.GetDistintMedicationsByConditionAndPatientIdForEarlyMedicationPDC(medication.Condition, medication.Patient.Id);
            var condition = "";
            if (pdc_medication.category == "Statins")
            {
                condition = "Cholesterol";
            }
            else
            {
                condition = pdc_medication.category;
            }
            if (medications.Count > 1)
            {
                medications.RemoveAt(medications.Count - 1);
                var medicationdb = medications.OrderByDescending(m => m.DrugSubGroup).FirstOrDefault();
                medications.Add(medication);
                if (medication.LastFillDate.CompareTo(medicationdb.NextFillDate) == 0)
                {
                    for (DateTime date = medicationdb.LastFillDate; date < medication.LastFillDate; date = date.AddDays(1))
                    {
                        MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id,medication.Condition);
                        medicationConsumption.Status = true;
                        var result = _medicationConsumptionData.Update(medicationConsumption);
                    }
                    for (DateTime date = medication.LastFillDate; date < medication.NextFillDate; date = date.AddDays(1))
                    {
                        MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                        if (medicationConsumption == null)
                        {
                            MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                            {
                                Condition = condition,
                                DrugSubGroup = pdc_medication.value_set_subgroup,
                                NDCNumber = medication.NDCNumber,
                                RxNumber = medication.RxNumber,
                                PatienId = medication.Patient.Id,
                                RxVendorRxID = medication.RxVendorRxID,
                                Date = date,
                                Status = true,
                            };
                            var result = await _medicationConsumptionData.Add(medicationConsumption1);
                        }

                    }

                }else if (medication.LastFillDate.CompareTo(medicationdb.NextFillDate) > 0)
                {
                    for (DateTime date = medicationdb.LastFillDate; date < medicationdb.NextFillDate; date = date.AddDays(1))
                    {
                        MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                        medicationConsumption.Status = true;
                        var result = _medicationConsumptionData.Update(medicationConsumption);
                    }
                    int count = 1;
                    for (DateTime date = medication.LastFillDate; date < medication.NextFillDate; date = date.AddDays(1))
                    {
                        if (count == 1)
                        {
                            MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                            if (medicationConsumption == null)
                            {
                                MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                {
                                    Condition = condition,
                                    DrugSubGroup = pdc_medication.value_set_subgroup,
                                    RxNumber = medication.RxNumber,
                                    NDCNumber = medication.NDCNumber,
                                    PatienId = medication.Patient.Id,
                                    RxVendorRxID = medication.RxVendorRxID,
                                    Date = date,
                                    Status = true,
                                };
                                var result = await _medicationConsumptionData.Add(medicationConsumption1);
                            }
                        }
                        else
                        {
                            MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                            if (medicationConsumption == null)
                            {
                                MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                {
                                    Condition = condition,
                                    DrugSubGroup = pdc_medication.value_set_subgroup,
                                    RxNumber = medication.RxNumber,
                                    NDCNumber = medication.NDCNumber,
                                    PatienId = medication.Patient.Id,
                                    RxVendorRxID = medication.RxVendorRxID,
                                    Date = date,
                                    Status = true,
                                };
                                var result = await _medicationConsumptionData.Add(medicationConsumption1);
                            }
                        }
                        count++;
                            

                    }
                }
                else
                {
                   
                    var medicationdbs = medications.OrderByDescending(m => m.DrugSubGroup).FirstOrDefault();
                    if (medication.LastFillDate.CompareTo(medicationdbs.LastFillDate) < 0)
                    {
                        var medis = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithGenericDrugName(medication.LastFillDate, medication.Patient.Id, medication.DrugSubGroup);
                        var med = await _medicationData.GetMedicationByVendorRxID(medication.RxVendorRxID);
                        if (medis.Count > 0)
                        {
                            var days = medis.Count;
                            medication.NextFillDate = med.NextFillDate.AddDays(days);
                          
                        }
                        medications.RemoveAt(medications.Count - 1);
                        var pdcMedicationDb = await _medicationData.GetPdcMedicationWithNdcNumber(medicationdb.NDCNumber);
                        if (pdcMedicationDb.value_set_subgroup.Equals(pdc_medication.value_set_subgroup))
                        {
                            
                            for (DateTime date = medication.LastFillDate; date < medication.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                                else
                                {
                                    medicationConsumption.Status = true;
                                    var result = _medicationConsumptionData.Update(medicationConsumption);
                                }
                            }
                        }
                        else
                        {

                            for (DateTime date = medicationdb.LastFillDate; date <= medication.LastFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                medicationConsumption.Status = true;
                                var result =  _medicationConsumptionData.Update(medicationConsumption);
                            }
                            for (DateTime date = medication.LastFillDate; date < medication.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                            }
                        }
                    }
                    else if(medication.NextFillDate.CompareTo(medicationdbs.LastFillDate) < 0)
                    {
                        var pdcMedicationDb = await _medicationData.GetPdcMedicationWithNdcNumber(medicationdb.NDCNumber);
                        if (pdcMedicationDb.value_set_subgroup.Equals(pdc_medication.value_set_subgroup))
                        {
                            var medicationdbss = medications[medications.Count - 2];
                            var med = await _medicationData.GetMedicationByVendorRxID(medication.RxVendorRxID);
                            var days = medicationdbss.NextFillDate.Subtract(medication.LastFillDate).Days;
                            med.NextFillDate = med.NextFillDate.AddDays(days);
                            _medicationData.UpdateMedicationWebhook(med);
                            for (DateTime date = medicationdbss.LastFillDate; date <= medication.LastFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption != null)
                                {
                                    medicationConsumption.Status = true;
                                    var result =  _medicationConsumptionData.Update(medicationConsumption);
                                }
                            }
                            for (DateTime date = medication.LastFillDate.AddDays(1); date < med.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                            }
                        }
                        else
                        {

                            for (DateTime date = medicationdb.LastFillDate; date <= medication.LastFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                medicationConsumption.Status = true;
                                var result = _medicationConsumptionData.Update(medicationConsumption);
                            }
                            for (DateTime date = medication.LastFillDate.AddDays(1); date < medication.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                            }
                        }
                    }
                    else
                    {
                        var pdcMedicationDb = await _medicationData.GetPdcMedicationWithNdcNumber(medicationdb.NDCNumber);
                        if (pdcMedicationDb.value_set_subgroup.Equals(pdc_medication.value_set_subgroup))
                        {
                            var medicationdbss = medications[medications.Count - 2];
                            var med = await _medicationData.GetMedicationByVendorRxID(medication.RxVendorRxID);
                            var days = medicationdbss.NextFillDate.Subtract(medication.LastFillDate).Days;
                            med.NextFillDate = med.NextFillDate.AddDays(days);
                            _medicationData.UpdateMedicationWebhook(med);
                            for (DateTime date = medicationdbss.LastFillDate; date <= medication.LastFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption != null)
                                {
                                    medicationConsumption.Status = true;
                                    var result = _medicationConsumptionData.Update(medicationConsumption);
                                }
                            }
                            for (DateTime date = medication.LastFillDate.AddDays(1); date < med.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                            }
                        }
                        else
                        {

                            for (DateTime date = medicationdb.LastFillDate; date <= medication.LastFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                medicationConsumption.Status = true;
                                var result =  _medicationConsumptionData.Update(medicationConsumption);
                            }
                            for (DateTime date = medication.LastFillDate.AddDays(1); date < medication.NextFillDate; date = date.AddDays(1))
                            {
                                MedicationConsumption medicationConsumption = await _medicationConsumptionData.GetMedicationConsumptionByDateAndPatientIdWithCondition(date, medication.Patient.Id, medication.Condition);
                                if (medicationConsumption == null)
                                {
                                    MedicationConsumption medicationConsumption1 = new MedicationConsumption()
                                    {
                                        Condition = condition,
                                        DrugSubGroup = pdc_medication.value_set_subgroup,
                                        RxNumber = medication.RxNumber,
                                        NDCNumber = medication.NDCNumber,
                                        PatienId = medication.Patient.Id,
                                        RxVendorRxID = medication.RxVendorRxID,
                                        Date = date,
                                        Status = true,
                                    };
                                    var result = await _medicationConsumptionData.Add(medicationConsumption1);
                                }
                            }
                        }
                    }
                    
                        
                }

                
                
                
                
            }
            else
            {
                for (DateTime date = medication.LastFillDate; date < medication.NextFillDate; date = date.AddDays(1))
                {
                    
                    MedicationConsumption medicationConsumption = new MedicationConsumption()
                    {
                        Condition = condition,
                        DrugSubGroup = pdc_medication.value_set_subgroup,
                        NDCNumber = medication.NDCNumber,
                        PatienId = medication.Patient.Id,
                        RxVendorRxID = medication.RxVendorRxID,
                        Date = date,
                        Status = true,
                    };
                    var result = await _medicationConsumptionData.Add(medicationConsumption);
                }
            }
            return null;
        }

        public async Task<Doctor> AddOrUpdateDoctor(Doctor doctor)
        {
            Doctor doctorExists = await _doctorData.GetDoctorByNPIID(doctor.Npi);
            if (doctorExists == null)
            {
                var result = await _doctorData.AddDoctor(doctor);
                return result;
            }
    
            ImportData importData = await _webhookData.GetImportDataById(doctor.ImportData.id);
            doctorExists.ImportData = importData;
            doctorExists.Contact.Fax = doctor.Contact.Fax;
            doctorExists.Contact.DoB = doctor.Contact.DoB;
            doctorExists.Contact.PrimaryEmail = doctor.Contact.PrimaryEmail;
            doctorExists.Contact.SecondaryEmail = doctor.Contact.SecondaryEmail;
            doctorExists.Contact.PrimaryPhone = doctor.Contact.PrimaryPhone;
            doctorExists.Contact.SecondaryPhone = doctor.Contact.SecondaryPhone;
            doctorExists.Npi = doctor.Npi;
            _doctorData.UpdateDoctorWebhook(doctorExists);
            return doctorExists;
        }

        public async Task<Patient> AddOrUpdatePatient(Patient patient)
        {
            Patient patientExists = await _patientData.GetPatientByPatientVendorRxID(patient.PatientVendorRxID);
            if(patientExists == null)
            {
                Note note = new Note();
                note.text = "";
                note.LastUpdated = DateTime.Now;
                patient.Note = note;
                var result = await _patientData.AddPatient(patient);
                return result;
            }
            Pharmacy pharmacy = await _pharmacyData.GetPharmacyById(patient.Pharmacy.Id);
           
            patientExists.Pharmacy = pharmacy;
            
            ImportData importData = await _webhookData.GetImportDataById(patient.ImportData.id);
            patientExists.ImportData = importData;
            //patientExists.Contact.Fax = patient.Contact.Fax;
            //patientExists.Contact.DoB = patient.Contact.DoB;
            //patientExists.Contact.PrimaryEmail = patient.Contact.PrimaryEmail;
            //patientExists.Contact.SecondaryEmail = patient.Contact.SecondaryEmail;
            //patientExists.Contact.PrimaryPhone = patient.Contact.PrimaryPhone;
            //patientExists.Contact.SecondaryPhone = patient.Contact.SecondaryPhone;
            //patientExists.Address.City = patient.Address.City;
            //patientExists.Address.State = patient.Address.State;
            //patientExists.Address.ZipCode = patient.Address.ZipCode;
            //patientExists.Address.AddressLineOne = patient.Address.AddressLineOne;
            //patientExists.Address.AddressLineTwo = patient.Address.AddressLineTwo;
            var resultUp =  _patientData.UpdatePatientWebhook(patientExists);
            return patientExists;
        }

        public async Task<Pharmacy> AddOrUpdatePharmacy(Pharmacy pharmacy)
        {
            Pharmacy pharmacyExists = await _pharmacyData.GetPharmacyByNcpdpNumber(pharmacy.NcpdpNumber);
            if(pharmacyExists == null)
            {
                Note note = new Note();
                note.text = "";
                note.LastUpdated = DateTime.Now;
                pharmacy.Note = note;
                var result = await _pharmacyData.AddPharmacy(pharmacy);
                return result;
            }
            pharmacyExists.Name = pharmacy.Name;
            pharmacyExists.LastUpdate = pharmacy.LastUpdate;
            pharmacyExists.NcpdpNumber = pharmacy.NcpdpNumber;
            pharmacyExists.Contact.Fax = pharmacy.Contact.Fax;
            pharmacyExists.Contact.DoB = pharmacy.Contact.DoB;
            pharmacyExists.Contact.PrimaryEmail = pharmacy.Contact.PrimaryEmail;
            pharmacyExists.Contact.SecondaryEmail = pharmacy.Contact.SecondaryEmail;
            pharmacyExists.Contact.PrimaryPhone = pharmacy.Contact.PrimaryPhone;
            pharmacyExists.Contact.SecondaryPhone = pharmacy.Contact.SecondaryPhone;
            pharmacyExists.Address.City = pharmacy.Address.City;
            pharmacyExists.Address.State = pharmacy.Address.State;
            pharmacyExists.Address.ZipCode = pharmacy.Address.ZipCode;
            pharmacyExists.Address.AddressLineOne = pharmacy.Address.AddressLineOne;
            pharmacyExists.Address.AddressLineTwo = pharmacy.Address.AddressLineTwo;
            pharmacyExists.PharmacyManager = pharmacy.PharmacyManager;
            ImportData importData = await _webhookData.GetImportDataById(pharmacy.ImportData.id);
            pharmacyExists.ImportData = importData;
            var resultUp = _pharmacyData.UpdatePharmacyWebhook(pharmacyExists);
            return pharmacyExists;
        }

        public async Task<ImportData> DumpImportData(JObject payload, string status)
        {
           
            ImportData importData = new ImportData();
            importData.status = status;
            importData.message = "In Progress";
            importData.data = payload.ToString();
            importData.created_datetime = DateTime.Now;
            ImportData importDataDb = await _webhookData.DumpImportData(importData);
            
            return importDataDb;
        }
        public async Task<ImportData> UpdateImportData(ImportData importData)
        {
            ImportData importDataDb = await _webhookData.UpdateImportData(importData);
            return importDataDb;
        }
        public DateTime GetRefillDate(Medication medication)
        {
            int supply = medication.Supply;
            DateTime lastFillDate = medication.LastFillDate;
            DateTime nextFillDate = lastFillDate.AddDays(supply);

            return nextFillDate;
        }

        public async Task<DoctorPharmacy> AddDoctorPharmacy(DoctorPharmacy doctorPharmacy) {

            var result = await _doctorData.AddDoctorPharmacy(doctorPharmacy);
            return result;
        }
        public async Task<DoctorPharmacy> GetDoctorPharmacyByDoctorIdAndPharmacyId(int doctorId, int pharmacyId) {

            var result = await _doctorData.GetDoctorPharmacyByDoctorIdAndPharmacyId(doctorId,pharmacyId);
            return result;
        }


    }
}
