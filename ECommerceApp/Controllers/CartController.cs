using ECommerceApp.Data;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var items = _db.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToList();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int productId, int qty = 1)
        {
            if (qty < 1) qty = 1;

            var userId = _userManager.GetUserId(User);

            var existing = _db.CartItems
                .FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);

            if (existing != null)
                existing.Quantity += qty;
            else
                _db.CartItems.Add(new CartItem { UserId = userId!, ProductId = productId, Quantity = qty });

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int id)
        {
            var userId = _userManager.GetUserId(User);

            var item = _db.CartItems.FirstOrDefault(ci => ci.Id == id && ci.UserId == userId);
            if (item != null)
            {
                _db.CartItems.Remove(item);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, int qty)
        {
            var userId = _userManager.GetUserId(User);

            var item = _db.CartItems.FirstOrDefault(ci => ci.Id == id && ci.UserId == userId);
            if (item == null) return RedirectToAction("Index");

            if (qty <= 0)
                _db.CartItems.Remove(item);
            else
                item.Quantity = qty;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
