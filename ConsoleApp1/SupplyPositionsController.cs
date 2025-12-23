using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;

namespace AutoPartsWarehouse.Controllers
{
    public class SupplyPositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupplyPositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SupplyPositions/Create?supplyId=5
        public IActionResult Create(int supplyId)
        {
            ViewData["SupplyId"] = supplyId;
            // Список запчастей для выбора
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name");
            return View();
        }

        // POST: SupplyPositions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupplyId,PartId,Quantity,PurchasePrice")] SupplyPosition supplyPosition)
        {
            if (ModelState.IsValid)
            {
                // 1. Сохраняем позицию в базе
                _context.Add(supplyPosition);

                // 2. БИЗНЕС-ЛОГИКА: Увеличиваем остаток на складе (ПРИХОД)
                // Ищем складскую запись для этой запчасти
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == supplyPosition.PartId);

                if (stock != null)
                {
                    stock.Quantity += supplyPosition.Quantity; // ПЛЮСУЕМ
                    _context.Update(stock);
                }
                else
                {
                    // Если вдруг записи на складе нет (хотя мы создаем её при создании запчасти), создадим новую
                    var newStock = new Stock
                    {
                        PartId = supplyPosition.PartId,
                        Quantity = supplyPosition.Quantity,
                        MinQuantity = 5,
                        Location = "Приемка"
                    };
                    _context.Add(newStock);
                }

                await _context.SaveChangesAsync();

                // Возвращаемся обратно к деталям поставки
                return RedirectToAction("Details", "Supplies", new { id = supplyPosition.SupplyId });
            }

            ViewData["SupplyId"] = supplyPosition.SupplyId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", supplyPosition.PartId);
            return View(supplyPosition);
        }
    }
}