using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class NotificationCascadeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notification_appointment_AppointmentID",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_AspNetUsers_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_message_MessageId",
                table: "notification");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "notification",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "notification",
                newName: "messageId");

            migrationBuilder.RenameColumn(
                name: "AppointmentID",
                table: "notification",
                newName: "appointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_notification_UserId",
                table: "notification",
                newName: "IX_notification_userId");

            migrationBuilder.RenameIndex(
                name: "IX_notification_MessageId",
                table: "notification",
                newName: "IX_notification_messageId");

            migrationBuilder.RenameIndex(
                name: "IX_notification_AppointmentID",
                table: "notification",
                newName: "IX_notification_appointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_appointment_appointmentId",
                table: "notification",
                column: "appointmentId",
                principalTable: "appointment",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_AspNetUsers_userId",
                table: "notification",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_message_messageId",
                table: "notification",
                column: "messageId",
                principalTable: "message",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_notification_appointment_appointmentId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_AspNetUsers_userId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_message_messageId",
                table: "notification");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "notification",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "messageId",
                table: "notification",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "appointmentId",
                table: "notification",
                newName: "AppointmentID");

            migrationBuilder.RenameIndex(
                name: "IX_notification_userId",
                table: "notification",
                newName: "IX_notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_notification_messageId",
                table: "notification",
                newName: "IX_notification_MessageId");

            migrationBuilder.RenameIndex(
                name: "IX_notification_appointmentId",
                table: "notification",
                newName: "IX_notification_AppointmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_appointment_AppointmentID",
                table: "notification",
                column: "AppointmentID",
                principalTable: "appointment",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_AspNetUsers_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notification_message_MessageId",
                table: "notification",
                column: "MessageId",
                principalTable: "message",
                principalColumn: "Id");
        }
    }
}
