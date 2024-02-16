using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities
{
    [Table("address")]
    public class Address
    {
        [Key]
        public int Id { get; set; }
        [Column("addressLineOne")]
        public string AddressLineOne { get; set; }
        [Column("addressLineTwo")]
        public string AddressLineTwo { get; set; }
        [Column("city")]
        public string City { get; set; }
        [Column("state")]
        public string State { get; set; }
        [Column("zipCode")]
        public string ZipCode { get; set; }
    }
}
