using ESports_Models;
using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;

namespace ESports_DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
