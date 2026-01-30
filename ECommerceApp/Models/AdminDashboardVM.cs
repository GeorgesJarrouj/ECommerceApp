namespace ECommerceApp.Models.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int ProductsAddedToday { get; set; }

        public int OrdersToday { get; set; }
        public decimal SalesToday { get; set; }
        public decimal AverageOrderToday { get; set; }

        // ✅ NEW: sales per day
        public List<DailySalesRow> DailySales { get; set; } = new();
    }

    public class DailySalesRow
    {
        public DateTime Day { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSales { get; set; }
    }
}
