using OntrackDb.Entities;

namespace OntrackDb.Dto
{
    public class MedicationDto
    {
        public int Id { get; set; }
        public int RelatedPharmacies { get; set; }
        public int AssignedPatient { get; set; }
        public string GenericName { get; set; }
        public string SbdcName { get; set; }
    }
}
