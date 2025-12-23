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
    public class SuppliesControllerTests
    {
        // Вспомогательный метод
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальная БД для каждого теста
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            using var context = GetDatabaseContext();
            context.Supplies.Add(new Supply { Status = "New" });
            await context.SaveChangesAsync();
            var controller = new SuppliesController(context);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_Valid_AddsSupply()
        {
            using var context = GetDatabaseContext();
            var controller = new SuppliesController(context);
            var supply = new Supply { Status = "Pending", Date = DateTime.Now };

            var result = await controller.Create(supply);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal(1, await context.Supplies.CountAsync());
        }

        [Fact]
        public async Task Create_Invalid_ReturnsView()
        {
            using var context = GetDatabaseContext();
            var controller = new SuppliesController(context);
            controller.ModelState.AddModelError("Error", "Test Error");
            var supply = new Supply();

            var result = await controller.Create(supply);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_UpdatesStatus()
        {
            using var context = GetDatabaseContext();
            var supply = new Supply { Status = "Old" };
            context.Supplies.Add(supply);
            await context.SaveChangesAsync();

            // Очищаем трекер, чтобы эмулировать новый запрос
            context.ChangeTracker.Clear();

            var controller = new SuppliesController(context);

            // Создаем объект с обновленными данными, используя ID из базы
            var updatedSupply = new Supply { Id = supply.Id, Status = "Received", Date = supply.Date };

            var result = await controller.Edit(supply.Id, updatedSupply);

            Assert.IsType<RedirectToActionResult>(result);
            var fromDb = await context.Supplies.FindAsync(supply.Id);
            Assert.Equal("Received", fromDb.Status);
        }

        [Fact]
        public async Task Details_ReturnsSupplyWithPositions()
        {
            using var context = GetDatabaseContext();

            // 1. Создаем запчасть и поставщика (чтобы не было пустых ссылок)
            var part = new Part { Name = "Test Part", Article = "123" };
            context.Parts.Add(part);

            var supplier = new Supplier { Name = "Test Supplier" };
            context.Suppliers.Add(supplier);

            await context.SaveChangesAsync();

            // 2. Создаем поставку
            var supply = new Supply { Status = "TestDetails", SupplierId = supplier.Id };
            context.Supplies.Add(supply);
            await context.SaveChangesAsync();

            // 3. Создаем позицию в поставке (чтобы Include(x => x.Positions) отработал на реальных данных)
            var pos = new SupplyPosition { SupplyId = supply.Id, PartId = part.Id, Quantity = 5, PurchasePrice = 100 };
            context.SupplyPositions.Add(pos);
            await context.SaveChangesAsync();

            // ВАЖНО: Очищаем трекер, чтобы эмулировать чистый запрос от контроллера к базе
            context.ChangeTracker.Clear();

            var controller = new SuppliesController(context);

            // Act
            var result = await controller.Details(supply.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Supply>(viewResult.Model);

            // Проверяем, что вернулась нужная поставка
            Assert.Equal(supply.Id, model.Id);
            Assert.Equal("TestDetails", model.Status);

            // Проверяем, что подгрузились позиции и названия запчастей
            Assert.NotNull(model.Positions);
            Assert.Single(model.Positions);
            Assert.Equal("Test Part", model.Positions[0].Part?.Name);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSupply()
        {
            using var context = GetDatabaseContext();
            var supply = new Supply { Status = "To Delete" };
            context.Supplies.Add(supply);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var controller = new SuppliesController(context);

            await controller.DeleteConfirmed(supply.Id);

            Assert.Null(await context.Supplies.FindAsync(supply.Id));
        }
    }
}