using Microsoft.EntityFrameworkCore.Migrations;

namespace OntrackDb.Migrations
{
    public partial class columnnametocamelCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Licenses_LicensesId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPharmacy_Doctors_DoctorsId",
                table: "DoctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPharmacy_Pharmacies_PharmaciesId",
                table: "DoctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_ImportDatas_ImportDataid",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_ImportDatas_ImportDataid",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Address_AddressId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_ImportDatas_ImportDataid",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Notes_NoteId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Pharmacies_PharmacyId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_Address_AddressId",
                table: "Pharmacies");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_Contacts_ContactId",
                table: "Pharmacies");

            migrationBuilder.DropForeignKey(
                name: "FK_Pharmacies_ImportDatas_ImportDataid",
                table: "Pharmacies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Pharmacies",
                table: "Pharmacies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notes",
                table: "Notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Medications",
                table: "Medications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportDatas",
                table: "ImportDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "Licenses",
                newName: "licenses");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "address");

            migrationBuilder.RenameTable(
                name: "Pharmacies",
                newName: "pharmacy");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "patient");

            migrationBuilder.RenameTable(
                name: "Notes",
                newName: "notes");

            migrationBuilder.RenameTable(
                name: "Medications",
                newName: "medication");

            migrationBuilder.RenameTable(
                name: "ImportDatas",
                newName: "importData");

            migrationBuilder.RenameTable(
                name: "Doctors",
                newName: "doctor");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "contact");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "licenses",
                newName: "number");

            migrationBuilder.RenameColumn(
                name: "IssueState",
                table: "licenses",
                newName: "issueState");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "licenses",
                newName: "expirationDate");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "LastLogin",
                table: "AspNetUsers",
                newName: "lastLogin");

            migrationBuilder.RenameColumn(
                name: "JobPosition",
                table: "AspNetUsers",
                newName: "jobPosition");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "AspNetUsers",
                newName: "imageName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "firstName");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "AspNetUsers",
                newName: "dateOfBirth");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "address",
                newName: "zipCode");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "address",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "address",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "AddressLineTwo",
                table: "address",
                newName: "addressLineTwo");

            migrationBuilder.RenameColumn(
                name: "AddressLineOne",
                table: "address",
                newName: "addressLineOne");

            migrationBuilder.RenameColumn(
                name: "PharmacyManager",
                table: "pharmacy",
                newName: "pharmacyManager");

            migrationBuilder.RenameColumn(
                name: "NpiNumber",
                table: "pharmacy",
                newName: "npiNumber");

            migrationBuilder.RenameColumn(
                name: "NcpdpNumber",
                table: "pharmacy",
                newName: "ncpdpNumber");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "pharmacy",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "LastUpdate",
                table: "pharmacy",
                newName: "lastUpdate");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "pharmacy",
                newName: "imageName");

            migrationBuilder.RenameIndex(
                name: "IX_Pharmacies_ImportDataid",
                table: "pharmacy",
                newName: "IX_pharmacy_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_Pharmacies_ContactId",
                table: "pharmacy",
                newName: "IX_pharmacy_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Pharmacies_AddressId",
                table: "pharmacy",
                newName: "IX_pharmacy_AddressId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "patient",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "patient",
                newName: "imageName");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_PharmacyId",
                table: "patient",
                newName: "IX_patient_PharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_NoteId",
                table: "patient",
                newName: "IX_patient_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_ImportDataid",
                table: "patient",
                newName: "IX_patient_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_ContactId",
                table: "patient",
                newName: "IX_patient_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_AddressId",
                table: "patient",
                newName: "IX_patient_AddressId");

            migrationBuilder.RenameColumn(
                name: "Supply",
                table: "medication",
                newName: "supply");

            migrationBuilder.RenameColumn(
                name: "RxNumber",
                table: "medication",
                newName: "rxNumber");

            migrationBuilder.RenameColumn(
                name: "RxDate",
                table: "medication",
                newName: "rxDate");

            migrationBuilder.RenameColumn(
                name: "RfNumber",
                table: "medication",
                newName: "rfNumber");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "medication",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "PrescriberName",
                table: "medication",
                newName: "prescriberName");

            migrationBuilder.RenameColumn(
                name: "PayDue",
                table: "medication",
                newName: "payDue");

            migrationBuilder.RenameColumn(
                name: "NextFillDate",
                table: "medication",
                newName: "nextFillDate");

            migrationBuilder.RenameColumn(
                name: "LastFillDate",
                table: "medication",
                newName: "lastFillDate");

            migrationBuilder.RenameColumn(
                name: "DrugName",
                table: "medication",
                newName: "drugName");

            migrationBuilder.RenameColumn(
                name: "Direction",
                table: "medication",
                newName: "direction");

            migrationBuilder.RenameColumn(
                name: "ConditionTreated",
                table: "medication",
                newName: "conditionTreated");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_PatientId",
                table: "medication",
                newName: "IX_medication_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Medications_ImportDataid",
                table: "medication",
                newName: "IX_medication_ImportDataid");

            migrationBuilder.RenameColumn(
                name: "Npi",
                table: "doctor",
                newName: "npi");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_ImportDataid",
                table: "doctor",
                newName: "IX_doctor_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_Doctors_ContactId",
                table: "doctor",
                newName: "IX_doctor_ContactId");

            migrationBuilder.RenameColumn(
                name: "SecondaryPhone",
                table: "contact",
                newName: "secondaryPhone");

            migrationBuilder.RenameColumn(
                name: "SecondaryEmail",
                table: "contact",
                newName: "secondaryEmail");

            migrationBuilder.RenameColumn(
                name: "PrimaryPhone",
                table: "contact",
                newName: "primaryPhone");

            migrationBuilder.RenameColumn(
                name: "PrimaryEmail",
                table: "contact",
                newName: "primaryEmail");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "contact",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "contact",
                newName: "firstName");

            migrationBuilder.RenameColumn(
                name: "Fax",
                table: "contact",
                newName: "fax");

            migrationBuilder.RenameColumn(
                name: "DoB",
                table: "contact",
                newName: "dob");

            migrationBuilder.AlterColumn<int>(
                name: "PharmacyId",
                table: "patient",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "medication",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_licenses",
                table: "licenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_address",
                table: "address",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_pharmacy",
                table: "pharmacy",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_patient",
                table: "patient",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notes",
                table: "notes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_medication",
                table: "medication",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_importData",
                table: "importData",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_doctor",
                table: "doctor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contact",
                table: "contact",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_address_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_licenses_LicensesId",
                table: "AspNetUsers",
                column: "LicensesId",
                principalTable: "licenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_doctor_contact_ContactId",
                table: "doctor",
                column: "ContactId",
                principalTable: "contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_doctor_importData_ImportDataid",
                table: "doctor",
                column: "ImportDataid",
                principalTable: "importData",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPharmacy_doctor_DoctorsId",
                table: "DoctorPharmacy",
                column: "DoctorsId",
                principalTable: "doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPharmacy_pharmacy_PharmaciesId",
                table: "DoctorPharmacy",
                column: "PharmaciesId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_medication_importData_ImportDataid",
                table: "medication",
                column: "ImportDataid",
                principalTable: "importData",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_medication_patient_PatientId",
                table: "medication",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_address_AddressId",
                table: "patient",
                column: "AddressId",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_contact_ContactId",
                table: "patient",
                column: "ContactId",
                principalTable: "contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_importData_ImportDataid",
                table: "patient",
                column: "ImportDataid",
                principalTable: "importData",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_note_NoteId",
                table: "patient",
                column: "NoteId",
                principalTable: "notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_patient_pharmacy_PharmacyId",
                table: "patient",
                column: "PharmacyId",
                principalTable: "pharmacy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pharmacy_address_AddressId",
                table: "pharmacy",
                column: "AddressId",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pharmacy_contact_ContactId",
                table: "pharmacy",
                column: "ContactId",
                principalTable: "contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_pharmacy_importData_ImportDataid",
                table: "pharmacy",
                column: "ImportDataid",
                principalTable: "importData",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_address_AddressId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_licenses_LicensesId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_doctor_contact_ContactId",
                table: "doctor");

            migrationBuilder.DropForeignKey(
                name: "FK_doctor_importData_ImportDataid",
                table: "doctor");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPharmacy_doctor_DoctorsId",
                table: "DoctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorPharmacy_pharmacy_PharmaciesId",
                table: "DoctorPharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_medication_importData_ImportDataid",
                table: "medication");

            migrationBuilder.DropForeignKey(
                name: "FK_medication_patient_PatientId",
                table: "medication");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_address_AddressId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_contact_ContactId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_importData_ImportDataid",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_note_NoteId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_patient_pharmacy_PharmacyId",
                table: "patient");

            migrationBuilder.DropForeignKey(
                name: "FK_pharmacy_address_AddressId",
                table: "pharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_pharmacy_contact_ContactId",
                table: "pharmacy");

            migrationBuilder.DropForeignKey(
                name: "FK_pharmacy_importData_ImportDataid",
                table: "pharmacy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_licenses",
                table: "licenses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_address",
                table: "address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_pharmacy",
                table: "pharmacy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_patient",
                table: "patient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_note",
                table: "notes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_medication",
                table: "medication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_importData",
                table: "importData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_doctor",
                table: "doctor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_contact",
                table: "contact");

            migrationBuilder.RenameTable(
                name: "licenses",
                newName: "Licenses");

            migrationBuilder.RenameTable(
                name: "address",
                newName: "Address");

            migrationBuilder.RenameTable(
                name: "pharmacy",
                newName: "Pharmacies");

            migrationBuilder.RenameTable(
                name: "patient",
                newName: "Patients");

            migrationBuilder.RenameTable(
                name: "note",
                newName: "Notes");

            migrationBuilder.RenameTable(
                name: "medication",
                newName: "Medications");

            migrationBuilder.RenameTable(
                name: "importData",
                newName: "ImportDatas");

            migrationBuilder.RenameTable(
                name: "doctor",
                newName: "Doctors");

            migrationBuilder.RenameTable(
                name: "contact",
                newName: "Contacts");

            migrationBuilder.RenameColumn(
                name: "number",
                table: "Licenses",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "issueState",
                table: "Licenses",
                newName: "IssueState");

            migrationBuilder.RenameColumn(
                name: "expirationDate",
                table: "Licenses",
                newName: "ExpirationDate");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "lastLogin",
                table: "AspNetUsers",
                newName: "LastLogin");

            migrationBuilder.RenameColumn(
                name: "jobPosition",
                table: "AspNetUsers",
                newName: "JobPosition");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "AspNetUsers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "imageName",
                table: "AspNetUsers",
                newName: "ImageName");

            migrationBuilder.RenameColumn(
                name: "firstName",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "dateOfBirth",
                table: "AspNetUsers",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "zipCode",
                table: "Address",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "Address",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "Address",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "addressLineTwo",
                table: "Address",
                newName: "AddressLineTwo");

            migrationBuilder.RenameColumn(
                name: "addressLineOne",
                table: "Address",
                newName: "AddressLineOne");

            migrationBuilder.RenameColumn(
                name: "pharmacyManager",
                table: "Pharmacies",
                newName: "PharmacyManager");

            migrationBuilder.RenameColumn(
                name: "npiNumber",
                table: "Pharmacies",
                newName: "NpiNumber");

            migrationBuilder.RenameColumn(
                name: "ncpdpNumber",
                table: "Pharmacies",
                newName: "NcpdpNumber");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Pharmacies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "lastUpdate",
                table: "Pharmacies",
                newName: "LastUpdate");

            migrationBuilder.RenameColumn(
                name: "imageName",
                table: "Pharmacies",
                newName: "ImageName");

            migrationBuilder.RenameIndex(
                name: "IX_pharmacy_ImportDataid",
                table: "Pharmacies",
                newName: "IX_Pharmacies_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_pharmacy_ContactId",
                table: "Pharmacies",
                newName: "IX_Pharmacies_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_pharmacy_AddressId",
                table: "Pharmacies",
                newName: "IX_Pharmacies_AddressId");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Patients",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "imageName",
                table: "Patients",
                newName: "ImageName");

            migrationBuilder.RenameIndex(
                name: "IX_patient_PharmacyId",
                table: "Patients",
                newName: "IX_Patients_PharmacyId");

            migrationBuilder.RenameIndex(
                name: "IX_patient_NoteId",
                table: "Patients",
                newName: "IX_Patients_NoteId");

            migrationBuilder.RenameIndex(
                name: "IX_patient_ImportDataid",
                table: "Patients",
                newName: "IX_Patients_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_patient_ContactId",
                table: "Patients",
                newName: "IX_Patients_ContactId");

            migrationBuilder.RenameIndex(
                name: "IX_patient_AddressId",
                table: "Patients",
                newName: "IX_Patients_AddressId");

            migrationBuilder.RenameColumn(
                name: "supply",
                table: "Medications",
                newName: "Supply");

            migrationBuilder.RenameColumn(
                name: "rxNumber",
                table: "Medications",
                newName: "RxNumber");

            migrationBuilder.RenameColumn(
                name: "rxDate",
                table: "Medications",
                newName: "RxDate");

            migrationBuilder.RenameColumn(
                name: "rfNumber",
                table: "Medications",
                newName: "RfNumber");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "Medications",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "prescriberName",
                table: "Medications",
                newName: "PrescriberName");

            migrationBuilder.RenameColumn(
                name: "payDue",
                table: "Medications",
                newName: "PayDue");

            migrationBuilder.RenameColumn(
                name: "nextFillDate",
                table: "Medications",
                newName: "NextFillDate");

            migrationBuilder.RenameColumn(
                name: "lastFillDate",
                table: "Medications",
                newName: "LastFillDate");

            migrationBuilder.RenameColumn(
                name: "drugName",
                table: "Medications",
                newName: "DrugName");

            migrationBuilder.RenameColumn(
                name: "direction",
                table: "Medications",
                newName: "Direction");

            migrationBuilder.RenameColumn(
                name: "conditionTreated",
                table: "Medications",
                newName: "ConditionTreated");

            migrationBuilder.RenameIndex(
                name: "IX_medication_PatientId",
                table: "Medications",
                newName: "IX_Medications_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_medication_ImportDataid",
                table: "Medications",
                newName: "IX_Medications_ImportDataid");

            migrationBuilder.RenameColumn(
                name: "npi",
                table: "Doctors",
                newName: "Npi");

            migrationBuilder.RenameIndex(
                name: "IX_doctor_ImportDataid",
                table: "Doctors",
                newName: "IX_Doctors_ImportDataid");

            migrationBuilder.RenameIndex(
                name: "IX_doctor_ContactId",
                table: "Doctors",
                newName: "IX_Doctors_ContactId");

            migrationBuilder.RenameColumn(
                name: "secondaryPhone",
                table: "Contacts",
                newName: "SecondaryPhone");

            migrationBuilder.RenameColumn(
                name: "secondaryEmail",
                table: "Contacts",
                newName: "SecondaryEmail");

            migrationBuilder.RenameColumn(
                name: "primaryPhone",
                table: "Contacts",
                newName: "PrimaryPhone");

            migrationBuilder.RenameColumn(
                name: "primaryEmail",
                table: "Contacts",
                newName: "PrimaryEmail");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "Contacts",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "firstName",
                table: "Contacts",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "fax",
                table: "Contacts",
                newName: "Fax");

            migrationBuilder.RenameColumn(
                name: "dob",
                table: "Contacts",
                newName: "DoB");

            migrationBuilder.AlterColumn<int>(
                name: "PharmacyId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientId",
                table: "Medications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Licenses",
                table: "Licenses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pharmacies",
                table: "Pharmacies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notes",
                table: "Notes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Medications",
                table: "Medications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportDatas",
                table: "ImportDatas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Address_AddressId",
                table: "AspNetUsers",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Licenses_LicensesId",
                table: "AspNetUsers",
                column: "LicensesId",
                principalTable: "Licenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPharmacy_Doctors_DoctorsId",
                table: "DoctorPharmacy",
                column: "DoctorsId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorPharmacy_Pharmacies_PharmaciesId",
                table: "DoctorPharmacy",
                column: "PharmaciesId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Contacts_ContactId",
                table: "Doctors",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_ImportDatas_ImportDataid",
                table: "Doctors",
                column: "ImportDataid",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_ImportDatas_ImportDataid",
                table: "Medications",
                column: "ImportDataid",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Medications_Patients_PatientId",
                table: "Medications",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Address_AddressId",
                table: "Patients",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Contacts_ContactId",
                table: "Patients",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_ImportDatas_ImportDataid",
                table: "Patients",
                column: "ImportDataid",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Notes_NoteId",
                table: "Patients",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Pharmacies_PharmacyId",
                table: "Patients",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_Address_AddressId",
                table: "Pharmacies",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_Contacts_ContactId",
                table: "Pharmacies",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pharmacies_ImportDatas_ImportDataid",
                table: "Pharmacies",
                column: "ImportDataid",
                principalTable: "ImportDatas",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
