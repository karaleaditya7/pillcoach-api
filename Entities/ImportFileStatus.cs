using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace OntrackDb.Entities;

public class ImportFileStatus
{
    public int Id { get; set; }

    [Required, Column(TypeName = "varchar(20)")]
    public string Name { get; set; }
}

internal class ImportFileStatusConfiguration : IEntityTypeConfiguration<ImportFileStatus>
{
    public void Configure(EntityTypeBuilder<ImportFileStatus> builder)
    {
        var list = new List<ImportFileStatus>();

        var rg = new Regex(@"[A-Z]");

        foreach (var e in Enum.GetValues<Enums.ImportFileStatus>())
            list.Add(new() { Id = (int)e, Name = rg.Replace(e.ToString(), " $0").Trim() });

        builder.HasData(list);
    }
}