using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedNotificationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "notification",
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
                    notificationType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_appointment_AppointmentID",
                        column: x => x.AppointmentID,
                        principalTable: "appointment",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_notification_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_notification_message_MessageId",
                        column: x => x.MessageId,
                        principalTable: "message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_notification_AppointmentID",
                table: "notification",
                column: "AppointmentID");

            migrationBuilder.CreateIndex(
                name: "IX_notification_MessageId",
                table: "notification",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_UserId",
                table: "notification",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notification");
        }
    }
}
