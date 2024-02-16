using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories;

public interface IPatientCallInfoData
{
    Task<List<PatientCallInfo>> GetPatientCallListAsync(int patientId);
    Task<PatientCallInfo> SavePatientCallInfoAsync(PatientCallInfo callInfo);
    void DeletePatientCallInfoAsync(PatientCallInfo patientCallInfo);
}

internal class PatientCallInfoData : IPatientCallInfoData
{
    readonly ApplicationDbContext dbContext;

    public PatientCallInfoData(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<List<PatientCallInfo>> GetPatientCallListAsync(int patientId)
    {
        return await dbContext.PatientCallInfo
            .Include(h => h.User)
            .Where(h => h.PatientId == patientId)
            .ToListAsync();
    }

    public void DeletePatientCallInfoAsync(PatientCallInfo patientCallInfo)
    {
          dbContext.PatientCallInfo.Remove(patientCallInfo);
           
    }

    public async Task<PatientCallInfo> SavePatientCallInfoAsync(PatientCallInfo callInfo)
    {
        if (callInfo == null) return null;

        var id = 0;

        if (callInfo.Id > 0)
        {
            var entity = await dbContext.PatientCallInfo
                .Where(c => c.Id == callInfo.Id && c.PatientId == callInfo.PatientId)
                .FirstOrDefaultAsync();

            if (entity == null) throw new ApplicationException($"No record found with id: {callInfo.Id} for patient: {callInfo.PatientId}");

            entity.CallReason = callInfo.CallReason;
            entity.Notes = callInfo.Notes;
            entity.MedicationsDiscussedJson = callInfo.MedicationsDiscussedJson;

            dbContext.Update(entity);

            if (await dbContext.SaveChangesAsync() > 0)
                id = entity.Id;
        }
        else
        {
            dbContext.PatientCallInfo.Add(callInfo);

            if (await dbContext.SaveChangesAsync() > 0)
                id = callInfo.Id;
        }

        if (id > 0)
        {
            return await dbContext.PatientCallInfo
                .Include(h => h.User)
                .Where(h => h.Id == id)
                .FirstOrDefaultAsync();
        }

        return null;
    }
}
