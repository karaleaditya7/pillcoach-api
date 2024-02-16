using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Entities
{
    public class PharmacyUser
    {
        [Key]
        public int Id { get; set; }
        public int PharmacyId {  get; set; }
        public Pharmacy Pharmacy { get; set; }
        public string UserId { get; set; }
        public Authentication.User User { get; set; }
        public string Name { get; }
        public Address Address { get; }
        public Contact Contact { get; }
    }
}
