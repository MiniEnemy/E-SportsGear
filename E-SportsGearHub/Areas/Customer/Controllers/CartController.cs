using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Models.ViewModels;
using ESports_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
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
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _stripeSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWork unitOfWork, IOptions<StripeSettings> stripeSettings, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _stripeSettings = stripeSettings.Value;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId, includeProperties: "Product");

            var shoppingCartVM = new ShoppingCartVM
            {
                OrderHeader = new OrderHeader(),
                ShoppingCartList = cartItems.ToList()
            };

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _unitOfWork.Product.GetAsync(p => p.Id == productId);

            if (product == null || product.Stock < count)
            {
                TempData["Error"] = "Product not available or insufficient stock.";
                return RedirectToAction("Details", "Product", new { id = productId, area = "Customer" });
            }

            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(
                c => c.ProductId == productId && c.ApplicationUserId == userId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    ApplicationUserId = userId,
                    ProductId = productId,
                    Count = count
                };
                await _unitOfWork.ShoppingCart.AddAsync(cartItem);
            }
            else
            {
                if (product.Stock >= cartItem.Count + count)
                {
                    cartItem.Count += count;
                    _unitOfWork.ShoppingCart.Update(cartItem);
                }
                else
                {
                    TempData["Error"] = "Cannot add more than available stock.";
                    return RedirectToAction("Details", "Product", new { id = productId, area = "Customer" });
                }
            }

            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Product added to cart successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(c => c.Id == cartId, includeProperties: "Product");

            if (cartItem != null && cartItem.Product.Stock > cartItem.Count)
            {
                cartItem.Count++;
                _unitOfWork.ShoppingCart.Update(cartItem);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Minus(int cartId)
        {
            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(c => c.Id == cartId);

            if (cartItem != null)
            {
                if (cartItem.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Remove(cartItem);
                }
                else
                {
                    cartItem.Count--;
                    _unitOfWork.ShoppingCart.Update(cartItem);
                }

                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(c => c.Id == cartId);

            if (cartItem != null)
            {
                _unitOfWork.ShoppingCart.Remove(cartItem);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId, includeProperties: "Product")).ToList();

            var orderVM = new OrderVM
            {
                OrderHeader = new OrderHeader(),
                ShoppingCartList = cartItems
            };

            return View(orderVM);
        }

        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orderHeaders = await _unitOfWork.OrderHeader
                .GetAllAsync(o => o.ApplicationUserId == userId);

            var orderDetails = await _unitOfWork.OrderDetail
                .GetAllAsync(d => orderHeaders.Select(o => o.Id).Contains(d.OrderHeaderId),
                             includeProperties: "Product");

            var orderVMList = orderHeaders.Select(order => new OrderVM
            {
                OrderHeader = order,
                OrderDetails = orderDetails
                    .Where(d => d.OrderHeaderId == order.Id)
                    .ToList() 
            }).ToList();

            return View(orderVMList);
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _unitOfWork.OrderHeader.GetFirstOrDefaultAsync(o => o.Id == id && o.ApplicationUserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus == Sd.StatusShipped || order.OrderStatus == Sd.StatusCancelled)
            {
                TempData["Error"] = "This order cannot be cancelled.";
                return RedirectToAction(nameof(MyOrders));
            }

            _unitOfWork.OrderHeader.CancelOrder(id);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Order cancelled successfully.";
            return RedirectToAction(nameof(MyOrders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(OrderVM orderVM, string paymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId, includeProperties: "Product")).ToList();

            if (!cartItems.Any())
                return RedirectToAction(nameof(Index));

            double orderTotal = cartItems.Sum(i => i.Product.Price * i.Count);

            var orderHeader = orderVM.OrderHeader;
            orderHeader.OrderDate = DateTime.Now;
            orderHeader.ApplicationUserId = userId;
            orderHeader.OrderStatus = "Pending";

            // COD PaymentStatus Pending; Online PaymentStatus Delayed Payment until Stripe success
            orderHeader.PaymentStatus = string.Equals(paymentMethod, "COD", StringComparison.OrdinalIgnoreCase) ? "Pending" : "Delayed Payment";
            orderHeader.OrderTotal = orderTotal;

            await _unitOfWork.OrderHeader.AddAsync(orderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var item in cartItems)
            {
                await _unitOfWork.OrderDetail.AddAsync(new OrderDetail
                {
                    OrderHeaderId = orderHeader.Id,
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Price = item.Product.Price
                });

                var product = await _unitOfWork.Product.GetAsync(p => p.Id == item.ProductId);
                product.Stock -= item.Count;
                _unitOfWork.Product.Update(product);
            }

            await _unitOfWork.SaveAsync();

            if (string.Equals(paymentMethod, "COD", StringComparison.OrdinalIgnoreCase))
            {
                // Clear cart and redirect directly to confirmation page
                foreach (var item in cartItems)
                    _unitOfWork.ShoppingCart.Remove(item);

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeader.Id });
            }

            // Stripe payment flow below for online payment
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cartItems.Select(i => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(i.Product.Price * 100), // in cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = i.Product.ProductName,
                            Description = i.Product.Description
                        }
                    },
                    Quantity = i.Count
                }).ToList(),
                Mode = "payment",
                SuccessUrl = Url.Action(nameof(PaymentSuccess), "Cart", new { orderId = orderHeader.Id }, Request.Scheme),
                CancelUrl = Url.Action(nameof(PaymentCancel), "Cart", new { orderId = orderHeader.Id }, Request.Scheme)
            };

            var service = new SessionService();
            Session session = service.Create(options);

            orderHeader.SessionId = session.Id;
            orderHeader.PaymentIntentId = session.PaymentIntentId;
            await _unitOfWork.SaveAsync();

            return Redirect(session.Url);
        }

        public async Task<IActionResult> PaymentSuccess(int orderId)
        {
            var order = await _unitOfWork.OrderHeader.GetFirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            order.OrderStatus = Sd.StatusApproved;
            order.PaymentStatus = Sd.PaymentStatusApproved;

            await _unitOfWork.SaveAsync();

            // Clear shopping cart after payment success
            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == order.ApplicationUserId);

            foreach (var item in cartItems)
            {
                _unitOfWork.ShoppingCart.Remove(item);
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(OrderConfirmation), new { id = orderId });
        }

        public async Task<IActionResult> PaymentCancel(int orderId)
        {
            var order = await _unitOfWork.OrderHeader.GetFirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound();

            // Optionally update order status or notify user
            TempData["Error"] = "Payment was cancelled.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = await _unitOfWork.OrderHeader.GetFirstOrDefaultAsync(
                o => o.Id == id, includeProperties: "OrderDetails,OrderDetails.Product");

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
