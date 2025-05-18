using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ESports_DataAccess.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // IMPORTANT: SetBasePath to the UI project folder where appsettings.json actually is
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\E-SportsGearHub");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)  // <-- Adjusted path here
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }
}
