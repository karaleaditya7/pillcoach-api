using System;

namespace OntrackDb.Dto
{
    public class StagingRowErrorDto
    {
        public int ImportSourceFileId { get; set; }
        public int RowNo { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyNPI { get; set; }
        public string PatientIdentifier { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public DateTime? PatientDateofBirth { get; set; }
        public string PatientPrimaryAddress { get; set; }
        public string PatientPrimaryCity { get; set; }
        public string PatientPrimaryState { get; set; }
        public string PatientPrimaryZipCode { get; set; }
        public string PatientPrimaryPhone { get; set; }
        public string PatientEmail { get; set; }
        public string PatientGender { get; set; }
        public string PatientLanguage { get; set; }
        public string PatientRace { get; set; }
        public string PrescriberFirstName { get; set; }
        public string PrescriberLastName { get; set; }
        public string PrescriberNPI { get; set; }
        public string PrescriberPrimaryAddress { get; set; }
        public string PrescriberPrimaryCity { get; set; }
        public string PrescriberPrimaryState { get; set; }
        public string PrescriberPrimaryZip { get; set; }
        public string PrescriberPrimaryPhone { get; set; }
        public string PrescriberFaxNumber { get; set; }
        public string RxNumber { get; set; }
        public int? RefillNumber { get; set; }
        public DateTime? DateFilled { get; set; }
        public DateTime? RxDate { get; set; }
        public int? DaysSupply { get; set; }
        public int? RefillsRemaining { get; set; }
        public decimal? DispensedQuantity { get; set; }
        public string DispensedItemNDC { get; set; }
        public string DispensedItemName { get; set; }
        public string Directions { get; set; }
        public decimal? PatientPaidAmount { get; set; }
        public string DrugSbdcName { get; set; }
    }
}
