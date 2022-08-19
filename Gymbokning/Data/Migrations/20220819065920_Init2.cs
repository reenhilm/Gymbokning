using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymbokning.Data.Migrations
{
    public partial class Init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 19, 9, 59, 20, 734, DateTimeKind.Local).AddTicks(6412));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 18, 18, 3, 33, 459, DateTimeKind.Local).AddTicks(5440));
        }
    }
}
