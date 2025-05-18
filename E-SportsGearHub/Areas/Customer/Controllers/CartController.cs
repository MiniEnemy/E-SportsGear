using ESports_DataAccess.Repository.IRepository;
using ESports_Models;
using ESports_Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace E_SportsGearHub.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Customer/Cart/Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartList = await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product");

            var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            var orderVM = new OrderVM
            {
                ListCart = cartList.ToList(),
                OrderHeader = new OrderHeader
                {
                    Name = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    StreetAddress = user.StreetAddress,
                    City = user.City,
                    State = user.State,
                    PostalCode = user.PostalCode
                }
            };

            orderVM.OrderHeader.OrderTotal = cartList.Sum(c => (decimal)(c.Product.Price * c.Count));

            return View(orderVM);
        }

        // POST: Customer/Cart/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(OrderVM orderVM, string paymentMethod)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            orderVM.ListCart = (await _unitOfWork.ShoppingCart.GetAllAsync(
                u => u.ApplicationUserId == userId,
                includeProperties: "Product")).ToList();

            orderVM.OrderHeader.OrderDate = DateTime.Now;
            orderVM.OrderHeader.ApplicationUserId = userId;
            orderVM.OrderHeader.OrderTotal = orderVM.ListCart.Sum(c => (decimal)(c.Product.Price * c.Count));

            await _unitOfWork.OrderHeader.AddAsync(orderVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var cart in orderVM.ListCart)
            {
                await _unitOfWork.OrderDetail.AddAsync(new OrderDetail
                {
                    ProductId = cart.ProductId,
                    OrderId = orderVM.OrderHeader.Id,
                    Price = (decimal)cart.Product.Price,
                    Count = cart.Count
                });

                // Deduct stock
                cart.Product.Stock -= cart.Count;
                _unitOfWork.Product.Update(cart.Product);
            }

            await _unitOfWork.SaveAsync();

            if (paymentMethod == "Stripe")
            {
                return RedirectToAction("StripePayment", new { orderId = orderVM.OrderHeader.Id });
            }

            if (paymentMethod == "COD")
            {
                orderVM.OrderHeader.PaymentStatus = "Pending";
                orderVM.OrderHeader.OrderStatus = "Pending";
                await _unitOfWork.SaveAsync();

                await _unitOfWork.ShoppingCart.RemoveRangeAsync(orderVM.ListCart);
                await _unitOfWork.SaveAsync();

                return RedirectToAction("OrderConfirmation", new { orderId = orderVM.OrderHeader.Id });
            }

            return View("Checkout", orderVM);
        }

        // GET: Customer/Cart/OrderConfirmation/{orderId}
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _unitOfWork.OrderHeader.GetAsync(
                o => o.Id == orderId,
                includeProperties: "ApplicationUser");

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Customer/Cart/StripePayment/{orderId}
        public IActionResult StripePayment(int orderId)
        {
            return RedirectToAction("OrderConfirmation", new { orderId });
        }

        // POST: Customer/Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int count = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var cartItem = await _unitOfWork.ShoppingCart.GetAsync(
                c => c.ApplicationUserId == userId && c.ProductId == productId);

            if (cartItem == null)
            {
                // Add new cart item
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
                // Update count
                cartItem.Count += count;
                _unitOfWork.ShoppingCart.Update(cartItem);
            }

            await _unitOfWork.SaveAsync();

            return RedirectToAction("Index", "Home");
        }

        // POST: Customer/Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var cart = await _unitOfWork.ShoppingCart.GetAsync(c => c.Id == cartId);

            if (cart == null)
                return NotFound();

            _unitOfWork.ShoppingCart.Remove(cart);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Checkout));
        }
    }
}
