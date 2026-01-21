using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Models
{
	public class OrderItem
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int OrderId { get; set; }

		[Required]
		public int ProductId { get; set; }

		public int Quantity { get; set; }
		[Precision(18, 2)]
		public decimal Price { get; set; }

		[ForeignKey(nameof(ProductId))]
		public Product Product { get; set; } = null!;
	}
}
