using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedCmrMedicationEntites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cmrMedication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rxVendorRxID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    rxDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    drugName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    direction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    DoctorPrescribedId = table.Column<int>(type: "int", nullable: true),
                    supply = table.Column<int>(type: "int", nullable: false),
                    prescriberName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    lastFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nextFillDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    payDue = table.Column<int>(type: "int", nullable: false),
                    rfNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    conditionTreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportDataid = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    optionalCondition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refillsRemaining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ndcNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    drugSubGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    genericName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isInclude = table.Column<bool>(type: "bit", nullable: false),
                    isExclude = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cmrMedication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cmrMedication_doctor_DoctorPrescribedId",
                        column: x => x.DoctorPrescribedId,
                        principalTable: "doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_cmrMedication_importData_ImportDataid",
                        column: x => x.ImportDataid,
                        principalTable: "importData",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_cmrMedication_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            //migrationBuilder.CreateTable(
            //    name: "medicationRelated",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        name = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_medicationRelated", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "medicationToDoList",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CmrMedicationId = table.Column<int>(type: "int", nullable: true),
            //        patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_medicationToDoList", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_medicationToDoList_cmrMedication_CmrMedicationId",
            //            column: x => x.CmrMedicationId,
            //            principalTable: "cmrMedication",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "medicationToDoListNonRelated",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MedicationToDoListId = table.Column<int>(type: "int", nullable: true),
            //        MedicationRelatedId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_medicationToDoListNonRelated", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_medicationToDoListNonRelated_medicationRelated_MedicationRelatedId",
            //            column: x => x.MedicationRelatedId,
            //            principalTable: "medicationRelated",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_medicationToDoListNonRelated_medicationToDoList_MedicationToDoListId",
            //            column: x => x.MedicationToDoListId,
            //            principalTable: "medicationToDoList",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "medicationToDoListRelated",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        MedicationToDoListId = table.Column<int>(type: "int", nullable: true),
            //        MedicationRelatedId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_medicationToDoListRelated", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_medicationToDoListRelated_medicationRelated_MedicationRelatedId",
            //            column: x => x.MedicationRelatedId,
            //            principalTable: "medicationRelated",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_medicationToDoListRelated_medicationToDoList_MedicationToDoListId",
            //            column: x => x.MedicationToDoListId,
            //            principalTable: "medicationToDoList",
            //            principalColumn: "Id");
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_cmrMedication_DoctorPrescribedId",
                table: "cmrMedication",
                column: "DoctorPrescribedId");

            migrationBuilder.CreateIndex(
                name: "IX_cmrMedication_ImportDataid",
                table: "cmrMedication",
                column: "ImportDataid");

            migrationBuilder.CreateIndex(
                name: "IX_cmrMedication_PatientId",
                table: "cmrMedication",
                column: "PatientId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_medicationToDoList_CmrMedicationId",
            //    table: "medicationToDoList",
            //    column: "CmrMedicationId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_medicationToDoListNonRelated_MedicationRelatedId",
            //    table: "medicationToDoListNonRelated",
            //    column: "MedicationRelatedId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_medicationToDoListNonRelated_MedicationToDoListId",
            //    table: "medicationToDoListNonRelated",
            //    column: "MedicationToDoListId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_medicationToDoListRelated_MedicationRelatedId",
            //    table: "medicationToDoListRelated",
            //    column: "MedicationRelatedId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_medicationToDoListRelated_MedicationToDoListId",
            //    table: "medicationToDoListRelated",
            //    column: "MedicationToDoListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "medicationToDoListNonRelated");

            //migrationBuilder.DropTable(
            //    name: "medicationToDoListRelated");

            //migrationBuilder.DropTable(
            //    name: "medicationRelated");

            //migrationBuilder.DropTable(
            //    name: "medicationToDoList");

            migrationBuilder.DropTable(
                name: "cmrMedication");
        }
    }
}
