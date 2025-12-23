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
    public class SuppliersControllerTests
    {
        private ApplicationDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithSuppliers()
        {
            using var context = GetDatabaseContext();
            context.Suppliers.Add(new Supplier { Name = "Supplier 1" });
            context.SaveChanges();
            var controller = new SuppliersController(context);

            var result = await controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Supplier>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Details_ReturnsViewResult_WhenSupplierExists()
        {
            using var context = GetDatabaseContext();
            var supplier = new Supplier { Name = "Supplier 1" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();
            var controller = new SuppliersController(context);

            var result = await controller.Details(supplier.Id);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(supplier, viewResult.Model);
        }

        [Fact]
        public async Task Create_Redirects_WhenModelIsValid()
        {
            using var context = GetDatabaseContext();
            var controller = new SuppliersController(context);
            var supplier = new Supplier { Name = "New Supplier", Rating = 5 };

            var result = await controller.Create(supplier);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Single(context.Suppliers);
        }

        [Fact]
        public async Task Create_ReturnsView_WhenModelIsInvalid()
        {
            using var context = GetDatabaseContext();
            var controller = new SuppliersController(context);
            controller.ModelState.AddModelError("Name", "Required");
            var supplier = new Supplier(); // Invalid (no name)

            var result = await controller.Create(supplier);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_UpdatesSupplier_WhenValid()
        {
            using var context = GetDatabaseContext();
            var supplier = new Supplier { Name = "Old Name" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();
            var controller = new SuppliersController(context);

            supplier.Name = "New Name";
            var result = await controller.Edit(supplier.Id, supplier);

            Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("New Name", context.Suppliers.Find(supplier.Id).Name);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesSupplier()
        {
            using var context = GetDatabaseContext();
            var supplier = new Supplier { Name = "To Delete" };
            context.Suppliers.Add(supplier);
            context.SaveChanges();
            var controller = new SuppliersController(context);

            await controller.DeleteConfirmed(supplier.Id);

            Assert.Empty(context.Suppliers);
        }
    }
}