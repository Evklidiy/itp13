using System.Diagnostics;
using AutoPartsWarehouse.Controllers;
using AutoPartsWarehouse.Models;
using Microsoft.AspNetCore.Http; // Добавлено
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AutoPartsWarehouse.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult()
        {
            var controller = new HomeController(null!);
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorModel()
        {
            // Arrange
            var controller = new HomeController(null!);

            // ИСПРАВЛЕНИЕ: Создаем контекст контроллера с HttpContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            // Устанавливаем TraceIdentifier, чтобы не было null
            controller.HttpContext.TraceIdentifier = "TestTraceId";

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            // Проверяем, что ID пробросился
            Assert.Equal("TestTraceId", model.RequestId);
        }
    }
}