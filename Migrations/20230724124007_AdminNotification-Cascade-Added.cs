using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AdminNotificationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_appointment_AppointmentID",
                table: "adminNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_AspNetUsers_UserId",
                table: "adminNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_message_MessageId",
                table: "adminNotification");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "adminNotification",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "adminNotification",
                newName: "messageId");

            migrationBuilder.RenameColumn(
                name: "AppointmentID",
                table: "adminNotification",
                newName: "appointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_UserId",
                table: "adminNotification",
                newName: "IX_adminNotification_userId");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_MessageId",
                table: "adminNotification",
                newName: "IX_adminNotification_messageId");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_AppointmentID",
                table: "adminNotification",
                newName: "IX_adminNotification_appointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_appointment_appointmentId",
                table: "adminNotification",
                column: "appointmentId",
                principalTable: "appointment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_AspNetUsers_userId",
                table: "adminNotification",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_message_messageId",
                table: "adminNotification",
                column: "messageId",
                principalTable: "message",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_appointment_appointmentId",
                table: "adminNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_AspNetUsers_userId",
                table: "adminNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_adminNotification_message_messageId",
                table: "adminNotification");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "adminNotification",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "messageId",
                table: "adminNotification",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "appointmentId",
                table: "adminNotification",
                newName: "AppointmentID");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_userId",
                table: "adminNotification",
                newName: "IX_adminNotification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_messageId",
                table: "adminNotification",
                newName: "IX_adminNotification_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_adminNotification_appointmentId",
                table: "adminNotification",
                newName: "IX_adminNotification_AppointmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_appointment_AppointmentID",
                table: "adminNotification",
                column: "AppointmentID",
                principalTable: "appointment",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_AspNetUsers_UserId",
                table: "adminNotification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_adminNotification_message_MessageId",
                table: "adminNotification",
                column: "MessageId",
                principalTable: "message",
                principalColumn: "Id");
        }
    }
}
