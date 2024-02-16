using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface IReconciliationSideEffectData
    {

        Task<List<ReconciliationSideeffect>> GetRecSideeffectMedSubstanceId(string medicationSubstance, int patientId);

        Task<ReconciliationSideeffect> AddPatientReconciliationSideEffect(ReconciliationSideeffect reconciliationSideeffect, bool commitChanges);

        Task SaveChangesAsync();

        Task<List<ReconciliationSideeffect>> GetRecSideEffectByPatientIdAndMedSubstanceId(int patientid, int medicationsubstanceid);

        Task DeleteReconciliationSideEffect(ReconciliationSideeffect reconciliationSideeffect);

        Task<List<Reaction>> GetAllRecSideEffectsReactionsByPatientId(int id);

        Task<List<MedicationSubstance>> GetMedSubstancesByPatientIdForSideEffect(int id);

        Task<List<ReconciliationSideeffect>> GetAllSideEffectReactionsByMedSubstanceById(int id, int patientId);

        Task<List<ReconciliationSideeffect>> GetRecSideEffectByMedSubstanceAndPatientId(int medicationSubstanceId, int patientId);

        Task<List<ReconciliationSideeffect>> GetMedRecSideEffectByPatientId(int patientId);

        void DeleteMedRecSideEffectByServiceTakeMedRecId(ReconciliationSideeffect reconciliationSideeffect);

        
    }
}
