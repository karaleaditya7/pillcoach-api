using System;

namespace OntrackDb.Model;

public class AuditPatientModel
{
    public int PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime PatientDOB { get; set; }
}
