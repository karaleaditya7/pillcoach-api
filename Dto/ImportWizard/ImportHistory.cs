using System;

namespace OntrackDb.Dto.ImportWizard;

public class ImportHistory
{
    public int ImportId { get; set; }
    public string Status { get; set; }
    public string UploadedBy { get; set; }
    public DateTime UploadDateUTC { get; set; }
    public DateTime? ImportDateUTC { get; set; }
    public string Filename { get; set; }
    public int RecordsUploaded { get; set; }
    public int RecordsImported { get; set; }
    public string ImportNumber => $"PC-{UploadDateUTC:MMddyyyy}-{ImportId.ToString().PadLeft(5, '0')}";
    public bool HasErrors { get; set; }
    public bool BlobReceived { get; set; }
    public string BlobUri { get; set; }
}
