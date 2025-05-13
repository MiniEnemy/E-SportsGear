using Microsoft.AspNetCore.Mvc;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using System.Threading.Tasks;
using ESports_DataAccess.Repository;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Product.AddAsync(product);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
