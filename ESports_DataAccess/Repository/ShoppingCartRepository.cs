using ESports_Models;
using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;

namespace ESports_DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
