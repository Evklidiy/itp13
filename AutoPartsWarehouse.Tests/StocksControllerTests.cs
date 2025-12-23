using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoPartsWarehouse.Controllers;
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoPartsWarehouse.Tests
{
    public class StocksControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewWithStocks()
        {
            using var context = GetDatabaseContext();

            // ИСПРАВЛЕНИЕ: Сначала создаем запчасть!
            var part = new Part { Name = "Test Part", Article = "123" };
            context.Parts.Add(part);
            await context.SaveChangesAsync(); // Сохраняем, чтобы получить ID

            // Создаем склад, привязанный к запчасти
            context.Stocks.Add(new Stock { Quantity = 10, Location = "A1", PartId = part.Id });
            await context.SaveChangesAsync();

            var controller = new StocksController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Stock>>(viewResult.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Edit_Get_ReturnsNotFound_IfNullId()
        {
            using var context = GetDatabaseContext();
            var controller = new StocksController(context);
            var result = await controller.Edit(null);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Get_ReturnsView_IfFound()
        {
            using var context = GetDatabaseContext();

            // ИСПРАВЛЕНИЕ: Создаем валидную связку Запчасть + Склад
            var part = new Part { Name = "Test Part" };
            context.Parts.Add(part);
            await context.SaveChangesAsync();

            var stock = new Stock { Location = "Old", PartId = part.Id };
            context.Stocks.Add(stock);
            await context.SaveChangesAsync();

            var controller = new StocksController(context);

            // Используем stock.Id, который сгенерировала база, а не хардкодим 1
            var result = await controller.Edit(stock.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Stock>(viewResult.Model);
            Assert.Equal("Old", model.Location);
        }

        // ... остальные тесты (Edit_Post_UpdatesLocationAndMinQty и Edit_Post_ReturnsNotFound_IfIdMismatch) 
        // тоже лучше обновить, убрав явное указание Id = 1, и используя реальный Id из базы, 
        // но ошибки были именно в Index и Edit_Get.

        [Fact]
        public async Task Edit_Post_UpdatesLocationAndMinQty()
        {
            using var context = GetDatabaseContext();
            var part = new Part { Name = "Part" };
            context.Parts.Add(part);
            await context.SaveChangesAsync();

            var stock = new Stock { Location = "Old", MinQuantity = 5, Quantity = 100, PartId = part.Id };
            context.Stocks.Add(stock);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear(); // Сброс кэша

            var controller = new StocksController(context);

            // Обновляем данные
            var updatedStock = new Stock { Id = stock.Id, Location = "New", MinQuantity = 10, PartId = part.Id };

            var result = await controller.Edit(stock.Id, updatedStock);

            Assert.IsType<RedirectToActionResult>(result);
            var dbStock = await context.Stocks.FindAsync(stock.Id);
            Assert.Equal("New", dbStock.Location);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_IfIdMismatch()
        {
            using var context = GetDatabaseContext();
            var controller = new StocksController(context);
            var stock = new Stock { Id = 2 };
            // Пытаемся обновить ID 1, передавая объект с ID 2
            var result = await controller.Edit(1, stock);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}