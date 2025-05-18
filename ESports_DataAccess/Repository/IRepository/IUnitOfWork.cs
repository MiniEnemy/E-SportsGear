using ESports_Models;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }

        IProductRepository Product { get; }
        ICategoryRepository Category { get; }
        IProductVisitRepository ProductVisit { get; }
        IApplicationUserRepository ApplicationUser { get; }

        Task SaveAsync();
    }
}
