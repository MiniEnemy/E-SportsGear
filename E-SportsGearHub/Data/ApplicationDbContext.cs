using Microsoft.EntityFrameworkCore;

namespace E_SportsGearHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) :base(options)
        {
            
        }
    }
}
