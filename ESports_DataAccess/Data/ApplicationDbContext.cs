using ESports_Models;
using Microsoft.EntityFrameworkCore;

namespace ESports_DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {


        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Headphones", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Mouse", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Keyboard", DisplayOrder = 3 }

                );

            modelBuilder.Entity<Product>().HasData(
    new
    {
        Id = 1,
        ProductName = "Razer DeathAdder V2",
        CompanyName = "Razer",
        Price = 900.0,
        Description = "Ergonomic gaming mouse with high-precision 20K DPI optical sensor and customizable RGB lighting.",
        CategoryId = 1,
        ImageUrl = ""
    },
    new
    {
        Id = 2,
        ProductName = "SteelSeries Apex Pro",
        CompanyName = "SteelSeries",
        Price = 1800.0,
        Description = "Mechanical gaming keyboard with adjustable actuation and OLED smart display.",
        CategoryId = 2,
        ImageUrl = ""
    },
    new
    {
        Id = 3,
        ProductName = "Logitech G Pro X",
        CompanyName = "Logitech",
        Price = 1200.0,
        Description = "High-performance wired gaming headset with Blue Voice microphone technology and 7.1 surround sound.",
        CategoryId = 3,
        ImageUrl = ""
    },
    new
    {
        Id = 4,
        ProductName = "Corsair MM300 Mousepad",
        CompanyName = "Corsair",
        Price = 3500.0,
        Description = "Anti-fray cloth gaming mousepad with superior control and glide optimization for competitive play.",
        CategoryId = 2,
        ImageUrl = ""
    },
    new
    {
        Id = 5,
        ProductName = "HyperX Cloud II",
        CompanyName = "HyperX",
        Price = 1000.0,
        Description = "Comfortable gaming headset with 53mm drivers, virtual 7.1 surround sound, and noise-canceling mic.",
        CategoryId = 3,
        ImageUrl = ""
    },
    new
    {
        Id = 6,
        ProductName = "ASUS ROG Strix Scope RX",
        CompanyName = "ASUS",
        Price = 1300.0,
        Description = "Gaming mechanical keyboard with ROG RX optical switches, RGB lighting, and IP57 water resistance.",
        CategoryId = 2,
        ImageUrl = ""
    }
);
        }
    }
}