using ESports_Models;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);
        void CancelOrder(int id);

        Task UpdateAsync(OrderHeader orderHeader);
    }
}
