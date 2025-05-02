using ESports_DataAccess.Data;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using Microsoft.AspNetCore.Mvc;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        public ProductController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        // GET: Product/Index
        public IActionResult Index()
        {
            List<Product> objProductList = _UnitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public IActionResult Create(Product obj)
        {         
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Add(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Product/Edit/{id}
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product productFromDb = _UnitOfWork.Product.Get(u=>u.Id==id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        // POST: Product/Edit
        [HttpPost]
        public IActionResult Edit(Product obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Product.Update(obj);
                _UnitOfWork.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Product/Delete/{id}
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Product? productFromDb = _UnitOfWork.Product.Get(u => u.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        // POST: Product/Delete
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Product obj = _UnitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _UnitOfWork.Product.Remove(obj);
            _UnitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
