//поставки
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Controllers
{
    public class SuppliesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Supplies.Include(s => s.Supplier);
            return View(await appDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var supply = await _context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Positions)
                .ThenInclude(p => p.Part)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (supply == null) return NotFound();

            return View(supply);
        }

        public IActionResult Create()
        {
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplierId,Date,Status")] Supply supply)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supply);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", supply.SupplierId);
            return View(supply);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var supply = await _context.Supplies.FindAsync(id);
            if (supply == null) return NotFound();
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", supply.SupplierId);
            return View(supply);
        }

        // САМОЕ ВАЖНОЕ: Обработка смены статуса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupplierId,Date,Status")] Supply supply)
        {
            if (id != supply.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Получаем СТАРУЮ версию из базы (без отслеживания, чтобы не конфликтовать)
                    var oldSupply = await _context.Supplies.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

                    // Если статус изменился с "Ожидается" (или любого другого) на "Получена"
                    if (oldSupply.Status != "Получена" && supply.Status == "Получена")
                    {
                        // Загружаем позиции этой поставки
                        var positions = await _context.SupplyPositions.Where(p => p.SupplyId == id).ToListAsync();

                        // Проходим по всем позициям и начисляем на склад
                        foreach (var pos in positions)
                        {
                            var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == pos.PartId);
                            if (stock != null)
                            {
                                stock.Quantity += pos.Quantity;
                                _context.Update(stock);
                            }
                        }
                    }

                    _context.Update(supply);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyExists(supply.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", supply.SupplierId);
            return View(supply);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var supply = await _context.Supplies.Include(s => s.Supplier).FirstOrDefaultAsync(m => m.Id == id);
            if (supply == null) return NotFound();
            return View(supply);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supply = await _context.Supplies.FindAsync(id);
            if (supply != null) _context.Supplies.Remove(supply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyExists(int id) => _context.Supplies.Any(e => e.Id == id);
    }
}