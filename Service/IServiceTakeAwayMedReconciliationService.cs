using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IServiceTakeAwayMedReconciliationService
    {
        Task<Response<ServiceTakeAwayMedReconciliation>> AddServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliationModel model);

        Task<Response<ServiceTakeAwayMedReconciliation>> GetServiceTakeawayMedReconciliationById(int serviceTakeawaymedreconciliationId);

        Task<Response<ServiceTakeAwayMedReconciliation>> EditServiceTakeawayMedReconciliation(ServiceTakeAwayMedReconciliationUpdateModel model);

        Task<Response<ServiceTakeAwayMedReconciliation>> DeleteServiceTakeAwayMedReconciliationById(int serviceTakeawaymedreconciliationId);
        Task<Response<ServiceTakeAwayMedReconciliation>> GetServiceTakewayMedReconciliationByPatientId(int patientId);

        Task<Response<ServiceTakeAwayMedReconciliation>> SendCmrPdfEmailByMedReconciliation(string email, int patientId);

        Task<Response<ServiceTakeAwayMedReconciliation>> CheckLinkExpiredByMedReconciliation(int patientId, string uuid);

        Task<Response<byte[]>> PatientVerificationForPdfMedReconciliation(string lastname, DateTime dob, int patientId, string uuid);
        Task<Response<TakeawayVerify>> GetTakeawayVerifyForMedRecByPatientId(int patientId);
    }
}
