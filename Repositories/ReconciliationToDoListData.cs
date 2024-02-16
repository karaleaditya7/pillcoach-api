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
    public class ReconciliationToDoListData : IReconciliationToDoListData
    {
        private readonly ApplicationDbContext _applicationDbcontext;
        public ReconciliationToDoListData(ApplicationDbContext applicationDbcontext)
        {
            _applicationDbcontext = applicationDbcontext;
        }
        public async Task<ReconciliationToDoRelated> GetReconciliationToDoListRelatedByCmrMedicationId(int medicationRecocilationId)
        {
            var result = await _applicationDbcontext.ReconciliationToDoRelateds.Include(m => m.MedicationReconciliation).Include(m => m.Patient).Where(m => m.MedicationReconciliation.Id == medicationRecocilationId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<ReconciliationToDoRelated> AddReconciliationMedicationToDoListRelated(ReconciliationToDoRelated reconciliationToDoRelated)
        {
            var result = await _applicationDbcontext.ReconciliationToDoRelateds.AddAsync(reconciliationToDoRelated);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ReconciliationToDoRelated> UpdateReconciliationToDoListRelated(ReconciliationToDoRelated reconciliationToDoRelated)
        {
            var result = _applicationDbcontext.ReconciliationToDoRelateds.Update(reconciliationToDoRelated);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<NonRelatedRecocilationToDo> GetNonReconciliationToDoListRelatedById(int nonRelatedRecocilationToDoId)
        {
            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.Where(m => m.Id == nonRelatedRecocilationToDoId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<NonRelatedRecocilationToDo> UpdateNonReconciliationToDoListRelated(NonRelatedRecocilationToDo nonRelatedRecocilationToDo)
        {
            var result = _applicationDbcontext.NonRelatedRecocilationToDos.Update(nonRelatedRecocilationToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<ActionItemReconciliationToDo> GetActionitemReconciliationByPatientId(string description, int patientId)
        {
            var actionItemReconciliationToDo = await _applicationDbcontext.NonRelatedRecocilationToDos.Where(m => m.ActionItemReconciliationToDo.Description == description && m.Patient.Id == patientId).Select(m => m.ActionItemReconciliationToDo).
                                            SingleOrDefaultAsync();

            return actionItemReconciliationToDo;
        }

        public async Task<ActionItemReconciliationToDo> AddActionItemReconciliationToDo(ActionItemReconciliationToDo actionItemReconciliationToDo)
        {
            var result = await _applicationDbcontext.ActionItemReconciliationToDos.AddAsync(actionItemReconciliationToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<NonRelatedRecocilationToDo> AddNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo)
        {
            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.AddAsync(nonRelatedRecocilationToDo);
            await _applicationDbcontext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<MedicationReconciliation>> GetAllMedicationReconciliation(int patientId)
        {

            var response = await _applicationDbcontext.ReconciliationToDoRelateds.Where(m => m.Patient.Id == patientId).Select(a => a.MedicationReconciliation).ToListAsync();

            return response;
        }

        public async Task<List<NonRelatedRecocilationToDo>> GetMedicationReconciliationNonRelatedRecocilationToDo(int patientId)
        {

            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.Include(a => a.Patient).Include(a => a.ActionItemReconciliationToDo).Where(m => m.Patient.Id == patientId).ToListAsync();

            return result;
        }

        public async Task<List<ActionItemReconciliationToDo>> GetAllActionItemReconciliationToDo(int patientId)
        {
            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.Where(m => m.Patient.Id == patientId).Select(m => m.ActionItemReconciliationToDo).ToListAsync();

            return result;
        }

        public async Task DeleteReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated)
        {

            _applicationDbcontext.ReconciliationToDoRelateds.Remove(reconciliationToDoRelated);

            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<NonRelatedRecocilationToDo> GetNonRelatedRecocilationToDoByActionitemRecocilationToDoId(int actionitemRecocilationToDoId)
        {
            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.Where(m => m.ActionItemReconciliationToDo.Id == actionitemRecocilationToDoId).FirstOrDefaultAsync();
            return result;
        }

        public async Task DeleteNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo)
        {

            _applicationDbcontext.NonRelatedRecocilationToDos.Remove(nonRelatedRecocilationToDo);
            await _applicationDbcontext.SaveChangesAsync();

        }

        public async Task<ReconciliationToDoRelated> GetReconciliationToDoListRelatedByMedicationReconciliationId(int medicationReconciliationId, int patientId)
        {
            var result = await _applicationDbcontext.ReconciliationToDoRelateds.Where(m => m.MedicationReconciliation.Id == medicationReconciliationId && m.Patient.Id == patientId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<NonRelatedRecocilationToDo> GetNonRelatedRecocilationToDoByActionitemReconciliationToDoId(int actionitemReconciliationToDoId, int patientId)
        {
            var result = await _applicationDbcontext.NonRelatedRecocilationToDos.Where(m => m.ActionItemReconciliationToDo.Id == actionitemReconciliationToDoId && m.Patient.Id == patientId).FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<ReconciliationToDoRelated>> GetAllReconciliationToDoRelatedByPatientId(int patientId)
        {
            var result = await _applicationDbcontext.ReconciliationToDoRelateds.Include(m => m.MedicationReconciliation).Where(m => m.Patient.Id == patientId).ToListAsync();
            return result;
        }

        public async Task<List<ReconciliationToDoRelated>> GetReconciliationToDoByPatientId(int patientId)
        {

            return await _applicationDbcontext.ReconciliationToDoRelateds.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void DeleteReconciliationToDoRelatedForServiceTakeAwayMedRec(ReconciliationToDoRelated reconciliationToDoRelated)
        {
            _applicationDbcontext.ReconciliationToDoRelateds.Remove(reconciliationToDoRelated);
        }

        public async Task<List<NonRelatedRecocilationToDo>> GetNonReconciliationToDoByPatientId(int patientId)
        {

            return await _applicationDbcontext.NonRelatedRecocilationToDos.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void DeleteNonRecocilationToDoForServiceTakeAwayMedRec(NonRelatedRecocilationToDo nonRelatedRecocilationToDo)
        {
            _applicationDbcontext.NonRelatedRecocilationToDos.Remove(nonRelatedRecocilationToDo);

        }

        public async Task<List<ReconciliationToDoRelated>> GetReconciliationToDoRelatedByPatientId(int patientId)
        {

            return await _applicationDbcontext.ReconciliationToDoRelateds.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void PatientDeleteForReconciliationToDoRelated(ReconciliationToDoRelated reconciliationToDoRelated)
        {
            _applicationDbcontext.ReconciliationToDoRelateds.Remove(reconciliationToDoRelated);

        }

        public async Task<List<NonRelatedRecocilationToDo>> GetNonRelatedRecocilationToDoRelatedByPatientId(int patientId)
        {

            return await _applicationDbcontext.NonRelatedRecocilationToDos.Where(a => a.Patient.Id == patientId).ToListAsync();
        }

        public void PatientDeleteForNonRelatedRecocilationToDo(NonRelatedRecocilationToDo nonRelatedRecocilationToDo)
        {
            _applicationDbcontext.NonRelatedRecocilationToDos.Remove(nonRelatedRecocilationToDo);

        }


    }
}
