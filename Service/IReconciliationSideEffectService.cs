using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IReconciliationSideEffectService
    {
        Task<Response<ReconciliationSideeffect>> AddPatientReconciliationSideEffect(ReconciliationSideEffectModel sideEffectModel);

        Task<Response<ReconciliationSideeffect>> UpdatePatientReconciliationSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions);
        Task<Response<Reaction>> GetAllReconciliationSideEffectReactions(int id);

        Task<Response<MedicationSubstance>> GetRecMedsubstancesByPatientIdForSideEffect(int id);

        Task<Response<ReconciliationSideeffect>> GetAllRecSideEffectReactionsByMedSubstanceById(int id, int patientId);

        Task<Response<ReconciliationSideeffect>> DeleteRecSideEffectByRecSideEffectId(int medicationSubstanceId, int patientId);
    }
}
