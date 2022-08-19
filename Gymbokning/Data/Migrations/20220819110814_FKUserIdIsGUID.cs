using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gymbokning.Data.Migrations
{
    public partial class FKUserIdIsGUID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId1",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserGymClass_ApplicationUserId1",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "ApplicationUserGymClass");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ApplicationUserGymClass",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 19, 14, 8, 14, 241, DateTimeKind.Local).AddTicks(1809));

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserGymClass_ApplicationUserId",
                table: "ApplicationUserGymClass",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGymClass",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId",
                table: "ApplicationUserGymClass");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUserGymClass_ApplicationUserId",
                table: "ApplicationUserGymClass");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationUserId",
                table: "ApplicationUserGymClass",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "ApplicationUserGymClass",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "GymClass",
                keyColumn: "Id",
                keyValue: 1,
                column: "StartTime",
                value: new DateTime(2022, 8, 19, 10, 17, 35, 278, DateTimeKind.Local).AddTicks(8825));

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserGymClass_ApplicationUserId1",
                table: "ApplicationUserGymClass",
                column: "ApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUserGymClass_AspNetUsers_ApplicationUserId1",
                table: "ApplicationUserGymClass",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
