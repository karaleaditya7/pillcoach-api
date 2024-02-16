namespace OntrackDb.Model
{
    public class TwilioRequestModel
    {
        public string CallSid { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CallStatus { get; set; }
    }
}
