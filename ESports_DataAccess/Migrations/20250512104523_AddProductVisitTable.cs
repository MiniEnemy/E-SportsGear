using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVisitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.DropIndex(
                name: "IX_ProductVisits_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastVisited",
                table: "ProductVisits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastVisited",
                table: "ProductVisits",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "IpAddress",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVisits_ApplicationUserId",
                table: "ProductVisits",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
