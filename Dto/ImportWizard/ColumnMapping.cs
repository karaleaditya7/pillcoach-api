using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Dto.ImportWizard;

public class ColumnMapping
{
    [Required]
    public string PCField { get; set; }

    [Required]
    public string ExternalField { get; set; }
}
