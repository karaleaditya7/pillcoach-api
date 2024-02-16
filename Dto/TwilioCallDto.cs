using System;

namespace OntrackDb.Dto;

public class TwilioCallDto
{
    public string Sid { get; set; }
    public DateTime CallDate { get; set; }
    public string CallStatus { get; set; }
    public string Direction { get; set; }
    public string OtherNumber { get; set; }
    public string OtherCaller { get; set; }
    public string DurationSeconds { get; set; }
    public string RecordingPathSid { get; set; }
    public string UserName { get; set; }
}
