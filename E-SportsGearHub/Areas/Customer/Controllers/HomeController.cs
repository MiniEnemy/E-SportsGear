using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace E_SportsGearHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var allProducts = await _unitOfWork.Product.GetAllAsync();

            List<Product> topVisitedProducts = new List<Product>();
            List<Product> randomProducts = new List<Product>();

            // Get global most visited product IDs
            var allVisits = await _unitOfWork.ProductVisit.GetAllAsync();

            var globalTopIds = allVisits
                .GroupBy(v => v.ProductId)
                .Select(g => new { ProductId = g.Key, TotalVisits = g.Sum(v => v.VisitCount) })
                .OrderByDescending(g => g.TotalVisits)
                .Take(5)
                .Select(g => g.ProductId)
                .ToList();

            // Fetch and sort top visited products manually
            var globalTopProducts = (await _unitOfWork.Product.GetAllAsync(p => globalTopIds.Contains(p.Id)))
                .ToList()
                .OrderBy(p => globalTopIds.IndexOf(p.Id))
                .ToList();

            topVisitedProducts = globalTopProducts;

            // Random products for discovery
            randomProducts = allProducts
                .OrderBy(p => Guid.NewGuid())
                .Take(6)
                .ToList();

            ViewBag.TopVisitedProducts = topVisitedProducts;
            ViewBag.RandomProducts = randomProducts;

            return View(allProducts);
        }

        public async Task<IActionResult> Details(int productId)
        {
            var product = await _unitOfWork.Product.GetAsync(u => u.Id == productId, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userId))
            {
                var visit = await _unitOfWork.ProductVisit.GetVisitAsync(productId, userId);

                if (visit == null)
                {
                    visit = new ProductVisit
                    {
                        ProductId = productId,
                        ApplicationUserId = userId,
                        VisitCount = 1,
                        VisitDate = DateTime.Now,
                        LastVisited = DateTime.Now
                    };
                    await _unitOfWork.ProductVisit.AddAsync(visit);
                }
                else
                {
                    visit.VisitCount++;
                    visit.LastVisited = DateTime.Now;
                    await _unitOfWork.ProductVisit.UpdateAsync(visit);
                }

                await _unitOfWork.SaveAsync();
            }

            return View(product);
        }

        public IActionResult Privacy() => View();
        public IActionResult AboutUs() => View();
        public IActionResult Home() => View();
    }
}
