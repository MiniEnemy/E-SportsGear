using E_SportsGearHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_SportsGearHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) :base(options)
        {

            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Headphones", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Mouse", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Keyboard", DisplayOrder = 3 }

                );
        }
    }
}
