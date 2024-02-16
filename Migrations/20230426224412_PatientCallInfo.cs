using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class PatientCallInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientCallInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CallDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AttemptNo = table.Column<int>(type: "int", nullable: false),
                    CallReason = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedicationsDiscussedJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCallInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCallInfo_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientCallInfo_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientCallInfo_PatientId",
                table: "PatientCallInfo",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCallInfo_UserId",
                table: "PatientCallInfo",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientCallInfo");
        }
    }
}
