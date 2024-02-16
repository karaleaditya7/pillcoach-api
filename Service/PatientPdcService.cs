using OntrackDb.Enums;
using OntrackDb.Model;
using OntrackDb.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OntrackDb.Service;

public interface IPatientPdcService
{
    Task<PdcModelEx> GetPdcForPatientAsync(int patientId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Task<PdcModelEx> GetPdcForPharmacyAsync(int pharmacyId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Task<PdcModelEx> GetPdcForUserAsync(string userId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth);
    Tuple<DateTime, DateTime> GetMedicationPeriodForGraph(int months);
}

public class PatientPdcService : IPatientPdcService
{
    readonly IPatientPdcData patientPdcData;

    public PatientPdcService(IPatientPdcData patientPdcData)
    {
        this.patientPdcData = patientPdcData;
    }

    public Tuple<DateTime, DateTime> GetMedicationPeriodForGraph(int months)
    {
        var today = DateTime.Today;

        DateTime firstDayOfThisMonth = today.AddDays(-today.Day + 1);

        DateTime endDate = firstDayOfThisMonth.AddMonths(-2);
        DateTime startDate = endDate.AddMonths(-(months - 1));

        if (months == 12)
        {
            startDate = new DateTime(endDate.Year, 3, 1); // YTD graph always starts from March

            if (today.Month == 3 || today.Month == 4)
            {
                // for March and April, show previous year's data
                startDate = new DateTime(today.Year - 1, 1, 1);
                endDate = new DateTime(today.Year - 1, 12, 1);
            }
        }

        return new Tuple<DateTime, DateTime>(startDate, endDate);
    }

    public async Task<PdcModelEx> GetPdcForPatientAsync(int patientId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        if (reportMonth.Day != 1) reportMonth = reportMonth.AddDays(-reportMonth.Day + 1);

        var pdcModel = new PdcModelEx { Date = reportMonth.Date, Condition = condition, DurationMonths = durationMonths };

        var pdcList = await patientPdcData.GetPatientPDCListForPatientAsync(patientId, condition, reportMonth, durationMonths, queryType);

        if (pdcList != null && pdcList.Any())
        {
            pdcModel.TotalPatients = 1;
            pdcModel.AdherenceCount = pdcList.Count(p => p.PDC >= 80);

            pdcModel.Value = Math.Round((double)pdcList.First().PDC, 2);
        }

        return pdcModel;
    }

    public async Task<PdcModelEx> GetPdcForPharmacyAsync(int pharmacyId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        if (reportMonth.Day != 1) reportMonth = reportMonth.AddDays(-reportMonth.Day + 1);

        var pdcModel = new PdcModelEx { Date = reportMonth.Date, Condition = condition, DurationMonths = durationMonths };

        var pdcList = await patientPdcData.GetPatientPDCListForPharmacyAsync(pharmacyId, condition, reportMonth, durationMonths, queryType);

        if (pdcList != null && pdcList.Any())
        {
            pdcModel.TotalPatients = pdcList.Count(p => p.PDC > 0);
            pdcModel.AdherenceCount = pdcList.Count(p => p.PDC >= 80);

            pdcModel.Value = pdcModel.TotalPatients == 0 ? 0 : Math.Round(((double)pdcModel.AdherenceCount / pdcModel.TotalPatients) * 100, 2);
        }

        return pdcModel;
    }

    public async Task<PdcModelEx> GetPdcForUserAsync(string userId, string condition, DateTime reportMonth, int durationMonths, PdcQueryType queryType = PdcQueryType.ByPdcMonth)
    {
        if (reportMonth.Day != 1) reportMonth = reportMonth.AddDays(-reportMonth.Day + 1);

        var pdcModel = new PdcModelEx { Date = reportMonth.Date, Condition = condition, DurationMonths = durationMonths };

        var pdcList = await patientPdcData.GetPatientPDCListForUserAsync(userId, condition, reportMonth, durationMonths, queryType);

        if (pdcList != null && pdcList.Any())
        {
            pdcModel.TotalPatients = pdcList.Count(p => p.PDC > 0);
            pdcModel.AdherenceCount = pdcList.Count(p => p.PDC >= 80);
            pdcModel.Value = pdcModel.TotalPatients == 0 ? 0 : Math.Round(((double)pdcModel.AdherenceCount / pdcModel.TotalPatients) * 100, 2);
        }

        return pdcModel;
    }
}
