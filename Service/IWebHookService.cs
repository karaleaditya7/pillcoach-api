using Newtonsoft.Json.Linq;
using OntrackDb.Dto;
using OntrackDb.Entities;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IWebHookService
    {
        Task<ImportData> DumpImportData(JObject payload, string status);
        Task<ImportData> UpdateImportData(ImportData importData);
        Task<Pharmacy> AddOrUpdatePharmacy(Pharmacy pharmacy);
        Task<Patient> AddOrUpdatePatient(Patient patient);
        Task<Medication> AddOrUpdateMedication(Medication medication);
        DateTime GetRefillDate(Medication medication);

        Task<Doctor> AddOrUpdateDoctor(Doctor doctor);
        Task<MedicationConsumption> AddMedicationConsumption(Medication medication);
        Task<DoctorPharmacy> AddDoctorPharmacy(DoctorPharmacy doctorPharmacy);
        Task<DoctorPharmacy> GetDoctorPharmacyByDoctorIdAndPharmacyId(int doctorId, int pharmacyId);
    }
}
