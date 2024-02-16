using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Columns_Birthday_Sms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "twilioSmsNumber",
                table: "pharmacy",
                type: "varchar(15)",
                unicode: false,
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "consentForBirthdaySms",
                table: "contact",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "twilioSmsNumber",
                table: "pharmacy");

            migrationBuilder.DropColumn(
                name: "consentForBirthdaySms",
                table: "contact");
        }
    }
}
