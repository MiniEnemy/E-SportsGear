using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;

namespace ESports_DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository Product { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductVisitRepository ProductVisit { get; private set; } 


        // If you still want ApplicationUser repository
        public IApplicationUserRepository ApplicationUser { get; private set; } // Optional if needed

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Product = new ProductRepository(_context);
            Category = new CategoryRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            ProductVisit = new ProductVisitRepository(_context);


        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
