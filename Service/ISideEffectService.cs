using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface ISideEffectService
    {
        Task<Response<SideEffect>> AddPatientSideEffects(SideEffectModel model);
        Task<Response<Reaction>> GetSideEffectReactionsByPatientId(int id);
        Task<Response<SideEffect>> UpdatePatientSideEffect(int patientid, int medicationsubstanceid, List<Reaction> reactions);

        Task<Response<MedicationSubstance>> GetMedicationsubstancesByPatientIdForSideEffect(int id);
        Task<Response<SideEffect>> GetAllSideEffectReactionsByMedicationSubstanceById(int id, int patientId);

        Task<Response<SideEffect>> DeleteSideEffectBySideEffectId(int medicationSubstanceId, int patientId);

        Task<Response<SideEffect>> DeleteSideEffectByMedicationSubstanceIdAndPatientId(int medicationSubstanceId, int patientId);
    }
}
