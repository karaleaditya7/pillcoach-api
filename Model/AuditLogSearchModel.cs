using OntrackDb.Enums;
using System;
using System.Collections.Generic;

namespace OntrackDb.Model;

public class AuditLogSearchModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string SortField { get; set; }
    public SortOrder SortDirection { get; set; }
    public IEnumerable<string> EmployeeIds { get; set; }
    public IEnumerable<int> PatientIds { get; set; }
    public int TimezoneOffset { get; set; }
}
