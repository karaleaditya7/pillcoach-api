using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedNewEntityMedicationToDoListRelatedNonRelated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "medicationRelated");

            //migrationBuilder.DropTable(
            //    name: "medicationToDoList");

            migrationBuilder.CreateTable(
                name: "actionItemToDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actionItemToDo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medicationToDoRelated",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CmrMedicationId = table.Column<int>(type: "int", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationToDoRelated", x => x.Id);
                    table.ForeignKey(
                        name: "FK_medicationToDoRelated_cmrMedication_CmrMedicationId",
                        column: x => x.CmrMedicationId,
                        principalTable: "cmrMedication",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_medicationToDoRelated_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "nonRelatedMedicationToDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    ActionItemToDoId = table.Column<int>(type: "int", nullable: true),
                    patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nonRelatedMedicationToDo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_nonRelatedMedicationToDo_actionItemToDo_ActionItemToDoId",
                        column: x => x.ActionItemToDoId,
                        principalTable: "actionItemToDo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_nonRelatedMedicationToDo_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_medicationToDoRelated_CmrMedicationId",
                table: "medicationToDoRelated",
                column: "CmrMedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_medicationToDoRelated_PatientId",
                table: "medicationToDoRelated",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_nonRelatedMedicationToDo_ActionItemToDoId",
                table: "nonRelatedMedicationToDo",
                column: "ActionItemToDoId");

            migrationBuilder.CreateIndex(
                name: "IX_nonRelatedMedicationToDo_PatientId",
                table: "nonRelatedMedicationToDo",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "medicationToDoRelated");

            migrationBuilder.DropTable(
                name: "nonRelatedMedicationToDo");

            migrationBuilder.DropTable(
                name: "actionItemToDo");

            migrationBuilder.CreateTable(
                name: "medicationRelated",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationRelated", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "medicationToDoList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CmrMedicationId = table.Column<int>(type: "int", nullable: true),
                    patientToDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medicationToDoList", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_medicationToDoList_CmrMedicationId",
                table: "medicationToDoList",
                column: "CmrMedicationId");
        }
    }
}
