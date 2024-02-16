using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SideEffectCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sideEffect_patient_PatientId",
                table: "sideEffect");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "sideEffect",
                newName: "patientId");

            migrationBuilder.RenameIndex(
                name: "IX_sideEffect_PatientId",
                table: "sideEffect",
                newName: "IX_sideEffect_patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_sideEffect_patient_patientId",
                table: "sideEffect",
                column: "patientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sideEffect_patient_patientId",
                table: "sideEffect");

            migrationBuilder.RenameColumn(
                name: "patientId",
                table: "sideEffect",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_sideEffect_patientId",
                table: "sideEffect",
                newName: "IX_sideEffect_PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_sideEffect_patient_PatientId",
                table: "sideEffect",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id");
        }
    }
}
