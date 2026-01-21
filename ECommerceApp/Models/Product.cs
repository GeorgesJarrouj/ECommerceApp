using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Models
{
	public class Product
	{
		[Key]
		public int Id { get; set; }

		[Required, MaxLength(150)]
		public string Name { get; set; } = "";

		[Precision(18, 2)]
		public decimal Price { get; set; }

		public string? Description { get; set; }

		// Example: "/images/product-1.png"
		public string? ImageUrl { get; set; }

		public int Stock { get; set; } = 0;
		public ICollection<CartItem>? CartItems { get; set; }
	}
}
