using System;

namespace OntrackDb.Dto;

public class PatientPdcDto
{
    public int PatientId { get; set; }
    public string Condition { get; set; }
    public DateTime PdcMonth { get; set; }
    public int DurationMonths { get; set; }
    public decimal PDC { get; set; }
}
