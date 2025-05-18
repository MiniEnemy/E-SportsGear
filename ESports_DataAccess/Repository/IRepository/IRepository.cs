using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, string includeProperties = "");
        Task AddAsync(T entity);

        void Remove(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);

        Task RemoveAsync(T entity);
        void Update(T entity);
    }
}
