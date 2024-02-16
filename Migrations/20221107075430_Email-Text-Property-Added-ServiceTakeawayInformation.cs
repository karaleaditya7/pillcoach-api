using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class EmailTextPropertyAddedServiceTakeawayInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "serviceTakeawayInformation");

            migrationBuilder.DropColumn(
                name: "text",
                table: "serviceTakeawayInformation");
        }
    }
}
