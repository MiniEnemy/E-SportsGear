using ESports_DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_SportsGearHub.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // List all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(includeProperties: "ApplicationUser");
            return View("~/Areas/Admin/Views/Orders/Index.cshtml", orders);
        }

        // View order details
        public async Task<IActionResult> Details(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetFirstOrDefaultAsync(
                u => u.Id == id,
                includeProperties: "ApplicationUser");

            var orderDetails = await _unitOfWork.OrderDetail.GetAllAsync(
                u => u.OrderHeaderId == id,
                includeProperties: "Product");

            ViewBag.OrderDetails = orderDetails;

            return View("~/Areas/Admin/Views/Orders/Details.cshtml", orderHeader);
        }
    }
}
