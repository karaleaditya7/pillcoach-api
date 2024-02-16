using System.Collections.Generic;

namespace OntrackDb.Dto.ImportWizard
{
    public class FileProcessingErrorDto
    {
        public string ErrorsJson { get; set; }
        public string FileName { get; set; }
        public int PharmacyId { get; set; }
        public List<StagingError> StagingErrors { get; set; } 

        // Constructor to initialize the list
        public FileProcessingErrorDto()
        {
            StagingErrors = new List<StagingError>();
        }
    }
}
