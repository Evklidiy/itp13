using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Controllers
{
    public class SalePositionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SalePositionsController(ApplicationDbContext context) => _context = context;

        // GET: SalePositions/Create?saleId=5
        public IActionResult Create(int saleId)
        {
            ViewData["SaleId"] = saleId;
            // Выпадающий список запчастей
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SaleId,PartId,Quantity,Price")] SalePosition salePosition)
        {
            if (ModelState.IsValid)
            {
                // 1. Добавляем позицию
                _context.Add(salePosition);

                // 2. Списываем со склада (Бизнес-логика)
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == salePosition.PartId);
                if (stock != null)
                {
                    stock.Quantity -= salePosition.Quantity; // Уменьшаем остаток
                    _context.Update(stock);
                }

                await _context.SaveChangesAsync();

                // Возвращаемся в детали продажи
                return RedirectToAction("Details", "Sales", new { id = salePosition.SaleId });
            }
            ViewData["SaleId"] = salePosition.SaleId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", salePosition.PartId);
            return View(salePosition);
        }
    }
}