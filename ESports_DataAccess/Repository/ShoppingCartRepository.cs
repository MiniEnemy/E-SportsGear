using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using Microsoft.EntityFrameworkCore;

namespace ESports_DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
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

        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
        public void Remove(ShoppingCart entity)
        {
            _db.Set<ShoppingCart>().Remove(entity);
        }

        public async Task RemoveRangeAsync(IEnumerable<ShoppingCart> shoppingCarts)
        {
            _db.ShoppingCarts.RemoveRange(shoppingCarts);
            await _db.SaveChangesAsync();
        }
    }
}
