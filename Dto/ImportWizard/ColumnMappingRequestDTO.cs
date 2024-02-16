using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OntrackDb.Dto.ImportWizard;

public class ColumnMappingRequestDTO
{
    [Range(1, int.MaxValue)]
    public int RecordId { get; set; }

    [Required]
    public string UniqueId { get; set; }

    [MinLength(1)]
    public List<ColumnMapping> ColumnMappings { get; set; }
}
