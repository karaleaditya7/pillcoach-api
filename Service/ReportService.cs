using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Helper;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public class ReportService : IReportService
    {
        private readonly IPharmacyData _pharmacyData;
        private readonly IPharmacyService _pharmacyService;
        private readonly IPatientService _patientService;
        private readonly IMedicationService _medicationService;
        private readonly IMedicationData _medicationData;

        public ReportService(IPharmacyData pharmacyData, IPharmacyService pharmacyService, IPatientService patientService, IMedicationService medicationService,IMedicationData medicationData)
        {
            _pharmacyData = pharmacyData;
            _pharmacyService = pharmacyService;
            _patientService = patientService;
            _medicationService = medicationService;
            _medicationData = medicationData;
        }



        
        public async Task<char[]> GetPharmacyCSVData(List<string> pharmacyIds, int month)
        {
            DateTime endDate = DateTime.Now;

            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            List<Pharmacy> pharmacyList =  _pharmacyData.GetPharmacyCSVData(pharmacyIds);
            for (int i = 0; i < pharmacyList.Count; i++)
            {
                if (pharmacyList[i] != null)
                {
                   PharmacyDto pharmacyListDto = await _pharmacyService.CalucalateAvergaePdcForPharmacy(pharmacyList[i].Id, startDate, endDate,month);
                    pharmacyList[i].CholestrolPDC = pharmacyListDto.CholestrolPDC;
                    pharmacyList[i].RASAPDC = pharmacyListDto.RASAPDC;
                    pharmacyList[i].DiabetesPDC = pharmacyListDto.DiabetesPDC;  
                    var patients = await _patientService.GetPatientsByPharmacyId(pharmacyList[i].Id);
                    pharmacyList[i].NewPatient =  _patientService.countNewPatient(patients.Data);
                    List<Medication> medications = await _medicationData.getAllMedicationsByPharmacyID(pharmacyList[i].Id);
                    pharmacyList[i].UpcomingRefill = await _medicationService.countDueForRefill(medications);
                }
                
            }
            List<object> pharmacies = new List<object>();
            foreach(var pharmacy in pharmacyList)
            {
                if (pharmacy != null)
                {
                    pharmacies.Add(new object[] {
                    pharmacy.Id,
                    pharmacy.Name,
                    pharmacy.PharmacyManager,
                    pharmacy.NcpdpNumber,
                    pharmacy.NpiNumber,
                    pharmacy.CholestrolPDC,
                    pharmacy.DiabetesPDC,
                    pharmacy.RASAPDC,
                    pharmacy.NewPatient,
                    pharmacy.UpcomingRefill
                });
                }
                

            }

            StringBuilder sb = new StringBuilder();
            var dateRangeRow = new string[] { "Run Date Range ( "+ startDate.Date + " - " + endDate.Date + " )" };
           
            foreach (var date in dateRangeRow)
            {
                sb.Append(date);
            }
            sb.Append("\r\n");
            var names = new string[] {
                "PharmacyId","PharmacyName","PharmacyManager","NcpdpNumber",
                "NpiNumber","CholestrolPDC","DiabetesPDC","RASAPDC","NewPatient","UpcomingRefill"
            };
            foreach (var name in names)
            {
                sb.Append(name + ',');
            }
            sb.Append("\r\n");
            foreach (object item in pharmacies)
            {
                object[] arrStudents = (object[])item;
                foreach (var data in arrStudents)
                {
                    if(data==null)
                    {
                        sb.Append("null" + ',');
                    }
                    else
                    {
                        sb.Append(data.ToString() + ',');
                    }    
                }
                sb.Append("\r\n");
            }
            return sb.ToString().ToArray();
        }



        public async Task<List<ReportsModel>> GetPharmacyListByPharmacyIdsAndUserId(List<string> pharmacyIds,int month)
        {
            DateTime endDate = DateTime.Now;

            DateTime startDate = UtilityHelper.GetStartDateByMonth(endDate, month);
            List<Pharmacy> pharmacyList =  _pharmacyData.GetPharmacyListByPharmacyIdsAndUserId(pharmacyIds);
            List<ReportsModel> reports = new List<ReportsModel>();

            for (int i = 0; i < pharmacyList.Count; i++)
            {
                if (pharmacyList[i] != null)
                {
                    ReportsModel repModel = new ReportsModel();
                    repModel.PharmacyId = pharmacyList[i].Id;
                    repModel.PharmacyName = pharmacyList[i].Name;
                    repModel.PharmacyManager = pharmacyList[i].PharmacyManager;
                    repModel.NcpdpNumber = pharmacyList[i].NcpdpNumber;
                    repModel.NpiNumber = pharmacyList[i].NpiNumber;
                    PharmacyDto pharmacyListDto = await _pharmacyService.CalucalateAvergaePdcForPharmacy(pharmacyList[i].Id, startDate, endDate, month);
                    pharmacyList[i].CholestrolPDC = pharmacyListDto.CholestrolPDC;
                    pharmacyList[i].RASAPDC = pharmacyListDto.RASAPDC;
                    pharmacyList[i].DiabetesPDC = pharmacyListDto.DiabetesPDC;
                    pharmacyList[i].NewPatient =  _patientService.countNewPatient(pharmacyList[i].Patients);

                    List<Medication> medications = await _medicationData.getAllMedicationsByPharmacyID(pharmacyList[i].Id);
                    pharmacyList[i].UpcomingRefill = await _medicationService.countDueForRefill(medications);
                    repModel.UpcomingRefill = pharmacyList[i].UpcomingRefill;
                    repModel.CholestrolPDC = pharmacyList[i].CholestrolPDC;
                    repModel.RASAPDC = pharmacyList[i].RASAPDC;
                    repModel.DiabetesPDC = pharmacyList[i].DiabetesPDC;
                    repModel.NewPatient = pharmacyList[i].NewPatient;
                    pharmacyList[i].Patients = null;
                    pharmacyList[i].Address = null;
                    pharmacyList[i].Contact = null;
                    pharmacyList[i].ImageName = null;
                    pharmacyList[i].NoteId = null;
                    reports.Add(repModel);
                }
                
            }
            return reports;
        }



    }


}
