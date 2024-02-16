using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedPropertyDescriptionCognitivelyImpairedTakeaway : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "descriptionCognitivelyImpaired",
                table: "serviceTakeawayInformation",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descriptionCognitivelyImpaired",
                table: "serviceTakeawayInformation");
        }
    }
}
