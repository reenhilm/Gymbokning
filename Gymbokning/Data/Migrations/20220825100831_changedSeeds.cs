using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymbokning.Data.Migrations
{
    public partial class changedSeeds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "GymClass",
                columns: new[] { "Id", "Description", "Duration", "Name", "StartTime" },
                values: new object[] { 1, "GymClassDescription", new TimeSpan(0, 1, 0, 0, 0), "GymClassName", new DateTime(2022, 8, 24, 11, 25, 54, 186, DateTimeKind.Local).AddTicks(3563) });
        }
    }
}
