using System;
using System.Collections.Generic;

namespace OntrackDb.Dto;

public class PatientCallInfoDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string UserName { get; set; }
    public DateTime CallDate { get; set; }
    public int AttemptNo { get; set; }
    public string CallReason { get; set; }
    public string Notes { get; set; }
    public IEnumerable<string> MedicationsDiscussed { get; set; }
}
