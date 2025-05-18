using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Models.ViewModels;
using ESports_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Sd.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Product/Index
        public async Task<IActionResult> Index()
        {
            var productsQuery = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
            var products = productsQuery.ToList();
            return View(products);
        }

        // GET: Product/Upsert
        public async Task<IActionResult> Upsert(int? id)
        {
            var categoriesQuery = await _unitOfWork.Category.GetAllAsync();
            ProductVM productVM = new()
            {
                CategoryList = categoriesQuery.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return View(productVM); // Create
            }
            else
            {
                var product = await _unitOfWork.Product.GetAsync(u => u.Id == id);
                productVM.Product = product;
                return View(productVM); // Edit
            }
        }

        // POST: Product/Upsert
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                try
                {
                    if (file != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwRootPath, "images", "product");

                        // Ensure the folder exists
                        if (!Directory.Exists(productPath))
                        {
                            Directory.CreateDirectory(productPath);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        productVM.Product.ImageUrl = @"\images\product\" + fileName;
                    }
                }
                catch (Exception ex)
                {
                    // Add model error and reload categories list for returning the view
                    ModelState.AddModelError("", "Image upload failed: " + ex.Message);
                    var categories = await _unitOfWork.Category.GetAllAsync();
                    productVM.CategoryList = categories.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    return View(productVM);
                }

                if (productVM.Product.Id == 0)
                {
                    await _unitOfWork.Product.AddAsync(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                await _unitOfWork.SaveAsync();
                TempData["success"] = "Product saved successfully";
                return RedirectToAction(nameof(Index));
            }

            var categoriesList = await _unitOfWork.Category.GetAllAsync();
            productVM.CategoryList = categoriesList.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(productVM);
        }


        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productsQuery = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category");
            var products = productsQuery.ToList();
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var productToDelete = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (productToDelete == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            if (!string.IsNullOrEmpty(productToDelete.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            await _unitOfWork.Product.RemoveAsync(productToDelete);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion
    }
}
