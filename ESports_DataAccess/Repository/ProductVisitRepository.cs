using ESports_Models;
using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository
{
    public class ProductVisitRepository : Repository<ProductVisit>, IProductVisitRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductVisitRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductVisit> GetVisitAsync(int productId, string userId)
        {
            return await _context.ProductVisits
                .Where(v => v.ProductId == productId && v.ApplicationUserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(ProductVisit entity)
        {
            _context.ProductVisits.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
