using System;

namespace OntrackDb.Dto;

public class MedicationHistory
{
    public int PatientId { get; set; }
    public string RxNumber { get; set; }
    public DateTime RxDate { get; set; }
    public int Quantity { get; set; }
    public int Supply { get; set; }
    public string PrescriberName { get; set; }
    public DateTime LastFillDate { get; set; }
    public DateTime? NextFillDate { get; set; }
    public int RefillRemaining { get; set; }
    public decimal PatientPayDue { get; set; }
    public int RefillNumber { get; set; }
    public string PrimaryThirdParty { get; set; }
}
