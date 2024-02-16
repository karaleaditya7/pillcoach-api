using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace OntrackDb.Repositories
{
    public class ServiceTakeAwayMedReconciliationData : IServiceTakeAwayMedReconciliationData
    {
        private readonly ApplicationDbContext _applicationDbcontext;

        public ServiceTakeAwayMedReconciliationData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }

        public async Task<ServiceTakeAwayMedReconciliation> GetServiceTakeAwayMedReconciliationByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.ServiceTakeAwayMedReconciliations.
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
                Include(s => s.Appointment)
            .SingleOrDefaultAsync(s => s.Patient.Id == patientId);
            return result;
        }

        public async Task<CmrPatient> GetCmrPatientById(int cmePatientId)
        {
            var result = await _applicationDbcontext.CmrPatients.Include(s => s.Contact).Include(s => s.Address).Where(s => s.Id == cmePatientId && s.IsMedRecType)
            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<CmrPatient> AddCmrPatient(CmrPatient cmrPatient)
        {
            var result = await _applicationDbcontext.CmrPatients.AddAsync(cmrPatient);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ServiceTakeAwayMedReconciliation> AddServiceTakeawayReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation)
        {
            var result = await _applicationDbcontext.ServiceTakeAwayMedReconciliations.AddAsync(serviceTakeAwayMedReconciliation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ServiceTakeAwayMedReconciliation> GetServiceTakeawayReconciliationById(int serviceTakeawaymedreconciliationId)
        {
            var result = await _applicationDbcontext.ServiceTakeAwayMedReconciliations.Include(s => s.Patient).Include(s => s.CmrPatient).Include(s => s.CmrPatient.Contact).Include(s => s.CmrPatient.Address).Include(s => s.Patient.Contact).Include(s => s.Patient.Address).Include(s => s.Contact).Include(s => s.Address).Include(s => s.Appointment).Where(s => s.Id == serviceTakeawaymedreconciliationId)
            .SingleOrDefaultAsync();
            return result;
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

        public async Task<List<VaccineReconciliation>> GetVaccineRecByServiceTakeawayRecById(int Id)
        {
            var result = await _applicationDbcontext.VaccineReconciliations.Include(c => c.ServiceTakeAwayMedReconciliation).Where(s => s.ServiceTakeAwayMedReconciliation.Id == Id).ToListAsync();

            return result;
        }

        public void PatientDeleteForVaccineRec(VaccineReconciliation vaccineReconciliation)
        {
            _applicationDbcontext.VaccineReconciliations.Remove(vaccineReconciliation);

        }

        public async Task<VaccineReconciliation> AddCmrVaccine(VaccineReconciliation cmrVaccineReconciliation)
        {
            var result = await _applicationDbcontext.VaccineReconciliations.AddAsync(cmrVaccineReconciliation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ServiceTakeAwayMedReconciliation> UpdateServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation)
        {
            var result = _applicationDbcontext.Update(serviceTakeAwayMedReconciliation);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ServiceTakeAwayMedReconciliation> GetServiceTakeawayMedReconciliationById(int serviceTakeawayInformationId)
        {
            var result = await _applicationDbcontext.ServiceTakeAwayMedReconciliations.Include(s => s.Patient).Include(s => s.CmrPatient).Include(s => s.CmrPatient.Contact).Include(s => s.CmrPatient.Address).Include(s => s.Patient.Contact).Include(s => s.Patient.Address).Include(s => s.Contact).Include(s => s.Address).Include(s => s.Appointment).Where(s => s.Id == serviceTakeawayInformationId)
            .SingleOrDefaultAsync();
            return result;
        }

        public void DeleteServiceTakeawayMedReconciliationCmrPatient(CmrPatient cmrPatient)
        {

            _applicationDbcontext.CmrPatients.Remove(cmrPatient);

        }

        public async Task DeleteServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation)
        {

            _applicationDbcontext.ServiceTakeAwayMedReconciliations.Remove(serviceTakeAwayMedReconciliation);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<List<CmrVaccine>> GetCmrVaccineByServiceTakeawayMedReconciliationById(int Id)
        {
            var result = await _applicationDbcontext.CmrVaccines.Include(c => c.ServiceTakeawayInformation).Where(s => s.ServiceTakeawayInformation.Id == Id).ToListAsync();

            return result;
        }

        public async Task<TakeawayVerify> GetTakeawayVerifyByPatientId(int patientId)
        {

            return await _applicationDbcontext.TakeawayVerify.Where(t => t.PatientId == patientId && t.IsServiceTakeAwayMedRec).FirstOrDefaultAsync();

        }
        public async Task<TakeawayVerify> UpdateTakeawayVerify(TakeawayVerify takeawayVerify)
        {
            var result = _applicationDbcontext.TakeawayVerify.Update(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<TakeawayVerify> AddTakeawayVerify(TakeawayVerify takeawayVerify)
        {
            var result = await _applicationDbcontext.TakeawayVerify.AddAsync(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<TakeawayVerify> GetTakeawayVerifyByPatientIdAndUUID(int patientId, string uuid)
        {

            return await _applicationDbcontext.TakeawayVerify.Where(t => t.PatientId == patientId && t.UUID == uuid && t.IsServiceTakeAwayMedRec).FirstOrDefaultAsync();

        }

        public async Task DeleteTakeawayVerify(TakeawayVerify takeawayVerify)
        {

            _applicationDbcontext.TakeawayVerify.Remove(takeawayVerify);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<Patient> PatientVerificationForPdf(string lastname, DateTime dob, int patientId)
        {
            var result = await _applicationDbcontext.Patients.Where(s => s.Contact.DoB.Date == dob && s.Contact.LastName == lastname && s.Id == patientId)
                .SingleOrDefaultAsync();

            return result;
        }

        public void DeleteServiceTakeawayMedRecForPatient(ServiceTakeAwayMedReconciliation serviceTakeAwayMedReconciliation)
        {

            _applicationDbcontext.ServiceTakeAwayMedReconciliations.Remove(serviceTakeAwayMedReconciliation);

        }

        public async Task<List<VaccineReconciliation>> GetMedRecVaccineReconciliationByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.VaccineReconciliations.Where(s => s.ServiceTakeAwayMedReconciliation.Patient.Id == patientId).ToListAsync();

            return result;
        }

        public void  PatientDeleteForMedRecVaccineReconciliation(VaccineReconciliation vaccineReconciliation)
        {
            _applicationDbcontext.VaccineReconciliations.Remove(vaccineReconciliation);

        }

    }
}
