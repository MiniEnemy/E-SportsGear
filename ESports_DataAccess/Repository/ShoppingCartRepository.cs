using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;

namespace ESports_DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int IncrementCount(ShoppingCart cart, int count)
        {
            cart.Count += count;
            return cart.Count;
        }

        public int DecrementCount(ShoppingCart cart, int count)
        {
            cart.Count -= count;
            return cart.Count;
        }
    }
}
