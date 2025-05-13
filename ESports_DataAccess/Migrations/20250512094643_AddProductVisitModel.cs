using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVisitModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastVisited",
                table: "ProductVisits",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.DropColumn(
                name: "IpAddress",
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
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
