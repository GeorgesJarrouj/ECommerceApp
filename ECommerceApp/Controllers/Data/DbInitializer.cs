using ECommerceApp.Models;
using System.Linq;


namespace ECommerceApp.Data
{
	public static class DbInitializer
	{
		public static void Seed(ApplicationDbContext db)
		{
			if (db.Products.Any()) return;

			db.Products.AddRange(
				new Product { Name = "Nordic Chair ", Price = 50m, ImageUrl = "/images/product-1.png", Stock = 10 },
				new Product { Name = "Kruzo Aero Chair", Price = 78m, ImageUrl = "/images/product-2.png", Stock = 8 },
				new Product { Name = "Ergonomic Chair", Price = 43m, ImageUrl = "/images/product-3.png", Stock = 15 }
			);

			db.SaveChanges();
		}
	}
}
