using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Models
{
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string UserId { get; set; } = string.Empty;

		[Required]
		public DateTime OrderDate { get; set; } = DateTime.Now;

		[Required]
		[Precision(18, 2)]
		public decimal TotalAmount { get; set; }

		// Shipping info
		public string FullName { get; set; } = "";
		public string Phone { get; set; } = "";
		public string Address { get; set; } = "";

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
