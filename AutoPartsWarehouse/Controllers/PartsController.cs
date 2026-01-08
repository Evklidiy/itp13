//запчасти
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoPartsWarehouse.Models;

namespace AutoPartsWarehouse.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parts (С поиском и пагинацией)
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var parts = _context.Parts.Include(p => p.Stock).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                parts = parts.Where(p => p.Name.Contains(searchString)
                                      || p.Article.Contains(searchString)
                                      || p.CompatibleModels.Contains(searchString));
            }

            // Сортировка по названию по умолчанию
            parts = parts.OrderBy(p => p.Name);

            int pageSize = 10;
            return View(await PaginatedList<Part>.CreateAsync(parts.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Parts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts
                .Include(p => p.Stock)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (part == null) return NotFound();

            return View(part);
        }

        // GET: Parts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Article,Name,Manufacturer,CompatibleModels,PurchasePrice,SalePrice")] Part part)
        {
            if (ModelState.IsValid)
            {
                _context.Add(part);
                await _context.SaveChangesAsync();

                // БИЗНЕС-ЛОГИКА: При создании запчасти сразу создаем для неё место на складе (0 шт)
                var stock = new Stock
                {
                    PartId = part.Id,
                    Quantity = 0,
                    MinQuantity = 5,
                    Location = "Не назначено"
                };
                _context.Add(stock);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(part);
        }

        // GET: Parts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();
            return View(part);
        }

        // POST: Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Article,Name,Manufacturer,CompatibleModels,PurchasePrice,SalePrice")] Part part)
        {
            if (id != part.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(part);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartExists(part.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(part);
        }

        // GET: Parts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (part == null) return NotFound();

            return View(part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartExists(int id)
        {
            return _context.Parts.Any(e => e.Id == id);
        }
    }
}