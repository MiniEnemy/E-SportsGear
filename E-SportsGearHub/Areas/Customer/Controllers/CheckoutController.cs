using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;
using System.Linq;

namespace E_SportsGearHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _stripeSettings;

        public CheckoutController(IUnitOfWork unitOfWork, IOptions<StripeSettings> stripeOptions)
        {
            _unitOfWork = unitOfWork;
            _stripeSettings = stripeOptions.Value;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user ID
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Get user's shopping cart items
            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(sc => sc.ApplicationUserId == userId, includeProperties: "Product");

            if (cartItems == null || cartItems.Count() == 0)
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            // Create Stripe Checkout Session
            var domain = $"{Request.Scheme}://{Request.Host}";

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + Url.Action("Success"),
                CancelUrl = domain + Url.Action("Cancel"),
            };

            foreach (var item in cartItems)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100), // convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.ProductName,
                        },
                    },
                    Quantity = item.Count,
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);

            // Save the session id in TempData or user session to verify on success page if needed
            TempData["SessionId"] = session.Id;

            // Redirect to Stripe Checkout
            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartItems = await _unitOfWork.ShoppingCart.GetAllAsync(sc => sc.ApplicationUserId == userId, includeProperties: "Product");

            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            // Create OrderHeader
            OrderHeader orderHeader = new OrderHeader
            {
                ApplicationUserId = userId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                PaymentStatus = "Paid",
                OrderTotal = cartItems.Sum(item => item.Product.Price * item.Count),
            };

            await _unitOfWork.OrderHeader.AddAsync(orderHeader);
            await _unitOfWork.SaveAsync();

            // Add OrderDetails
            foreach (var item in cartItems)
            {
                OrderDetail detail = new OrderDetail
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = orderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                };

                await _unitOfWork.OrderDetail.AddAsync(detail);
            }

            // Clear cart
            _unitOfWork.ShoppingCart.RemoveRangeAsync(cartItems);
            await _unitOfWork.SaveAsync();

            return View();
        }


        public IActionResult Cancel()
        {
            return View();
        }
    }
}
