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
    public class SalePositionsControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Create_Post_Valid_DecreasesStockQuantity()
        {
            using var context = GetDatabaseContext();

            // 1. Создаем запчасть и склад (было 20 шт)
            var part = new Part { Name = "Brakes" };
            context.Parts.Add(part);
            context.SaveChanges();

            var stock = new Stock { PartId = part.Id, Quantity = 20 };
            context.Stocks.Add(stock);

            var sale = new Sale { ClientName = "Client" };
            context.Sales.Add(sale);
            await context.SaveChangesAsync();

            var controller = new SalePositionsController(context);

            // 2. Продаем 3 штуки
            var pos = new SalePosition { SaleId = sale.Id, PartId = part.Id, Quantity = 3, Price = 500 };

            // Act
            var result = await controller.Create(pos);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            // ПРОВЕРКА ЛОГИКИ: Остаток должен стать 17 (20 - 3)
            var updatedStock = await context.Stocks.FirstOrDefaultAsync(s => s.PartId == part.Id);
            Assert.Equal(17, updatedStock.Quantity);
        }

        [Fact]
        public void Create_Get_ReturnsViewWithData()
        {
            using var context = GetDatabaseContext();
            var controller = new SalePositionsController(context);
            var result = controller.Create(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.ViewData["SaleId"]);
        }
    }
}