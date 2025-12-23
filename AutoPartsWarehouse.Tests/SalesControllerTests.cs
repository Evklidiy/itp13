using System;
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
    public class SalesControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult()
        {
            using var context = GetDatabaseContext();
            context.Sales.Add(new Sale { ClientName = "Client 1" });
            context.SaveChanges();
            var controller = new SalesController(context);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
        }

        [Fact]
        public async Task Create_AddsSale_AndRedirects()
        {
            using var context = GetDatabaseContext();
            var controller = new SalesController(context);
            var sale = new Sale { ClientName = "New Client", Date = DateTime.Now };

            var result = await controller.Create(sale);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal(1, context.Sales.Count());
        }

        [Fact]
        public async Task Create_ReturnsView_WhenInvalid()
        {
            using var context = GetDatabaseContext();
            var controller = new SalesController(context);
            controller.ModelState.AddModelError("Error", "Sample Error");
            var sale = new Sale();

            var result = await controller.Create(sale);

            Assert.IsType<ViewResult>(result);
            Assert.Equal(0, context.Sales.Count());
        }

        [Fact]
        public async Task Details_ReturnsNotFound_IfIdNull()
        {
            using var context = GetDatabaseContext();
            var controller = new SalesController(context);

            var result = await controller.Details(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ReturnsViewWithSale()
        {
            using var context = GetDatabaseContext();
            var sale = new Sale { ClientName = "Test" };
            context.Sales.Add(sale);
            context.SaveChanges();
            var controller = new SalesController(context);

            var result = await controller.Details(sale.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(sale, viewResult.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesSale()
        {
            using var context = GetDatabaseContext();
            var sale = new Sale { ClientName = "Delete Me" };
            context.Sales.Add(sale);
            context.SaveChanges();
            var controller = new SalesController(context);

            await controller.DeleteConfirmed(sale.Id);

            Assert.Empty(context.Sales);
        }
    }
}