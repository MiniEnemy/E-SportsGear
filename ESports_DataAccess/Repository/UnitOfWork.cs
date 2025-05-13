using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using System.Threading.Tasks;

namespace ESports_DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductVisitRepository ProductVisit { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            Product = new ProductRepository(_context);
            ProductVisit = new ProductVisitRepository(_context);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
