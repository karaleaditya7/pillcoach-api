using Microsoft.AspNetCore.Mvc;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Model;
using System;
using System.Threading.Tasks;

namespace OntrackDb.Service
{
    public interface IServiceTakeawayInformationService
    {
        Task<Response<ServiceTakeawayInformation>> AddServiceTakeawayInformation(ServiceTakeawayInformationModel model);
        Task<Response<ServiceTakeawayInformation>> GetServiceTakeawayInformationById(int serviceTakeawayInformationId);
        Task<Response<ServiceTakeawayInformation>> EditServiceTakeawayInformation(ServiceTakeawayInformationForUpdateModel model);
        Task<Response<ServiceTakeawayInformation>> DeleteServiceTakawayInformationById(int serviceTakeawayInformationId);

        Task<Response<ServiceTakeawayInformation>> GetServiceTakewayInformationByPatientId(int patientId);

        Task<Response<byte[]>> PatientVerificationForPdf( string lastname, DateTime dob, int patientId,string uuid);
        Task<Response<ServiceTakeawayInformation>> CheckLinkExpired(int patientId, string uuid);
        Task<Response<ServiceTakeawayInformation>> SendCmrPdfEmail(string email,int patientId);
        Task<Response<TakeawayVerify>> GetTakeawayVerifyByPatientId(int patientId);
    }
}
