using OntrackDb.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("message")]
    public class Message
    {
        [Key]
        public int Id { get; set; }
       
        [Column("fromUserId")]
        public User  FromUser  { get; set; }

        [Column("toUserId")]
        public User ToUser { get; set; }

        [Column("messageText")]
        public string MessageText { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("sentStatus")]
        public string SentStatus { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("sentDateTime")]
        public DateTime SentDateTime { get; set; }

        [Column("readDateTime")]
        public DateTime ReadDateTime { get; set; }

        [Column("isDeleted")]
        public Boolean IsDeleted { get; set; }

        [Column("fromDeleted")]
        public Boolean FromDeleted { get; set; }


        [Column("toDeleted")]
        public Boolean ToDeleted { get; set; }


        [NotMapped]
        public string StrSendDateTime { get; set; }


    }
}
