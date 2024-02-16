using System;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Model;

public class AuditLogModel
{
    public int? PatientId { get; set; }

    [Required]
    public int ActionTypeId { get; set; }

    [Required]
    public int ActionSourceId { get; set; }
}

public class AuditLogDetailModel : AuditLogModel
{
    public int LogId { get; set; }
    public DateTime LogDateUTC { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeEmail { get; set; }
    public string PatientName { get; set; }
    public DateTime? PatientDOB { get; set; }
    public string ActionType { get; set; }
    public string ActionSource { get; set; }

    internal string GetRecordForCsv()
    {
        string[] data =
        {
            LogDateUTC.ToString("MM/dd/yyyy"),
            LogDateUTC.ToString("HH:mm:ss"),
            EmployeeName,
            PatientName,
            PatientDOB?.ToString("MM/dd/yyyy"),
            ActionType,
            ActionSource
        };

        return string.Join(",", data);
    }

    internal static string GetHeaderRowForCsv()
    {
        string[] columnNames =
        {
            "Date",
            "Time",
            "Employee Name",
            "Patient Name",
            "Patient DOB",
            "Action",
            "Source"
        };

        return string.Join(",", columnNames);
    }
}
