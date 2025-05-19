using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Models.ViewModels;
using ESports_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orders = await _unitOfWork.OrderHeader
                                .GetAllAsync(o => o.ApplicationUserId == userId, includeProperties: "ApplicationUser");

            return View(orders.OrderByDescending(o => o.OrderDate));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _unitOfWork.OrderHeader.GetAsync(o => o.Id == id);
            if (order == null)
                return NotFound();

            // Only allow cancel if order status is Pending
            if (order.OrderStatus == Sd.StatusPending)
            {
                order.OrderStatus = Sd.StatusCancelled;
                _unitOfWork.OrderHeader.Update(order);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                TempData["Error"] = "Cannot cancel this order.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == id,
                includeProperties: "ApplicationUser");

            if (orderHeader == null)
                return NotFound();

            var orderDetails = (await _unitOfWork.OrderDetail.GetAllAsync(
                c => c.OrderHeaderId == id,
                includeProperties: "Product")).ToList();

            OrderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(OrderVM);
        }


        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = (await _unitOfWork.ShoppingCart.GetAllAsync(
                c => c.ApplicationUserId == userId,
                includeProperties: "Product")).ToList();

            if (cartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty. Please add some products first.";
                return RedirectToAction("Index", "Cart", new { area = "Customer" });
            }

            var orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                OrderTotal = cartItems.Sum(c => c.Count * c.Product.Price)
            };

            OrderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                CartList = cartItems
            };

            return View(OrderVM);
        }


        
        public async Task<IActionResult> Checkout(OrderVM orderVM, string paymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product")).ToList();

            if (cartItems.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty.");
                orderVM.CartList = cartItems;
                return View(orderVM);
            }

            // Stock validation
            foreach (var item in cartItems)
            {
                if (item.Count > item.Product.Stock)
                {
                    ModelState.AddModelError("", $"Insufficient stock for {item.Product.ProductName}. Only {item.Product.Stock} left.");
                    orderVM.CartList = cartItems;
                    return View(orderVM);
                }
            }

            // Fill order header info
            orderVM.OrderHeader.ApplicationUserId = userId;
            orderVM.OrderHeader.OrderDate = DateTime.Now;
            orderVM.OrderHeader.OrderTotal = cartItems.Sum(c => c.Count * c.Product.Price);

            if (paymentMethod == "COD")
            {
                orderVM.OrderHeader.PaymentStatus = Sd.PaymentStatusCOD;
                orderVM.OrderHeader.OrderStatus = Sd.StatusPending;
            }
            else if (paymentMethod == "Stripe")
            {
                orderVM.OrderHeader.PaymentStatus = Sd.PaymentStatusPending;
                orderVM.OrderHeader.OrderStatus = Sd.StatusPending;
            }
            else
            {
                ModelState.AddModelError("", "Invalid payment method.");
                orderVM.CartList = cartItems;
                return View(orderVM);
            }

            // Save OrderHeader
            await _unitOfWork.OrderHeader.AddAsync(orderVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var cart in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = orderVM.OrderHeader.Id,
                    Price = cart.Product.Price,
                    Count = cart.Count
                };
                await _unitOfWork.OrderDetail.AddAsync(orderDetail);

                // Deduct stock
                cart.Product.Stock -= cart.Count;
                if (cart.Product.Stock < 0) cart.Product.Stock = 0;
                _unitOfWork.Product.Update(cart.Product);
            }
            await _unitOfWork.SaveAsync();

            if (paymentMethod == "COD")
            {
                await _unitOfWork.ShoppingCart.RemoveRangeAsync(cartItems);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(OrderConfirmation), new { id = orderVM.OrderHeader.Id });
            }
            else if (paymentMethod == "Stripe")
            {
                var domain = $"{Request.Scheme}://{Request.Host}/";
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
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((double)item.Product.Price * 100), // Explicit cast
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.ProductName
                            },
                        },
                        Quantity = item.Count,
                    });
                }

                var service = new SessionService();
                Session session = service.Create(options);

                orderVM.OrderHeader.SessionId = session.Id;
                orderVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
                await _unitOfWork.SaveAsync();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return View(orderVM);
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader.GetAsync(o => o.Id == id);

            if (orderHeader == null)
                return NotFound();

            if (orderHeader.PaymentStatus != Sd.PaymentStatusCOD)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    orderHeader.PaymentStatus = Sd.PaymentStatusApproved;
                    orderHeader.OrderStatus = Sd.StatusApproved;
                    await _unitOfWork.SaveAsync();

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(c => c.ApplicationUserId == userId);
                    await _unitOfWork.ShoppingCart.RemoveRangeAsync(cartItems);
                    await _unitOfWork.SaveAsync();
                }
            }

            return View(id);
        }
    }
}
