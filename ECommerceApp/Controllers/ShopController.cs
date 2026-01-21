using Microsoft.AspNetCore.Mvc;
using ECommerceApp.Data;

namespace ECommerceApp.Controllers
{
	public class ShopController : Controller
	{
		private readonly ApplicationDbContext _db;
		public ShopController(ApplicationDbContext db) => _db = db;

		public IActionResult Index()
		{
			var products = _db.Products.ToList();
			return View(products);
		}
	}
}
