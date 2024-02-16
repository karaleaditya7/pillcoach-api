using Microsoft.AspNetCore.Mvc;
using OntrackDb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Repositories
{
    public interface ISideEffectData
    {
        Task<SideEffect> AddPatientSideEffect(SideEffect sideEffect,bool commitChanges);
        Task<List<int>> GetReactionyBySideEffectId(int sideeffectId);
        Task<SideEffect> GetSideEffectBySideEffectId(int sideeffectId);
        Task<Reaction> GetReactionByReactionId(int reactionId);
        Task<SideEffect> UpdateReactionBySideEffect(SideEffect sideEffect);

        Task<List<Reaction>> GetAllSideEffectsReactionsByPatientId(int id);
        Task<List<MedicationSubstance>> GetMedicationSubstancesByPatientIdForSideEffect(int id);
        Task<List<SideEffect>> GetAllSideEffectReactionsByMedicationSubstanceById(int id, int patientId);

        Task DeleteSideEffect(SideEffect sideEffect);

        Task<List<SideEffect>> GetSideEffectByMedicationSubstanceAndPatientId(int medicationSubstanceId, int patientId);

        Task<SideEffect> GetSideEffectById(int sideEffectId);

        Task<List<SideEffect>> GetSideEffectMedicationSubstanceId(string medicationSubstance, int patientId);

        Task<List<SideEffect>> GetSideEffectByPatientId(int patientId);

        void DeleteSideEffectBySideEffectForServiceTakeAway(SideEffect sideEffect);
        Task<List<SideEffect>> GetAllSideEffectByPatientId(int patientId);
        void PatientDeleteForSideEffect(SideEffect sideEffect);

        void DeleteMedicationSubstanceForSideEffect(MedicationSubstance medicationSubstance);
        Task<List<string>> GetReactionyBySideEffectId(int patientid,int medicationsubstanceid);
        Task<List<SideEffect>> GetSideEffectByPatientIdAndMedicationSubstanceId(int patientid, int medicationsubstanceid);
        Task SaveChangesAsync();
    }
}
