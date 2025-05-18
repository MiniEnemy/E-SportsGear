using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Models.ViewModels;
using ESports_Utility;
using Stripe;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace E_SportsGearHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Order List for Logged-in User
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _unitOfWork.OrderHeader.GetAllAsync(
                o => o.ApplicationUserId == userId,
                includeProperties: "ApplicationUser");

            return View(orders);
        }

        // GET: Order Details
        // GET: Order Details
        public async Task<IActionResult> Details(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == id,
                includeProperties: "ApplicationUser");

            var cartItems = (await _unitOfWork.ShoppingCart.GetAllAsync(
                c => c.ApplicationUserId == orderHeader.ApplicationUserId,
                includeProperties: "Product")).ToList();

            OrderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                CartList = cartItems
            };

            return View(OrderVM);
        }


        // GET: Checkout page
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            var orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                OrderTotal = cartItems.Sum(c => c.Count * (decimal)c.Product.Price)
            };

            OrderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                CartList = cartItems.ToList()
            };

            return View(OrderVM);
        }

        // POST: Submit Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderVM orderVM)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            orderVM.OrderHeader.ApplicationUserId = userId;
            orderVM.OrderHeader.OrderDate = DateTime.Now;
            orderVM.OrderHeader.OrderTotal = cartItems.Sum(c => c.Count * (decimal)c.Product.Price);
            orderVM.OrderHeader.PaymentStatus ??= Sd.PaymentStatusPending;
            orderVM.OrderHeader.OrderStatus = Sd.StatusPending;

            await _unitOfWork.OrderHeader.AddAsync(orderVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var cart in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = orderVM.OrderHeader.Id,
                    Price = (decimal)cart.Product.Price,
                    Count = cart.Count
                };
                await _unitOfWork.OrderDetail.AddAsync(orderDetail);
            }

            await _unitOfWork.SaveAsync();

            // ✅ Cash on Delivery
            if (orderVM.OrderHeader.PaymentStatus == Sd.PaymentStatusCOD)
            {
                await _unitOfWork.ShoppingCart.RemoveRangeAsync(cartItems);
                await _unitOfWork.SaveAsync();

                return RedirectToAction(nameof(OrderConfirmation), new { id = orderVM.OrderHeader.Id });
            }

            // ✅ Stripe Payment Integration
            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/order/OrderConfirmation?id={orderVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
            };

            foreach (var item in cartItems)
            {
                var lineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)((decimal)item.Product.Price * 100), // cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(lineItem);
            }

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            orderVM.OrderHeader.SessionId = session.Id;
            orderVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            await _unitOfWork.SaveAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        // ✅ GET: Order Confirmation page
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == id,
                includeProperties: "ApplicationUser");

            if (orderHeader == null)
                return NotFound();

            if (orderHeader.PaymentStatus != Sd.PaymentStatusCOD)
            {
                var service = new SessionService();
                var session = await service.GetAsync(orderHeader.SessionId);

                if (session.PaymentStatus?.ToLower() == "paid")
                {
                    orderHeader.PaymentStatus = Sd.PaymentStatusApproved;
                    orderHeader.OrderStatus = Sd.StatusApproved;

                    var userCart = await _unitOfWork.ShoppingCart.GetAllAsync(
                        u => u.ApplicationUserId == orderHeader.ApplicationUserId);

                    await _unitOfWork.ShoppingCart.RemoveRangeAsync(userCart);
                    await _unitOfWork.SaveAsync();
                }
            }

            return View(orderHeader);
        }
    }
}
