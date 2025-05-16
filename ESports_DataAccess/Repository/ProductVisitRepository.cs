using ESports_DataAccess.Data;
using ESports_Models;
using ESports_DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository
{
    public class ProductVisitRepository : Repository<ProductVisit>, IProductVisitRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductVisitRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ProductVisit> GetVisitAsync(int productId, string userId)
        {
            return await _db.ProductVisits
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.ApplicationUserId == userId);
        }

        public async Task UpdateAsync(ProductVisit visit)
        {
            _db.ProductVisits.Update(visit);
            await _db.SaveChangesAsync();
        }
    }
}