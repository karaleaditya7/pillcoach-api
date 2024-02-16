using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public class MedicationToDoListData :IMedicationToDoListData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public MedicationToDoListData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;   
        }

        public async Task<MedicationToDoRelated> AddMedicationToDoListRelated(MedicationToDoRelated medicationToDoRelated)
        {
            var result = await _applicationDbcontext.MedicationToDoRelateds.AddAsync(medicationToDoRelated);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<MedicationToDoRelated> UpdateMedicationToDoListRelated(MedicationToDoRelated medicationToDoRelated)
        {
            var result =  _applicationDbcontext.MedicationToDoRelateds.Update(medicationToDoRelated);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<NonRelatedMedicationToDo> UpdateNonMedicationToDoListRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo)
        {
            var result = _applicationDbcontext.NonRelatedMedicationToDos.Update(nonRelatedMedicationToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<NonRelatedMedicationToDo> AddNonMedicationRelatedToDo(NonRelatedMedicationToDo nonRelatedMedicationToDo)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.AddAsync(nonRelatedMedicationToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ActionItemToDo> AddActionItemToDo(ActionItemToDo actionItemToDo)
        {
            var result = await _applicationDbcontext.ActionItemToDos.AddAsync(actionItemToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ActionItemToDo> GetActionitem(string description)
        {
            var actionItemToDo = await _applicationDbcontext.ActionItemToDos.Where(m => m.Description == description).
                                            SingleOrDefaultAsync();

            return actionItemToDo;
        }

        public async Task<ActionItemToDo> GetActionitemByPatientId(string description, int patientId)
        {
            var actionItemToDo = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.ActionItemToDo.Description == description && m.Patient.Id == patientId).Select(m=>m.ActionItemToDo).
                                            SingleOrDefaultAsync();

            return actionItemToDo;
        }

        public async Task<List<CmrMedication>> getAllCmrMedicationRelated(int patientId)
        {
          
            var response = await _applicationDbcontext.MedicationToDoRelateds.Where(m => m.Patient.Id == patientId).Select(a => a.CmrMedication).ToListAsync();
                                            
            return response;
        }

        public async Task<List<NonRelatedMedicationToDo>> getCmrNonMedicationRelated(int patientId)
        {
          
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Include(a => a.Patient).Include(a => a.ActionItemToDo).Where(m => m.Patient.Id == patientId).ToListAsync();

             return result;           
        }

        public async Task<List<ActionItemToDo>> getAllActionItemToDo(int patientId)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.Patient.Id == patientId).Select(m => m.ActionItemToDo).ToListAsync();

            return result;
        }

        public async Task<MedicationToDoRelated> getMedicationToDoListRelatedByCmrMedicationId(int cmrMedicationId)
        {
            var result = await _applicationDbcontext.MedicationToDoRelateds.Include(m => m.CmrMedication).Include(m => m.Patient).Where(m => m.CmrMedication.Id  == cmrMedicationId).FirstOrDefaultAsync();
            return result;
        }
        public async Task<MedicationToDoRelated> getMedicationToDoListRelatedById(int medicationToDoRelatedId)
        {
            var result = await _applicationDbcontext.MedicationToDoRelateds.Where(m => m.Id == medicationToDoRelatedId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedById(int medicationToDoListNonRelatedId)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.Id == medicationToDoListNonRelatedId).FirstOrDefaultAsync();
            return result;
        }


        public async Task DeleteMedicationToDoRelated(MedicationToDoRelated medicationToDoRelated)
        {

            _applicationDbcontext.MedicationToDoRelateds.Remove(medicationToDoRelated);
           
            await _applicationDbcontext.SaveChangesAsync();
            
        }
        public void PatientDeleteMedicationToDoRelated(MedicationToDoRelated medicationToDoRelated)
        {

            _applicationDbcontext.MedicationToDoRelateds.Remove(medicationToDoRelated);

        }
        public void PatientDeleteNonMedicationToDoRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo)
        {

            _applicationDbcontext.NonRelatedMedicationToDos.Remove(nonRelatedMedicationToDo);

        }
        public async Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedByActionitemToDoId(int actionitemToDoId)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.ActionItemToDo.Id == actionitemToDoId).FirstOrDefaultAsync();
            return result;
        }

        public async Task DeleteNonMedicationToDoListRelated(NonRelatedMedicationToDo nonRelatedMedicationToDo)
        {

            _applicationDbcontext.NonRelatedMedicationToDos.Remove(nonRelatedMedicationToDo);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<NonRelatedMedicationToDo> getNonMedicationToDoListRelatedByActionitemToDoIdPatientId(int actionitemToDoId , int patientId)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.ActionItemToDo.Id == actionitemToDoId && m.Patient.Id == patientId).FirstOrDefaultAsync();
            return result;
        }
        public async Task<MedicationToDoRelated> getMedicationToDoListRelatedByCmrMedicationIdByPatientId(int cmrMedicationId , int patientId)
        {
            var result = await _applicationDbcontext.MedicationToDoRelateds.Where(m => m.CmrMedication.Id == cmrMedicationId && m.Patient.Id == patientId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<MedicationToDoRelated>> GetMedicationByPatientId(int patientId)
        {

            return await _applicationDbcontext.MedicationToDoRelateds.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public async Task<List<NonRelatedMedicationToDo>> GetNonMedicationByPatientId(int patientId)
        {

            return await _applicationDbcontext.NonRelatedMedicationToDos.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void DeleteNonMedicationToDoListRelatedForServiceTakeAwayInformaction(NonRelatedMedicationToDo nonRelatedMedicationToDo)
        {
            _applicationDbcontext.NonRelatedMedicationToDos.Remove(nonRelatedMedicationToDo);

        }
        public void DeleteMedicationToDoRelatedForServiceTakeAwayInformaction(MedicationToDoRelated medicationToDoRelated)
        {
            _applicationDbcontext.MedicationToDoRelateds.Remove(medicationToDoRelated);
        }

        public async Task<List<MedicationToDoRelated>> getMedicationToDoRelatedsbyPatientId( int patientId)
        {
            var result = await _applicationDbcontext.MedicationToDoRelateds.Include(m=>m.CmrMedication).Where(m => m.Patient.Id == patientId).ToListAsync();
            return result;
        }
        public async Task<List<NonRelatedMedicationToDo>> getNonMedicationToDoRelatedsbyPatientId(int patientId)
        {
            var result = await _applicationDbcontext.NonRelatedMedicationToDos.Where(m => m.Patient.Id == patientId).ToListAsync();
            return result;
        }
    }
}
