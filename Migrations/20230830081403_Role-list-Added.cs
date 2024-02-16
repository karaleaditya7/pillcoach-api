using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class RolelistAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
              table: "AspNetRoles",
              columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
              values: new object[,]
              {
                    { "1","Employee","Employee", Guid.NewGuid().ToString()},
                    { "2","Admin","Admin",Guid.NewGuid().ToString() },
                    { "3","SuperAdmin","SuperAdmin",Guid.NewGuid().ToString()},
              });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
               table: "AspNetRoles",
               keyColumns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
               keyValues: new object[,]
               {
                    { "1","Employee", "Employee",Guid.NewGuid().ToString() },
                    { "2","Admin", "Admin",Guid.NewGuid().ToString() },
                    { "3","SuperAdmin", "SuperAdmin",Guid.NewGuid().ToString() }
               });
        }
    }
}
