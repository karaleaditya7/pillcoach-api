using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class NonRelatedReconciliationToDoCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nonRelatedRecocilationToDo_patient_PatientId",
                table: "nonRelatedRecocilationToDo");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "nonRelatedRecocilationToDo",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_nonRelatedRecocilationToDo_PatientId",
                table: "nonRelatedRecocilationToDo",
                newName: "IX_nonRelatedRecocilationToDo_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_nonRelatedRecocilationToDo_patient_patientId",
                table: "nonRelatedRecocilationToDo",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nonRelatedRecocilationToDo_patient_patientId",
                table: "nonRelatedRecocilationToDo");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "nonRelatedRecocilationToDo",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_nonRelatedRecocilationToDo_patientId",
                table: "nonRelatedRecocilationToDo",
                newName: "IX_nonRelatedRecocilationToDo_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_nonRelatedRecocilationToDo_patient_PatientId",
                table: "nonRelatedRecocilationToDo",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
