using Microsoft.AspNetCore.Mvc;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using System.Threading.Tasks;
using ESports_DataAccess.Repository;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.AddAsync(category);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}
