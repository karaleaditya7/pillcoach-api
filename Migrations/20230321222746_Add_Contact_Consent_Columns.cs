using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class Add_Contact_Consent_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "consentForCall",
                table: "contact",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "consentForEmail",
                table: "contact",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "consentForText",
                table: "contact",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "consentForCall",
                table: "contact");

            migrationBuilder.DropColumn(
                name: "consentForEmail",
                table: "contact");

            migrationBuilder.DropColumn(
                name: "consentForText",
                table: "contact");
        }
    }
}
