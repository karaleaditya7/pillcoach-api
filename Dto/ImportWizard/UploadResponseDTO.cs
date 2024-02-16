using System.Collections.Generic;

namespace OntrackDb.Dto.ImportWizard;

public class UploadResponseDTO
{
    public int RecordId { get; set; }
    public string UniqueId { get; set; }
    public IEnumerable<string> ColumnNames { get; set; }
}
