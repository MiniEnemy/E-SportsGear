using System;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Product { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }

        IProductVisitRepository ProductVisit { get; }
        ICategoryRepository Category { get; }

        Task SaveAsync();
    }
}
