using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("licenses")]
    public class Licenses
    {
        [Key]
        public int Id { get; set; }
        [Column("number")]
        public string Number { get; set; }
        [Column("issueState")]
        public string IssueState {  get; set; }
        [Column("expirationDate")]
        public DateTime ExpirationDate {  get; set; }
    }
}
