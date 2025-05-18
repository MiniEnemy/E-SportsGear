using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports_Models;

namespace ESports_DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductVisit> ProductVisits { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category seed data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Headphones", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Mouse", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Keyboard", DisplayOrder = 3 }
            );

            // Precision for financial fields
            modelBuilder.Entity<OrderDetail>()
                .Property(o => o.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderHeader>()
                .Property(o => o.OrderTotal)
                .HasColumnType("decimal(18,2)");
        }
    }
}
