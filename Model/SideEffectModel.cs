using System.Collections.Generic;

namespace OntrackDb.Model
{
    public class SideEffectModel
    {
        public string MedicationSubstance { get; set; }
        public List<string> Reactions { get; set; }
        public int PatientId { get; set; }
    }
}
