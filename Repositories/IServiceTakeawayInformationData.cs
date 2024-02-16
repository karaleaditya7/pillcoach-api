using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace OntrackDb.Repositories
{
    public interface IServiceTakeawayInformationData
    {
        Task<ServiceTakeawayInformation> GetServiceTakeawayInformationByPatientId(int patientId);
        Task<ServiceTakeawayInformation> AddServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation);
        Task<ServiceTakeawayInformation> GetServiceTakeawayInformationById(int serviceTakeawayInformationId);
        Task<Address> GetAddressById(int id);
        Task<Contact> GetContactById(int id);
        Task<ServiceTakeawayInformation> UpdateServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation);
        Task DeleteServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation);
        Task<CmrVaccine> AddCmrVaccine(CmrVaccine cmrVaccine);

        Task<List<CmrVaccine>> GetCmrVaccineByServiceTakeawayInformationById(int Id);
        Task<CmrVaccine> GetCmrVaccineByName(string cmrVaccine, int serviceTakeawayInformationId);

        Task<Patient> PatientVerificationForPdf(string lastname, DateTime dob, int patientId);
        void DeleteServiceTakeawayInformationForPatient(ServiceTakeawayInformation serviceTakeawayInformation);

        Task<CmrPatient> GetCmrPatientById(int cmrPatientId);
        Task<CmrPatient> GetCmrPatientByPatientId(int patientId);
        Task<CmrPatient> AddCmrPatient(CmrPatient cmrPatient);
        Task<List<CmrVaccine>> GetCmrVaccineByPatientId(int patientId);
        void PatientDeleteForCmrVaccine(CmrVaccine cmrVaccine);

        void DeleteServiceTakeawayInformationCmrPatient(CmrPatient cmrPatient);

        Task<Appointment> GetDeleteAppoimentByServiceTakeawayInformationById(int Id);

        Task<Contact> AddContact(Contact contact);
        Task<Address> AddAddress(Address address);
        void DeleteCmrVaccine(CmrVaccine CmrVaccine);
        Task<TakeawayVerify> GetTakeawayVerifyByPatientId(int patientId);
        Task<TakeawayVerify> UpdateTakeawayVerify(TakeawayVerify takeawayVerify);
        Task<TakeawayVerify> GetTakeawayVerifyByPatientIdAndUUID(int patientId,string uuid);
        Task DeleteTakeawayVerify(TakeawayVerify takeawayVerify);
        Task<TakeawayVerify> AddTakeawayVerify(TakeawayVerify takeawayVerify);
        Task<int> UpdateCmrPatient(CmrPatient cmrPatient);
    }
}