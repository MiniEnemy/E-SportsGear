using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductVisitToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
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
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductVisits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
