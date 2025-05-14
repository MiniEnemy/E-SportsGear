using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Sd.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Admin/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();
            var categoryList = await categories.ToListAsync(); // Fix for IQueryable Task
            return View(categoryList);
        }

        // GET: Admin/Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The display order shouldn't match the name");
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.AddAsync(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        // GET: Admin/Category/Edit/3   
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = await _unitOfWork.Category.GetAsync(u => u.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Admin/Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "The display order shouldn't match the name");

            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(obj);
        }

        // GET: Admin/Category/Delete/3
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var category = await _unitOfWork.Category.GetAsync(u => u.Id == id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Admin/Category/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int? id)
        {
            var category = await _unitOfWork.Category.GetAsync(u => u.Id == id);
            if (category == null)
                return NotFound();

            await _unitOfWork.Category.RemoveAsync(category);
            await _unitOfWork.SaveAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
