using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace OntrackDb.Entities;

public class AuditActionType
{
    public int Id { get; set; }

    [Required, Column(TypeName = "varchar(30)")]
    public string ActionType { get; set; }
}

internal class AuditActionTypeConfiguration : IEntityTypeConfiguration<AuditActionType>
{
    public void Configure(EntityTypeBuilder<AuditActionType> builder)
    {
        var list = new List<AuditActionType>();

        var rg = new Regex(@"\s[A-Z]");

        foreach (var e in Enum.GetValues<Enums.AuditActionType>())
            list.Add(new() { Id = (int)e, ActionType = rg.Replace(e.ToString(), "$0").Trim() });

        builder.HasData(list);
    }
}
