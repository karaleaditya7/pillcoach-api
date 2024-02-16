using Microsoft.EntityFrameworkCore;
using OntrackDb.Context;
using OntrackDb.Dto;
using OntrackDb.Entities;
using OntrackDb.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Repositories;

public interface IPatientPdcData
{
    Task<double> GetPatientPDCAsync(int patientId, string condition, DateTime reportMonth, int durationMonths);
    Task<List<PatientPdcDto>> GetPatientPDCListForPatientAsync(int patientId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Task<List<PatientPdcDto>> GetPatientPDCListForUserAsync(string userId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Task<List<PatientPdcDto>> GetPatientPDCListForPharmacyAsync(int pharmacyId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Task<List<PatientPDC>> GetPatientPDCByPatientId(int patientId);
    void DeletePatientPDCForPatient(PatientPDC patientPDC);
}

internal class PatientPdcData : IPatientPdcData
{
    readonly ApplicationDbContext dbcontext;

    public PatientPdcData(ApplicationDbContext dbcontext)
    {
        this.dbcontext = dbcontext;
    }

    public async Task<double> GetPatientPDCAsync(int patientId, string condition, DateTime reportMonth, int durationMonths)
    {
        if (reportMonth.Day != 1) reportMonth = reportMonth.AddDays(-reportMonth.Day + 1);

        var record = await dbcontext.PatientPDCs.FirstOrDefaultAsync(p =>
                p.PatientId == patientId &&
                p.Condition == condition &&
                p.PdcMonth == reportMonth.Date &&
                p.DurationMonths == durationMonths);

        return (double)(record?.PDC ?? 0);
    }

    public async Task<List<PatientPdcDto>> GetPatientPDCListForPatientAsync(int patientId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        return await GetPatientPDCListAsync(new int[] {patientId}, condition, reportMonth, durationMonths, queryType);
    }

    public async Task<List<PatientPdcDto>> GetPatientPDCListForPharmacyAsync(int pharmacyId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        var patientIds = await dbcontext.Patients.Where(p => p.Pharmacy.Id == pharmacyId && !p.IsDeleted && !p.Pharmacy.IsDeleted)
            .Select(p => p.Id).ToListAsync();

        return await GetPatientPDCListAsync(patientIds, condition, reportMonth, durationMonths, queryType);
    }

    public async Task<List<PatientPdcDto>> GetPatientPDCListForUserAsync(string userId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        var patientIds = await dbcontext.Patients
            .Where(p => p.Pharmacy.PharmacyUsers.Any(pu => pu.UserId == userId) && !p.IsDeleted)
            .Select(p => p.Id).ToListAsync();

        return await GetPatientPDCListAsync(patientIds, condition, reportMonth, durationMonths, queryType);
    }

    public async Task<List<PatientPDC>> GetPatientPDCByPatientId(int patientId)
    {
        var result = await dbcontext.PatientPDCs.Where(s => s.PatientId == patientId).ToListAsync();

        return result;
    }

    public void DeletePatientPDCForPatient(PatientPDC patientPDC)
    {
        dbcontext.PatientPDCs.Remove(patientPDC);

    }

    async Task<List<PatientPdcDto>> GetPatientPDCListAsync(IEnumerable<int> patientIds, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType)
    {
        if (patientIds == null || !patientIds.Any()) return new();

        var date = reportMonth.Date;

        var query = dbcontext.PatientPDCs.Where(p =>
                patientIds.Any(id => id == p.PatientId) &&
                p.Condition == condition &&
                p.DurationMonths == durationMonths
            );

        if (queryType == PdcQueryType.ByPdcMonth)
        {
            // pdc month is stored as the first day of the month
            if (date.Day != 1) date = date.AddDays(-date.Day + 1);
            query = query.Where(p => p.PdcMonth == date);
        }
        else if (queryType == PdcQueryType.ByEndDate)
        {
            // end date is stored as the last day of the month
            date = new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
            query = query.Where(p => p.EndDate == date);
        }

        var records = await query
            .Select(p => new PatientPdcDto
                {
                    PatientId = p.PatientId,
                    PdcMonth = date,
                    Condition = condition,
                    DurationMonths = durationMonths,
                    PDC = p.PDC
                }
            ).ToListAsync();

        return records;
    }
}
