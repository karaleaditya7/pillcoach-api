using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OntrackDb.Authentication;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OntrackDb.Entities;

[Table("ImportSourceFiles")]
public class ImportSourceFile
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Filename { get; set; }
    public int PharmacyId { get; set; }

    [Required, Column(TypeName = "varchar(50)")]
    public string BlobName { get; set; }

    [Required]
    public string UserId { get; set; }

    public DateTime UploadDateUTC { get; set; }
    public DateTime? StagingStartTimeUTC { get; set; }
    public DateTime? StagingEndTimeUTC { get; set; }
    public DateTime? ImportStartTimeUTC { get; set; }
    public DateTime? ImportEndTimeUTC { get; set; }

    [Required]
    public int ImportStatusId { get; set; }

    public int? TotalRecords { get; set; }
    public int? TotalImported { get; set; }

    public string ColumnMappingsJson { get; set; }
    public string ErrorsJson { get; set; }
    public string ErrorStack { get; set; }

    [NotMapped]
    public string RequestId => $"PC-{UploadDateUTC:MMddyyyy}-{Id.ToString().PadLeft(5, '0')}";

    public User User { get; set; }
    public ImportFileStatus Status { get; set; }
}

internal class ImportFileConfiguration : IEntityTypeConfiguration<ImportSourceFile>
{
    public void Configure(EntityTypeBuilder<ImportSourceFile> builder)
    {
        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Pharmacy>()
            .WithMany()
            .HasForeignKey(b => b.PharmacyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Status)
            .WithMany()
            .HasForeignKey(b => b.ImportStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}