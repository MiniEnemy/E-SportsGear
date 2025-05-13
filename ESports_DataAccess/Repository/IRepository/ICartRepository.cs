using ESports_Models;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        void Update(Cart cart);
    }
}
