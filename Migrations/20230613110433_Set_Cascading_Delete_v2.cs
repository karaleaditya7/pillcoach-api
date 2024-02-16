using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Set_Cascading_Delete_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCallInfo_AspNetUsers_UserId",
                table: "PatientCallInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCallInfo_patient_PatientId",
                table: "PatientCallInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_webSocket_AspNetUsers_UserId",
                table: "webSocket");

            migrationBuilder.AddForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCallInfo_AspNetUsers_UserId",
                table: "PatientCallInfo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCallInfo_patient_PatientId",
                table: "PatientCallInfo",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_webSocket_AspNetUsers_UserId",
                table: "webSocket",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCallInfo_AspNetUsers_UserId",
                table: "PatientCallInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientCallInfo_patient_PatientId",
                table: "PatientCallInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_webSocket_AspNetUsers_UserId",
                table: "webSocket");

            migrationBuilder.AddForeignKey(
                name: "FK_notes_AspNetUsers_userId",
                table: "notes",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCallInfo_AspNetUsers_UserId",
                table: "PatientCallInfo",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientCallInfo_patient_PatientId",
                table: "PatientCallInfo",
                column: "PatientId",
                principalTable: "patient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_webSocket_AspNetUsers_UserId",
                table: "webSocket",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
