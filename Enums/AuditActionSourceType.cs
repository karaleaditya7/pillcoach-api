namespace OntrackDb.Enums;

public enum AuditActionSourceType
{
    PatientProfile = 1,
    PatientReportedMedications = 2,
    HealthConditions = 3,
    PatientInbox = 4,
    CompanyInbox = 5,
    RefillRequest = 6,
    DoctorPhone = 7,
    DoctorFax = 8,
    PharmacyCall = 9,
    PatientPhone = 10,
    PatientEmail = 11,
    Appointments = 12,
    PatientDocument = 13,
    PatientNote = 14,
    PharmacyNote = 15,
    PatientConsent = 16,
    PatientCMR = 17,
    PatientMedRec = 18,
    AccountLogin = 19,
    AccountLogout = 20
}
