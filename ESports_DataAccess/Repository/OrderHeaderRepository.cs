using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;

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

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var order = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    order.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var order = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.SessionId = sessionId;
                order.PaymentIntentId = paymentIntentId;
            }
        }
    }
}
