using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework; // Use NUnit for testing
using ThAmCo.CheapestProducts.Services.CheapestProduct;
using ThAmCoFrontEnd.Controllers;

namespace ThAmCoFrontEnd.Tests
{
    [TestFixture] // NUnit test class attribute
    public class HomeControllerTests
    {
        [Test] // NUnit test method attribute
        public async Task Index_RetriesOnTransientFailures()
        {
            // Arrange
            var mockService = new Mock<ILowestPriceService>();
            var logger = Mock.Of<ILogger<HomeController>>();

            // Simulate transient failures for the first two attempts, then succeed
            mockService
                .SetupSequence(service => service.GetLowestPriceAsync())
                .ThrowsAsync(new Exception("Simulated transient failure")) // First attempt fails
                .ThrowsAsync(new Exception("Simulated transient failure")) // Second attempt fails
                .ReturnsAsync(new List<LowestProductDto> // Third attempt succeeds
                {
                    new LowestProductDto { Id = 1, Name = "Product 1", Price = 10, StockLevel = 5 },
                    new LowestProductDto { Id = 2, Name = "Product 2", Price = 20, StockLevel = 3 }
                });

            var controller = new HomeController(logger, mockService.Object);

            // Act
            var result = await controller.Index(null, null, null);

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>()); // Check the result is a ViewResult
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.AssignableTo<IEnumerable<LowestProductDto>>()); // Check the model type

            var model = (IEnumerable<LowestProductDto>)viewResult.Model;

            // Log the test data
            TestContext.WriteLine("Test Data:");
            foreach (var product in model)
            {
                TestContext.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, StockLevel: {product.StockLevel}");
            }

            Assert.That(model, Is.Not.Null); // Check that the model is not null
            Assert.That(model.Count(), Is.EqualTo(2)); // Check that the model contains 2 items
            Assert.That(model.Any(p => p.Name == "Product 1" && p.Price == 10), Is.True); // Check for Product 1
            Assert.That(model.Any(p => p.Name == "Product 2" && p.Price == 20), Is.True); // Check for Product 2

            // Verify that the service was called at least three times due to retries
            mockService.Verify(service => service.GetLowestPriceAsync(), Times.Exactly(3));
        }
    }
}