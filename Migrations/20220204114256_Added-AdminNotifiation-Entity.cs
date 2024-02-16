using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedAdminNotifiationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adminNotification",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    AppointmentID = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sendDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    readDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    notificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminNotification", x => x.id);
                    table.ForeignKey(
                        name: "FK_adminNotification_appointment_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "appointment",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_adminNotification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_adminNotification_message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_adminNotification_AppointmentID",
                table: "adminNotification",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_adminNotification_MessageId",
                table: "adminNotification",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_adminNotification_UserId",
                table: "adminNotification",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminNotification");
        }
    }
}
