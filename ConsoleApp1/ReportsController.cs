using AutoPartsWarehouse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ReportsController(ApplicationDbContext context) => _context = context;

        public IActionResult Index() => View();

        public async Task<IActionResult> LowStockReport()
        {
            var lowStocks = await _context.Stocks
                .Include(s => s.Part)
                .Where(s => s.Quantity < s.MinQuantity)
                .Select(s => new LowStockViewModel
                {
                    // Используем null-coalescing operator (??), чтобы избежать null
                    Article = s.Part != null ? s.Part.Article : "Нет данных",
                    Name = s.Part != null ? s.Part.Name : "Нет данных",
                    Current = s.Quantity,
                    Min = s.MinQuantity,
                    ToOrder = (s.MinQuantity - s.Quantity) + 10
                })
                .ToListAsync();
            return View(lowStocks);
        }

        public async Task<IActionResult> SalesReport(DateTime? start, DateTime? end)
        {
            var s = start ?? DateTime.Now.AddMonths(-1);
            var e = end ?? DateTime.Now;
            ViewBag.Start = s;
            ViewBag.End = e;

            var stats = await _context.SalePositions
                .Include(sp => sp.Sale)
                .Include(sp => sp.Part)
                .Where(sp => sp.Sale != null && sp.Sale.Date >= s && sp.Sale.Date <= e)
                .GroupBy(sp => sp.Part != null ? sp.Part.Name : "Неизвестно")
                .Select(g => new SalesStatsViewModel
                {
                    PartName = g.Key,
                    TotalQty = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.Price)
                })
                .ToListAsync();

            return View(stats);
        }
    }

    // ИСПРАВЛЕННЫЕ ViewModels (инициализация строк)
    public class LowStockViewModel
    {
        public string Article { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Current { get; set; }
        public int Min { get; set; }
        public int ToOrder { get; set; }
    }
    public class SalesStatsViewModel
    {
        public string PartName { get; set; } = string.Empty;
        public int TotalQty { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}