using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
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

        public async Task<ProductVisit> GetVisitAsync(int productId, string applicationUserId)
        {
            return await _db.ProductVisits
                .FirstOrDefaultAsync(v => v.ProductId == productId && v.ApplicationUserId == applicationUserId);
        }
    }
}
