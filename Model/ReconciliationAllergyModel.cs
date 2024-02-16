using System.Collections.Generic;

namespace OntrackDb.Model
{
    public class ReconciliationAllergyModel
    {
        public string MedicationSubstance { get; set; }
        public List<string> Reactions { get; set; }
        public int PatientId { get; set; }

    }
}
