using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymbokning.Data.Migrations
{
    public partial class addedFirstNameLastNameTimeOfRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimeOfRegistration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GetDate()");

            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 23, 12, 10, 6, 313, DateTimeKind.Local).AddTicks(8847));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeOfRegistration",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 19, 16, 54, 20, 144, DateTimeKind.Local).AddTicks(124));
        }
    }
}
