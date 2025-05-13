using ESports_Models;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IProductVisitRepository ProductVisit { get; }

        Task SaveAsync();
    }
}
