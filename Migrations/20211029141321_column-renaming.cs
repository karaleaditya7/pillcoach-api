using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class columnrenaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("ImportDataid", "doctor", "importDataId", "dbo");
            migrationBuilder.RenameColumn("ContactId", "doctor", "contactId", "dbo");
            migrationBuilder.RenameColumn("DoctorsId", "doctorPharmacy", "doctorsId", "dbo");
            migrationBuilder.RenameColumn("PharmaciesId", "doctorPharmacy", "pharmaciesId", "dbo");

            migrationBuilder.RenameColumn("ImportDataid", "medication", "importDataId", "dbo");
            migrationBuilder.RenameColumn("PatientId", "medication", "patientId", "dbo");

            migrationBuilder.RenameColumn("ImportDataid", "patient", "importDataId", "dbo");
            migrationBuilder.RenameColumn("AddressId", "patient", "addressId", "dbo");
            migrationBuilder.RenameColumn("PharmacyId", "patient", "pharmacyId", "dbo");
            migrationBuilder.RenameColumn("NoteId", "patient", "noteId", "dbo");
            migrationBuilder.RenameColumn("ContactId", "patient", "contactId", "dbo");

            migrationBuilder.RenameColumn("ImportDataid", "pharmacy", "importDataId", "dbo");
            migrationBuilder.RenameColumn("AddressId", "pharmacy", "addressId", "dbo");
            migrationBuilder.RenameColumn("ContactId", "pharmacy", "contactId", "dbo");

            migrationBuilder.RenameTable(
                name: "DoctorPharmacy",
                newName: "doctorPharmacy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("importDataId", "doctor", "ImportDataid");
            migrationBuilder.RenameColumn("contactId", "doctor", "ContactId"); 
            migrationBuilder.RenameColumn("doctorsId", "doctorPharmacy", "DoctorsId"); 

            migrationBuilder.RenameColumn("importDataId", "medication", "ImportDataid");
            migrationBuilder.RenameColumn("patientId", "medication", "PatientId");

            migrationBuilder.RenameColumn("importDataId", "patient", "ImportDataid");
            migrationBuilder.RenameColumn("addressId", "patient", "AddressId");
            migrationBuilder.RenameColumn("pharmacyId", "patient", "PharmacyId");
            migrationBuilder.RenameColumn("noteId", "patient", "NoteId");
            migrationBuilder.RenameColumn("contactId", "patient", "ContactId");

            migrationBuilder.RenameColumn("importDataId", "pharmacy", "ImportDataid");
            migrationBuilder.RenameColumn("addressId", "pharmacy", "AddressId");
            migrationBuilder.RenameColumn("contactId", "pharmacy", "ContactId");

            migrationBuilder.RenameTable(
                name: "doctorPharmacy",
                newName: "DoctorPharmacy");
        }
    }
}
