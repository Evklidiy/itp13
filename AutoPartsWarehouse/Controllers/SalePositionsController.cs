using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;

namespace AutoPartsWarehouse.Controllers
{
    public class SalePositionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalePositionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // CREATE: Добавление позиции + Списание со склада
        // =========================================================

        // GET: SalePositions/Create?saleId=5
        public IActionResult Create(int saleId)
        {
            ViewData["SaleId"] = saleId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SaleId,PartId,Quantity,Price")] SalePosition salePosition)
        {
            if (ModelState.IsValid)
            {
                // 1. Сохраняем позицию продажи
                _context.Add(salePosition);

                // 2. БИЗНЕС-ЛОГИКА: Списание товара со склада
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == salePosition.PartId);
                if (stock != null)
                {
                    stock.Quantity -= salePosition.Quantity; // Уменьшаем количество
                    _context.Update(stock);
                }

                await _context.SaveChangesAsync();

                // Возвращаемся обратно в детали продажи
                return RedirectToAction("Details", "Sales", new { id = salePosition.SaleId });
            }

            ViewData["SaleId"] = salePosition.SaleId;
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", salePosition.PartId);
            return View(salePosition);
        }

        // =========================================================
        // EDIT: Редактирование позиции
        // =========================================================

        // GET: SalePositions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var salePosition = await _context.SalePositions.FindAsync(id);
            if (salePosition == null) return NotFound();

            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", salePosition.PartId);
            return View(salePosition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SaleId,PartId,Quantity,Price")] SalePosition salePosition)
        {
            if (id != salePosition.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salePosition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalePositionExists(salePosition.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Details", "Sales", new { id = salePosition.SaleId });
            }
            ViewData["PartId"] = new SelectList(_context.Parts, "Id", "Name", salePosition.PartId);
            return View(salePosition);
        }

        // =========================================================
        // DELETE: Удаление позиции (опционально, но полезно)
        // =========================================================

        // GET: SalePositions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var salePosition = await _context.SalePositions
                .Include(s => s.Part)
                .Include(s => s.Sale)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (salePosition == null) return NotFound();

            return View(salePosition);
        }

        // POST: SalePositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salePosition = await _context.SalePositions.FindAsync(id);
            if (salePosition != null)
            {
                // При удалении продажи можно вернуть товар на склад (опциональная логика)
                // var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.PartId == salePosition.PartId);
                // if (stock != null) stock.Quantity += salePosition.Quantity;

                _context.SalePositions.Remove(salePosition);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Sales", new { id = salePosition.SaleId });
            }
            return RedirectToAction(nameof(Index), "Sales"); // На всякий случай
        }

        private bool SalePositionExists(int id)
        {
            return _context.SalePositions.Any(e => e.Id == id);
        }
    }
}