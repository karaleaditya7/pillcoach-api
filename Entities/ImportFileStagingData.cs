using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities;

public class ImportFileStagingData
{
    public int Id { get; set; }
    public int ImportSourceFileId { get; set; }
    public int RowNo { get; set; }
    public string ErrorsJson { get; set; }
    public bool IsProcessed { get; set; }

    [MaxLength(100)]
    public string PharmacyName { get; set; }

    [MaxLength(50)]
    public string PharmacyNPI { get; set; }

    [MaxLength(100)]
    public string PatientIdentifier { get; set; }

    [MaxLength(50)]
    public string PatientFirstName { get; set; }

    [MaxLength(50)]
    public string PatientLastName { get; set; }

    public DateTime? PatientDateofBirth { get; set; }

    [MaxLength(100)]
    public string PatientPrimaryAddress { get; set; }

    [MaxLength(50)]
    public string PatientPrimaryCity { get; set; }

    [MaxLength(50)]
    public string PatientPrimaryState { get; set; }

    [MaxLength(20)]
    public string PatientPrimaryZipCode { get; set; }

    [MaxLength(20)]
    public string PatientPrimaryPhone { get; set; }

    [MaxLength(100)]
    public string PatientEmail { get; set; }

    [MaxLength(20)]
    public string PatientGender { get; set; }

    [MaxLength(50)]
    public string PatientLanguage { get; set; }

    [MaxLength(50)]
    public string PatientRace { get; set; }

    [MaxLength(50)]
    public string PrescriberFirstName { get; set; }

    [MaxLength(50)]
    public string PrescriberLastName { get; set; }

    [MaxLength(50)]
    public string PrescriberNPI { get; set; }

    [MaxLength(100)]
    public string PrescriberPrimaryAddress { get; set; }

    [MaxLength(50)]
    public string PrescriberPrimaryCity { get; set; }

    [MaxLength(50)]
    public string PrescriberPrimaryState { get; set; }

    [MaxLength(20)]
    public string PrescriberPrimaryZip { get; set; }

    [MaxLength(20)]
    public string PrescriberPrimaryPhone { get; set; }

    [MaxLength(20)]
    public string PrescriberFaxNumber { get; set; }

    [MaxLength(50)]
    public string RxNumber { get; set; }
    public DateTime? RxDate { get; set; }
    public int? RefillNumber { get; set; }
    public DateTime? DateFilled { get; set; }
    public int? DaysSupply { get; set; }
    public int? RefillsRemaining { get; set; }
    public decimal? DispensedQuantity { get; set; }

    [MaxLength(50)]
    public string DispensedItemNDC { get; set; }
    public string DispensedItemName { get; set; }
    public string Directions { get; set; }
    public decimal? PatientPaidAmount { get; set; }

    public string DrugSbdcName { get; set; }
    public string PrimaryThirdParty { get; set; }
    public string PrimaryThirdPartyBin { get; set; }
    [NotMapped]
    public bool HasRowUpdated { get; set; }
}

internal class ImportFileDataConfiguration : IEntityTypeConfiguration<ImportFileStagingData>
{
    public void Configure(EntityTypeBuilder<ImportFileStagingData> builder)
    {
        builder.HasOne<ImportSourceFile>()
            .WithMany()
            .HasForeignKey(b => b.ImportSourceFileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.IsProcessed)
            .HasDefaultValueSql("0");
    }
}