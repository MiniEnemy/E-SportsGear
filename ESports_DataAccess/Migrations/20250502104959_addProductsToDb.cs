using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ESports_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addProductsToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CompanyName", "Description", "Price", "ProductName" },
                values: new object[,]
                {
                    { 1, "Razer", "Ergonomic gaming mouse with high-precision 20K DPI optical sensor and customizable RGB lighting.", 900.0, "Razer DeathAdder V2" },
                    { 2, "SteelSeries", "Mechanical gaming keyboard with adjustable actuation and OLED smart display.", 1800.0, "SteelSeries Apex Pro" },
                    { 3, "Logitech", "High-performance wired gaming headset with Blue Voice microphone technology and 7.1 surround sound.", 1200.0, "Logitech G Pro X" },
                    { 4, "Corsair", "Anti-fray cloth gaming mousepad with superior control and glide optimization for competitive play.", 3500.0, "Corsair MM300 Mousepad" },
                    { 5, "HyperX", "Comfortable gaming headset with 53mm drivers, virtual 7.1 surround sound, and noise-canceling mic.", 1000.0, "HyperX Cloud II" },
                    { 6, "ASUS", "Gaming mechanical keyboard with ROG RX optical switches, RGB lighting, and IP57 water resistance.", 1300.0, "ASUS ROG Strix Scope RX" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
