namespace OntrackDb.Enums;

public enum ImportFileStatus
{
    /// <summary>
    /// Data being collected in the Wizard
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Wizard completed and file uploaded to blob storage - ready for staging
    /// </summary>
    Uploaded = 2,

    /// <summary>
    /// Records from source file being read and copied into the staging table
    /// </summary>
    StagingInProgress = 3,

    /// <summary>
    /// All records from the source file are entered in the staging table - ready for import
    /// </summary>
    StagingCompleted = 4,

    /// <summary>
    /// Import process in progress - records from staging table are copied to respective tables
    /// </summary>
    ImportInProgress = 5,

    /// <summary>
    /// All records successfully imported to respective tables
    /// </summary>
    Imported = 6,

    /// <summary>
    /// Staging failed due to some error
    /// </summary>
    StagingFailed = 7,

    /// <summary>
    /// Certain records successfully imported, while some had errors
    /// </summary>
    PartiallyImported = 8,

    /// <summary>
    /// No records were imported
    /// </summary>
    ImportFailed = 9,

    /// <summary>
    /// Rejected/Discarded by user (TBD)
    /// </summary>
    Rejected = 10
}
