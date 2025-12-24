using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsWarehouse.Controllers
{
    public class SupplyPositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create(int supplyId)
        {
            ViewData["SupplyId"] = supplyId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplyId,PartId,Quantity,PurchasePrice")] SupplyPosition supplyPosition)
        {
            if (ModelState.IsValid)
            {
                // 1. Сохраняем позицию
                _context.Add(supplyPosition);

                // 2. Проверяем статус самой поставки
                var supply = await _context.Supplies.FindAsync(supplyPosition.SupplyId);

                // ВАЖНО: Начисляем на склад ТОЛЬКО если статус уже "Получена"
                if (supply != null && supply.Status == "Получена")
                {
                    var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == supplyPosition.PartId);
                    if (stock != null)
                    {
                        stock.Quantity += supplyPosition.Quantity;
                        _context.Update(stock);
                    }
                    // Если стока нет — создаем (код опущен для краткости, так как у нас стоки создаются с запчастями)
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Supplies", new { id = supplyPosition.SupplyId });
            }

            ViewData["SupplyId"] = supplyPosition.SupplyId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", supplyPosition.PartId);
            return View(supplyPosition);
        }
    }
}