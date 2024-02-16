using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Repositories;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service;

public interface IPatientCallInfoService
{
    Task<Response<PatientCallInfoDto>> GetPatientCallListAsync(int patientId);
    Task<Response<PatientCallInfoDto>> SavePatientCallInfoAsync(PatientCallInfoDto callInfo);
}

internal class PatientCallInfoService : IPatientCallInfoService
{
    readonly IPatientCallInfoData patientCallInfoData;
    readonly IHttpContextAccessor httpContextAccessor;

    public PatientCallInfoService(IPatientCallInfoData patientCallInfoData, IHttpContextAccessor httpContextAccessor)
    {
        this.patientCallInfoData = patientCallInfoData;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Response<PatientCallInfoDto>> GetPatientCallListAsync(int patientId)
    {
        var response = new Response<PatientCallInfoDto>();

        var calls = await patientCallInfoData.GetPatientCallListAsync(patientId);

        var results = calls.Select(c => ConvertToDto(c));

        response.Success = true;
        response.DataList = results.ToList();

        return response;
    }

    private PatientCallInfoDto ConvertToDto(PatientCallInfo c)
    {
        return new PatientCallInfoDto
        {
            Id = c.Id,
            PatientId = c.PatientId,
            UserName = $"{c.User.FirstName} {c.User.LastName}",
            CallDate = c.CallDate,
            AttemptNo = c.AttemptNo,
            CallReason = c.CallReason,
            Notes = c.Notes,
            MedicationsDiscussed = JsonConvert.DeserializeObject<IEnumerable<string>>(c.MedicationsDiscussedJson)
        };
    }

    public async Task<Response<PatientCallInfoDto>> SavePatientCallInfoAsync(PatientCallInfoDto callInfo)
    {
        var response = new Response<PatientCallInfoDto>();

        try
        {
            var userId = httpContextAccessor.HttpContext.User?.FindFirst("id")?.Value;

            var entity = new PatientCallInfo
            {
                Id = callInfo.Id,
                PatientId = callInfo.PatientId,
                UserId = userId,
                CallDate = callInfo.CallDate,
                AttemptNo = callInfo.AttemptNo,
                CallReason = callInfo.CallReason,
                Notes = callInfo.Notes.TrimEmptyToNull(),
                MedicationsDiscussedJson = JsonConvert.SerializeObject(callInfo.MedicationsDiscussed)
            };

            var result = await patientCallInfoData.SavePatientCallInfoAsync(entity);

            response.Success = result != null;

            if (response.Success)
                response.Data = ConvertToDto(result);
            else
                response.Message = "Unable to save call info";
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
        }

        return response;
    }
}
