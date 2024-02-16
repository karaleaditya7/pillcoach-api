using OntrackDb.Authentication;
using OntrackDb.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OntrackDb.Model
{
    public class MessageUserModel
    {
        public User User { get; set; }
        public string MessageBody { get; set; }
        public DateTime SendDateTime { get; set; }

        public int UnReadMessageCount { get; set; }

        public List<Message> ListOfAllMessages { get; set; }
    }
}
