using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ThAmCo.CheapestProducts.Services.CheapestProduct;
using ThAmCoFrontEnd.Controllers;
using ThAmCoFrontEnd.Models;

namespace HomeControllerTests.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ILowestPriceService> _mockService;
        private Mock<ILogger<HomeController>> _mockLogger;
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<ILowestPriceService>();
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object, _mockService.Object);
        }

        [Test]
        public async Task Index_ReturnsViewResult_WithFilteredProducts()
        {
            // Arrange
            var products = new List<LowestProductDto>
            {
                new LowestProductDto { Id = 1, Name = "Product 1", Price = 10, StockLevel = 5 },
                new LowestProductDto { Id = 2, Name = "Product 2", Price = 20, StockLevel = 3 },
                new LowestProductDto { Id = 3, Name = "Product 3", Price = 30, StockLevel = 0 }
            };

            _mockService.Setup(service => service.GetLowestPriceAsync())
                        .ReturnsAsync(products);

            // Act
            var result = await _controller.Index(15, 25, "Product");

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<LowestProductDto>>());

            var model = (IEnumerable<LowestProductDto>)viewResult.Model;
            Assert.That(model.Count(), Is.EqualTo(1));
            Assert.That(model.First().Name, Is.EqualTo("Product 2"));
        }

        [Test]
        public async Task FilterProducts_ReturnsPartialView_WithFilteredProducts()
        {
            // Arrange
            var products = new List<LowestProductDto>
            {
                new LowestProductDto { Id = 1, Name = "Product 1", Price = 10, StockLevel = 5 },
                new LowestProductDto { Id = 2, Name = "Product 2", Price = 20, StockLevel = 3 },
                new LowestProductDto { Id = 3, Name = "Product 3", Price = 30, StockLevel = 0 }
            };

            _mockService.Setup(service => service.GetLowestPriceAsync())
                        .ReturnsAsync(products);

            // Act
            var result = await _controller.FilterProducts(10, 20, "Product");

            // Assert
            Assert.That(result, Is.InstanceOf<PartialViewResult>());
            var partialViewResult = (PartialViewResult)result;
            Assert.That(partialViewResult.ViewName, Is.EqualTo("_ProductList"));
            Assert.That(partialViewResult.Model, Is.AssignableTo<IEnumerable<LowestProductDto>>());

            var model = (IEnumerable<LowestProductDto>)partialViewResult.Model;
            Assert.That(model.Count(), Is.EqualTo(2));
            Assert.That(model.Any(p => p.Name == "Product 1"), Is.True);
            Assert.That(model.Any(p => p.Name == "Product 2"), Is.True);
        }

        [Test]
        public async Task Index_ReturnsEmptyView_WhenNoProductsMatchFilter()
        {
            // Arrange
            var products = new List<LowestProductDto>
            {
                new LowestProductDto { Id = 1, Name = "Product 1", Price = 10, StockLevel = 5 },
                new LowestProductDto { Id = 2, Name = "Product 2", Price = 20, StockLevel = 3 }
            };

            _mockService.Setup(service => service.GetLowestPriceAsync())
                        .ReturnsAsync(products);

            // Act
            var result = await _controller.Index(50, 100, "Nonexistent");

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<LowestProductDto>>());

            var model = (IEnumerable<LowestProductDto>)viewResult.Model;
            Assert.That(model.Count(), Is.EqualTo(0));
        }
    }
}