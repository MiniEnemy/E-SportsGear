using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;

namespace ESports_DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.Update(category);
        }
    }
}
