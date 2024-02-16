using OntrackDb.Entities;

namespace OntrackDb.Model
{
    public class EditPatientModel
    {
        public int Id { get; set; }

        public Address Address { get; set; }

        public Contact Contact { get; set; }
        public string Language { get; set; }
        public int PrimaryThirdPartyId { get; set; }

    }
}
