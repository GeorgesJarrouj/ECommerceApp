using ECommerceApp.Data;
using ECommerceApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Now.Date;

            // Orders today
            var ordersTodayQuery = _db.Orders.Where(o => o.OrderDate.Date == today);

            var ordersToday = await ordersTodayQuery.CountAsync();

            var salesToday = await ordersTodayQuery
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var avgOrderToday = ordersToday == 0 ? 0m : salesToday / ordersToday;

            var dailySales = await _db.Orders
     .GroupBy(o => o.OrderDate.Date)
     .Select(g => new DailySalesRow
     {
         Day = g.Key,
         OrdersCount = g.Count(),
         TotalSales = g.Sum(x => x.TotalAmount)
     })
     .OrderByDescending(x => x.Day)
     .ToListAsync();


            // Build VM
            var vm = new AdminDashboardVM
            {
                TotalUsers = await _db.Users.CountAsync(),
                TotalProducts = await _db.Products.CountAsync(),

                // Requires Product.CreatedAt (if you don't have it, set this to 0)
                ProductsAddedToday = await _db.Products.CountAsync(p => p.CreatedAt >= today),

                OrdersToday = ordersToday,
                SalesToday = salesToday,
                AverageOrderToday = avgOrderToday,

                DailySales = dailySales
            };

            return View(vm);
        }
    }
}
