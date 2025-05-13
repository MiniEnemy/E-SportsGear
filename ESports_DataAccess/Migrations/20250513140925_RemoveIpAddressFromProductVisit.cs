using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIpAddressFromProductVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "ProductVisits");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(450)",
                nullable: false,
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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVisits_AspNetUsers_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.DropIndex(
                name: "IX_ProductVisits_ApplicationUserId",
                table: "ProductVisits");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
