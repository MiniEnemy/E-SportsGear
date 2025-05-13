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

        // Display all products with global recommendations
        public async Task<IActionResult> Index()
        {
            var allProducts = await _unitOfWork.Product.GetAllAsync();

            List<Product> topVisitedProducts = new List<Product>();
            List<Product> randomProducts = new List<Product>();

            // Global top 5 most visited product IDs (aggregated across all users)
            var allVisits = await _unitOfWork.ProductVisit.GetAllAsync();
            if (allVisits != null && allVisits.Any())
            {
                var globalTopIds = allVisits
                    .GroupBy(v => v.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalVisits = g.Sum(v => v.VisitCount)
                    })
                    .OrderByDescending(g => g.TotalVisits)
                    .Take(5)
                    .Select(g => g.ProductId)
                    .ToList();

                var visitedProducts = (await _unitOfWork.Product
                    .GetAllAsync(p => globalTopIds.Contains(p.Id)))
                    .AsEnumerable()
                    .OrderBy(p => globalTopIds.IndexOf(p.Id)) // run this in memory
                    .ToList();

                topVisitedProducts = visitedProducts;
            }

            // Always get random products
            randomProducts = allProducts
                .OrderBy(p => Guid.NewGuid())
                .Take(6)
                .ToList();

            ViewBag.TopVisitedProducts = topVisitedProducts;
            ViewBag.RandomProducts = randomProducts;

            return View(allProducts);
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        public async Task<IActionResult> AboutUs()
        {
            return View();
        }

        public async Task<IActionResult> Home()
        {
            return View();
        }

        // Product details + visit tracking
        public async Task<IActionResult> Details(int productId)
        {
            var product = await _unitOfWork.Product.GetAsync(u => u.Id == productId, includeProperties: "Category");
            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                var visit = await _unitOfWork.ProductVisit.GetVisitAsync(productId, userId);

                if (visit == null)
                {
                    visit = new ProductVisit
                    {
                        ProductId = productId,
                        ApplicationUserId = userId,
                        IpAddress = ipAddress,
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
    }
}
