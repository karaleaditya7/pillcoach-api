using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IServiceTakeAwayMedReconciliationData
    {
        Task<ServiceTakeAwayMedReconciliation> GetServiceTakeAwayMedReconciliationByPatientId(int patientId);

        Task<CmrPatient> GetCmrPatientById(int cmePatientId);

        Task<CmrPatient> AddCmrPatient(CmrPatient cmrPatient);

        Task<ServiceTakeAwayMedReconciliation> AddServiceTakeawayReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation);

        Task<ServiceTakeAwayMedReconciliation> GetServiceTakeawayReconciliationById(int serviceTakeawaymedreconciliationId);

        Task<Address> AddAddress(Address address);

        Task<Contact> AddContact(Contact contact);

        Task<Address> GetAddressById(int id);

        Task<Contact> GetContactById(int id);

        Task<List<VaccineReconciliation>> GetVaccineRecByServiceTakeawayRecById(int Id);

        void PatientDeleteForVaccineRec(VaccineReconciliation vaccineReconciliation);

        Task<VaccineReconciliation> AddCmrVaccine(VaccineReconciliation cmrVaccineReconciliation);

        Task<ServiceTakeAwayMedReconciliation> UpdateServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation);

        Task<ServiceTakeAwayMedReconciliation> GetServiceTakeawayMedReconciliationById(int serviceTakeawayInformationId);

        void DeleteServiceTakeawayMedReconciliationCmrPatient(CmrPatient cmrPatient);

        Task DeleteServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation);

        Task<List<CmrVaccine>> GetCmrVaccineByServiceTakeawayMedReconciliationById(int Id);

        Task<TakeawayVerify> GetTakeawayVerifyByPatientId(int patientId);

        Task<TakeawayVerify> UpdateTakeawayVerify(TakeawayVerify takeawayVerify);

        Task<TakeawayVerify> AddTakeawayVerify(TakeawayVerify takeawayVerify);

        Task<TakeawayVerify> GetTakeawayVerifyByPatientIdAndUUID(int patientId, string uuid);

        Task DeleteTakeawayVerify(TakeawayVerify takeawayVerify);

        Task<Patient> PatientVerificationForPdf(string lastname, DateTime dob, int patientId);

        void DeleteServiceTakeawayMedRecForPatient(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation);

        Task<List<VaccineReconciliation>> GetMedRecVaccineReconciliationByPatientId(int patientId);

        void PatientDeleteForMedRecVaccineReconciliation(VaccineReconciliation vaccineReconciliation);
    }
}
