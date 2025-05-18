using ESports_Models;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);
        void Remove(ShoppingCart entity);

        Task RemoveRangeAsync(IEnumerable<ShoppingCart> shoppingCarts);

        int IncrementCount(ShoppingCart cart, int count);
        int DecrementCount(ShoppingCart cart, int count);
    }
}
