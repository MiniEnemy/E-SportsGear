using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;

namespace ESports_DataAccess.Repository
{
    public class CartRepository : Repository<Cart>
        //, ICartRepository
    {
        private readonly ApplicationDbContext _db;

        public CartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //public void Update(Cart cart)
        //{
        //    _db.Carts.Update(cart);
        //}
    }
}
