using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CheckoutController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET: /Checkout
        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var cartItems = _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            return View(cartItems);
        }

        // POST: /Checkout/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string fullName, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError(string.Empty, "Please fill all required fields.");
                return Index(); // ✅ reload with cart model
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                return Challenge();

            var cartItems = _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            if (!cartItems.Any())
            {
                ModelState.AddModelError(string.Empty, "Your cart is empty.");
                return RedirectToAction("Index", "Cart");
            }

            using var tx = _db.Database.BeginTransaction();

            try
            {
                // ✅ Validate stock + decrease stock
                foreach (var item in cartItems)
                {
                    if (item.Product.Stock <= 0)
                    {
                        ModelState.AddModelError(string.Empty,
                            $"{item.Product.Name} is out of stock.");

                        tx.Rollback();
                        return View("Index", cartItems);
                    }

                    if (item.Product.Stock < item.Quantity)
                    {
                        ModelState.AddModelError(string.Empty,
                            $"Not enough stock for {item.Product.Name}. Available: {item.Product.Stock}");

                        tx.Rollback();
                        return View("Index", cartItems);
                    }

                    item.Product.Stock -= item.Quantity;
                }

                decimal total = 0m;
                foreach (var item in cartItems)
                {
                    total += item.Product.Price * item.Quantity;
                }

                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = total,
                    FullName = fullName.Trim(),
                    Phone = phone.Trim(),
                    Address = address.Trim()
                };

                _db.Orders.Add(order);
                _db.SaveChanges();

                var orderItems = cartItems.Select(ci => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList();

                _db.OrderItems.AddRange(orderItems);

                _db.CartItems.RemoveRange(cartItems);

                _db.SaveChanges();
                tx.Commit();

                return RedirectToAction("ThankYou");
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
