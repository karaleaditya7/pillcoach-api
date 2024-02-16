using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class AddedEntityPdc_Medication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pdc_Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value_set_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value_set_subgroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value_set_item = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    route = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dosage_form = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ingredient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    strength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    units = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    from_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thru_date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    attribute_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    attribute_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pdc_Medications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pdc_Medications");
        }
    }
}
