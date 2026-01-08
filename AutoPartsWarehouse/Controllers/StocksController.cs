//запасы
using AutoPartsWarehouse.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StocksController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var stocks = _context.Stocks.Include(s => s.Part);
            return View(await stocks.ToListAsync());
        }

        // Редактирование (только локация и мин. остаток, кол-во меняют продажи/поставки)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var stock = await _context.Stocks.Include(s => s.Part).FirstOrDefaultAsync(s => s.Id == id);
            if (stock == null) return NotFound();
            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PartId,Quantity,Location,MinQuantity")] Models.Stock stock)
        {
            if (id != stock.Id) return NotFound();

            // Мы обновляем только Location и MinQuantity, Quantity не трогаем вручную здесь
            var existingStock = await _context.Stocks.FindAsync(id);
            if (existingStock != null)
            {
                existingStock.Location = stock.Location;
                existingStock.MinQuantity = stock.MinQuantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}