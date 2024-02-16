using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace OntrackDb.Entities;

public class AuditActionSourceType
{
    public int Id { get; set; }

    [Required, Column(TypeName = "varchar(50)")]
    public string ActionSourceType { get; set; }
}

internal class AuditActionSourceTypeConfiguration : IEntityTypeConfiguration<AuditActionSourceType>
{
    public void Configure(EntityTypeBuilder<AuditActionSourceType> builder)
    {
        var list = new List<AuditActionSourceType>();

        var rg = new Regex(@"\s[A-Z]");

        foreach (var e in Enum.GetValues<Enums.AuditActionSourceType>())
            list.Add(new() { Id = (int)e, ActionSourceType = rg.Replace(e.ToString(), "$0").Trim() });

        builder.HasData(list);
    }
}
