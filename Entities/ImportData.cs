using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("importData")]
    public class ImportData
    {
        [Key]
        public int id { get; set; }
        public string data { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public DateTime created_datetime { get; set; }

    }
}
