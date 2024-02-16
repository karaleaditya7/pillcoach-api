using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class ServiceTakeawayInformationData: IServiceTakeawayInformationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public ServiceTakeawayInformationData(ApplicationDbContext applicationDbcontext)
        {
                _applicationDbcontext = applicationDbcontext;   
        }

        public async Task<ServiceTakeawayInformation> GetServiceTakeawayInformationByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.ServiceTakeawayInformations.
               Include(s => s.CmrPatient).
               Include(s => s.CmrPatient.Contact).
               Include(s => s.CmrPatient.Address).
               Include(s => s.Patient).
               Include(s => s.Patient.Contact).
               Include(s => s.Patient.Address).
               Include(s => s.Patient.Pharmacy).
               Include(s => s.Patient.Pharmacy.Contact).
               Include(s => s.Patient.Pharmacy.Address).
                Include(s => s.Address).
                Include(s => s.Contact).
                Include(s=>s.Appointment)
            .SingleOrDefaultAsync(s => s.Patient.Id == patientId);
            return result;
        }

        public async Task<ServiceTakeawayInformation> GetServiceTakeawayInformationById(int serviceTakeawayInformationId)
        {
            var result = await _applicationDbcontext.ServiceTakeawayInformations.Include(s => s.Patient).Include(s => s.CmrPatient).Include(s => s.CmrPatient.Contact).Include(s => s.CmrPatient.Address).Include(s=>s.Patient.Contact).Include(s=>s.Patient.Address).Include(s => s.Contact).Include(s => s.Address).Include(s => s.Appointment).Where(s => s.Id == serviceTakeawayInformationId)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<CmrPatient> GetCmrPatientById(int cmrPatientId)
        {
            var result = await _applicationDbcontext.CmrPatients.Include(s => s.Contact).Include(s => s.Address).Where(s => s.Id == cmrPatientId && s.IsCmrType)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<int> UpdateCmrPatient(CmrPatient cmrPatient)
        {
            _applicationDbcontext.CmrPatients.Update(cmrPatient);
            var result = await _applicationDbcontext.SaveChangesAsync();
            return result;
        }
        public async Task<CmrPatient> GetCmrPatientByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.CmrPatients.Include(s => s.Contact).Include(s => s.Address).Where(s => s.PatientId == patientId)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<Address> GetAddressById(int id)
        {
            var result = await _applicationDbcontext.Address.Where(s => s.Id == id)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<Contact> GetContactById(int id)
        {
            var result = await _applicationDbcontext.Contacts.Where(s => s.Id == id)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<ServiceTakeawayInformation> UpdateServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation)
        {
            var result =  _applicationDbcontext.Update(serviceTakeawayInformation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<ServiceTakeawayInformation> AddServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation)
        {
            var result = await _applicationDbcontext.ServiceTakeawayInformations.AddAsync(serviceTakeawayInformation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task<CmrVaccine> AddCmrVaccine(CmrVaccine cmrVaccine)
        {
            var result = await _applicationDbcontext.CmrVaccines.AddAsync(cmrVaccine);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Address> AddAddress(Address address)
        {
            var result = await _applicationDbcontext.Address.AddAsync(address);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<Contact> AddContact(Contact contact)
        {
            var result = await _applicationDbcontext.Contacts.AddAsync(contact);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<CmrPatient> AddCmrPatient(CmrPatient cmrPatient)
        {
            var result = await _applicationDbcontext.CmrPatients.AddAsync(cmrPatient);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<TakeawayVerify> AddTakeawayVerify(TakeawayVerify takeawayVerify)
        {
            var result = await _applicationDbcontext.TakeawayVerify.AddAsync(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<TakeawayVerify> UpdateTakeawayVerify(TakeawayVerify takeawayVerify)
        {
            var result = _applicationDbcontext.TakeawayVerify.Update(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }


        public async Task DeleteServiceTakeawayInformation(ServiceTakeawayInformation serviceTakeawayInformation)
        {

            _applicationDbcontext.ServiceTakeawayInformations.Remove(serviceTakeawayInformation);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task DeleteTakeawayVerify(TakeawayVerify takeawayVerify)
        {

            _applicationDbcontext.TakeawayVerify.Remove(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<TakeawayVerify> GetTakeawayVerifyByPatientId(int patientId)
        {

            return await _applicationDbcontext.TakeawayVerify.Where(t => t.PatientId == patientId && t.IsServiceTakeAwayInfo).FirstOrDefaultAsync();

        }

        public async Task<TakeawayVerify> GetTakeawayVerifyByPatientIdAndUUID(int patientId, string uuid)
        {

            return await _applicationDbcontext.TakeawayVerify.Where(t => t.PatientId == patientId && t.UUID == uuid && t.IsServiceTakeAwayInfo).FirstOrDefaultAsync();

        }

        public void DeleteServiceTakeawayInformationForPatient(ServiceTakeawayInformation serviceTakeawayInformation)
        {

            _applicationDbcontext.ServiceTakeawayInformations.Remove(serviceTakeawayInformation);

        }
        public void DeleteCmrVaccine(CmrVaccine CmrVaccine)
        {

            _applicationDbcontext.CmrVaccines.Remove(CmrVaccine);

        }

        public void DeleteServiceTakeawayInformationCmrPatient(CmrPatient cmrPatient)
        {

            _applicationDbcontext.CmrPatients.Remove(cmrPatient);

        }
        public async Task<List<CmrVaccine>> GetCmrVaccineByServiceTakeawayInformationById(int Id)
        {
            var result = await _applicationDbcontext.CmrVaccines.Include(c => c.ServiceTakeawayInformation).Where(s => s.ServiceTakeawayInformation.Id == Id).ToListAsync();
            
            return result;
        }

        public async Task<CmrVaccine> GetCmrVaccineByName(string cmrVaccine, int serviceTakeawayInformationId)
        {
            var result = await _applicationDbcontext.CmrVaccines.Where(s => s.Name == cmrVaccine && s.ServiceTakeawayInformation.Id == serviceTakeawayInformationId).SingleOrDefaultAsync();

            return result;
        }

        public async Task<List<CmrVaccine>> GetCmrVaccineByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.CmrVaccines.Where(s =>  s.ServiceTakeawayInformation.Patient.Id==patientId ).ToListAsync();

            return result;
        }

        public void PatientDeleteForCmrVaccine(CmrVaccine cmrVaccine)
        {
            _applicationDbcontext.CmrVaccines.Remove(cmrVaccine);

        }

        public async Task<Patient> PatientVerificationForPdf(string lastname, DateTime dob , int patientId)
        {
            var result = await _applicationDbcontext.Patients.Where(s => s.Contact.DoB.Date == dob && s.Contact.LastName== lastname && s.Id == patientId)
                .SingleOrDefaultAsync();
            
            return result;
        }

        public async Task<Appointment> GetDeleteAppoimentByServiceTakeawayInformationById(int Id)
        {
            var result = await _applicationDbcontext.ServiceTakeawayInformations.Where(s => s.Id == Id).Select(s => s.Appointment).FirstOrDefaultAsync();

            return result;
        }

    }
}
