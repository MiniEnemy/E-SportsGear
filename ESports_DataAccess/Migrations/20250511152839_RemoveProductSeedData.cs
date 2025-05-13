using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductSeedData : Migration
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

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "VisitDate",
                table: "ProductVisits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductVisits");

            migrationBuilder.DropColumn(
                name: "VisitDate",
                table: "ProductVisits");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CompanyName", "Description", "ImageUrl", "Price", "ProductName" },
                values: new object[,]
                {
                    { 1, 1, "Razer", "Ergonomic gaming mouse with high-precision 20K DPI optical sensor and customizable RGB lighting.", "", 900.0, "Razer DeathAdder V2" },
                    { 2, 2, "SteelSeries", "Mechanical gaming keyboard with adjustable actuation and OLED smart display.", "", 1800.0, "SteelSeries Apex Pro" },
                    { 3, 3, "Logitech", "High-performance wired gaming headset with Blue Voice microphone technology and 7.1 surround sound.", "", 1200.0, "Logitech G Pro X" },
                    { 4, 2, "Corsair", "Anti-fray cloth gaming mousepad with superior control and glide optimization for competitive play.", "", 3500.0, "Corsair MM300 Mousepad" },
                    { 5, 3, "HyperX", "Comfortable gaming headset with 53mm drivers, virtual 7.1 surround sound, and noise-canceling mic.", "", 1000.0, "HyperX Cloud II" },
                    { 6, 2, "ASUS", "Gaming mechanical keyboard with ROG RX optical switches, RGB lighting, and IP57 water resistance.", "", 1300.0, "ASUS ROG Strix Scope RX" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVisits_ApplicationUserId",
                table: "ProductVisits",
                column: "ApplicationUserId");

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
