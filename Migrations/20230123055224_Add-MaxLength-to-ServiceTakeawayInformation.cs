using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddMaxLengthtoServiceTakeawayInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "patientTakeawayDeliveryDate",
                table: "serviceTakeawayInformation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cmrCompleted",
                table: "serviceTakeawayInformation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "patientTakeawayDeliveryDate",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "cmrCompleted",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
