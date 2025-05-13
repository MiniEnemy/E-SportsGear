using ESports_Models;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IProductVisitRepository : IRepository<ProductVisit>
    {
        Task<ProductVisit> GetVisitAsync(int productId, string applicationUserId);
    }
}
