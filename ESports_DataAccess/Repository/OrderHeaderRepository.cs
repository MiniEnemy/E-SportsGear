using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Utility;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }
        public async Task<IEnumerable<OrderHeader>> GetOrdersWithDetailsAsync(string userId)
        {
            return await _db.OrderHeaders
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .ToListAsync();
        }
        public void CancelOrder(int id)
        {
            var order = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (order != null && order.OrderStatus != Sd.StatusShipped && order.OrderStatus != Sd.StatusCancelled)
            {
                order.OrderStatus = Sd.StatusCancelled;
            }
        }

        public async Task UpdateAsync(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();
        }
    }
}
