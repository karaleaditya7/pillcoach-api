using System;

namespace OntrackDb.Model
{
    public class MessageModel
    {
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string MessageText { get; set; }
        public string Status { get; set; }
        public string SentStatus { get; set; }
        public string Type { get; set; }
        public DateTime SentDateTime { get; set; }
        public DateTime ReadDateTime { get; set; }
    }
}
