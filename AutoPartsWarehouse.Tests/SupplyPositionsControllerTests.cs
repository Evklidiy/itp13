using System;
using System.Threading.Tasks;
using AutoPartsWarehouse.Controllers;
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoPartsWarehouse.Tests
{
    public class SupplyPositionsControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_Post_Valid_IncreasesStockQuantity()
        {
            using var context = GetDatabaseContext();

            // 1. Создаем запчасть и склад (было 10 шт)
            var part = new Part { Name = "Filter" };
            context.Parts.Add(part);
            context.SaveChanges();

            var stock = new Stock { PartId = part.Id, Quantity = 10 };
            context.Stocks.Add(stock);

            var supply = new Supply { Status = "New" };
            context.Supplies.Add(supply);
            await context.SaveChangesAsync();

            var controller = new SupplyPositionsController(context);

            // 2. Создаем позицию поставки: +5 штук
            var pos = new SupplyPosition { SupplyId = supply.Id, PartId = part.Id, Quantity = 5, PurchasePrice = 100 };

            // Act
            var result = await controller.Create(pos);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);

            // ПРОВЕРКА ЛОГИКИ: Остаток должен стать 15 (10 + 5)
            var updatedStock = await context.Stocks.FirstOrDefaultAsync(s => s.PartId == part.Id);
            Assert.Equal(15, updatedStock.Quantity);
        }

        [Fact]
        public async Task Create_Post_Invalid_ReturnsView()
        {
            using var context = GetDatabaseContext();
            var controller = new SupplyPositionsController(context);
            controller.ModelState.AddModelError("Err", "Err");
            var pos = new SupplyPosition();

            var result = await controller.Create(pos);

            Assert.IsType<ViewResult>(result);
        }
    }
}