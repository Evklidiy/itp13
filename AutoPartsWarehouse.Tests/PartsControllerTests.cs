using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPartsWarehouse.Controllers;
using AutoPartsWarehouse.Data;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AutoPartsWarehouse.Tests
{
    public class PartsControllerTests
    {
        // Вспомогательный метод для создания изолированной БД
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Уникальное имя для каждого теста
                .Options;
            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithParts()
        {
            // Arrange
            using var context = GetDatabaseContext();
            context.Parts.Add(new Part { Name = "TestPart", Article = "123" });
            context.SaveChanges();
            var controller = new PartsController(context);

            // Act
            var result = await controller.Index(null, 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Part>>(viewResult.ViewData.Model);
            Assert.Single(model); // Проверяем, что в списке 1 элемент
        }

        [Fact]
        public async Task Index_SearchString_ReturnsFilteredParts()
        {
            // Arrange
            using var context = GetDatabaseContext();
            context.Parts.Add(new Part { Name = "FilterMe", Article = "A1" });
            context.Parts.Add(new Part { Name = "SkipMe", Article = "B2" });
            context.SaveChanges();
            var controller = new PartsController(context);

            // Act
            var result = await controller.Index("FilterMe", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Part>>(viewResult.ViewData.Model);
            Assert.Single(model);
            Assert.Equal("FilterMe", model.First().Name);
        }

        [Fact]
        public async Task Details_ReturnsNotFound_WhenIdIsNull()
        {
            // Arrange
            using var context = GetDatabaseContext();
            var controller = new PartsController(context);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_ValidModel_AddsPartAndStock()
        {
            // Arrange
            using var context = GetDatabaseContext();
            var controller = new PartsController(context);
            var newPart = new Part { Name = "New Part", Article = "NP-001", Manufacturer = "Test", PurchasePrice = 100, SalePrice = 200 };

            // Act
            var result = await controller.Create(newPart);

            // Assert
            // 1. Проверяем редирект
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // 2. Проверяем, что запчасть создалась
            Assert.Equal(1, context.Parts.Count());

            // 3. БИЗНЕС-ЛОГИКА: Проверяем, что создался Склад (Stock)
            Assert.Equal(1, context.Stocks.Count());
            Assert.Equal("New Part", context.Stocks.First().Part?.Name);
        }

        [Fact]
        public async Task Edit_ValidModel_UpdatesPart()
        {
            // Arrange
            using var context = GetDatabaseContext();
            var part = new Part { Name = "Old Name", Article = "Old", Manufacturer = "Old" };
            context.Parts.Add(part);
            context.SaveChanges();
            var controller = new PartsController(context);

            // Act
            part.Name = "New Name";
            var result = await controller.Edit(part.Id, part);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            var updatedPart = context.Parts.Find(part.Id);
            Assert.Equal("New Name", updatedPart.Name);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesPart()
        {
            // Arrange
            using var context = GetDatabaseContext();
            var part = new Part { Name = "To Delete", Article = "D1" };
            context.Parts.Add(part);
            context.SaveChanges();
            var controller = new PartsController(context);

            // Act
            var result = await controller.DeleteConfirmed(part.Id);

            // Assert
            Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(context.Parts);
        }
    }
}