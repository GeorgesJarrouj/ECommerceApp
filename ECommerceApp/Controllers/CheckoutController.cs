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
			// You can show the checkout form view you already have
			return View();
		}

		// POST: /Checkout/PlaceOrder
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult PlaceOrder(string fullName, string phone, string address)
		{
			// Basic validation
			if (string.IsNullOrWhiteSpace(fullName) ||
				string.IsNullOrWhiteSpace(phone) ||
				string.IsNullOrWhiteSpace(address))
			{
				ModelState.AddModelError(string.Empty, "Please fill all required fields.");
				return View("Index");
			}

			var userId = _userManager.GetUserId(User);
			if (string.IsNullOrWhiteSpace(userId))
				return Challenge(); // forces login if somehow missing

			// Load cart items for user + include product for price
			var cartItems = _db.CartItems
				.Include(ci => ci.Product)
				.Where(ci => ci.UserId == userId)
				.ToList();

			if (!cartItems.Any())
			{
				ModelState.AddModelError(string.Empty, "Your cart is empty.");
				return RedirectToAction("Index", "Cart");
			}

			// Calculate total
			decimal total = 0m;
			foreach (var item in cartItems)
			{
				total += item.Product.Price * item.Quantity;
			}

			// Create order
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
			_db.SaveChanges(); // to get order.Id

			// Create order items
			var orderItems = cartItems.Select(ci => new OrderItem
			{
				OrderId = order.Id,
				ProductId = ci.ProductId,
				Quantity = ci.Quantity,
				Price = ci.Product.Price
			}).ToList();

			_db.OrderItems.AddRange(orderItems);

			// Clear cart
			_db.CartItems.RemoveRange(cartItems);

			_db.SaveChanges();

			return RedirectToAction("ThankYou");
		}

		// GET: /Checkout/ThankYou
		public IActionResult ThankYou()
		{
			return View();
		}
	}
}
