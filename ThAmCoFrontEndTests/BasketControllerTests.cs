using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ThAmCoFrontEnd.Controllers;

namespace ThAmCoFrontEnd.Tests
{
    [TestFixture]
    public class BasketControllerTests
    {
        private Mock<ISession> _mockSession;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private BasketController _controller;
        private Dictionary<string, byte[]> _sessionStorage;

        [SetUp]
        public void SetUp()
        {
            _sessionStorage = new Dictionary<string, byte[]>();
            _mockSession = new Mock<ISession>();

            // Simulate ISession behavior
            _mockSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                        .Callback<string, byte[]>((key, value) => _sessionStorage[key] = value);

            _mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                        .Returns((string key, out byte[] value) =>
                        {
                            var exists = _sessionStorage.TryGetValue(key, out var storedValue);
                            value = storedValue;
                            return exists;
                        });

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext
            {
                Session = _mockSession.Object
            };
            _mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            _controller = new BasketController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
        }

        private void SetSession<T>(string key, T value)
        {
            var serializedValue = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
            _mockSession.Object.Set(key, serializedValue);
        }

        private T GetSession<T>(string key)
        {
            if (_mockSession.Object.TryGetValue(key, out var value))
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        [Test]
        public void Index_ReturnsViewResult_WithBasketItems()
        {
            // Arrange
            var basket = new List<BasketItem>
            {
                new BasketItem { ProductId = 1, Name = "Product 1", Price = 10.0, Quantity = 2 },
                new BasketItem { ProductId = 2, Name = "Product 2", Price = 20.0, Quantity = 1 }
            };
            SetSession("Basket", basket);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ViewResult>());
            var viewResult = (ViewResult)result;

            var actualBasket = viewResult.ViewData["BasketItems"] as List<BasketItem>;
            Assert.That(actualBasket, Is.Not.Null);
            Assert.That(actualBasket.Count, Is.EqualTo(basket.Count));

            for (int i = 0; i < basket.Count; i++)
            {
                Assert.That(actualBasket[i].ProductId, Is.EqualTo(basket[i].ProductId));
                Assert.That(actualBasket[i].Name, Is.EqualTo(basket[i].Name));
                Assert.That(actualBasket[i].Price, Is.EqualTo(basket[i].Price));
                Assert.That(actualBasket[i].Quantity, Is.EqualTo(basket[i].Quantity));
            }

            Assert.That(viewResult.ViewData["TotalPrice"], Is.EqualTo(40.0));
        }

        [Test]
        public void AddToBasket_AddsNewItem_WhenItemDoesNotExist()
        {
            // Arrange
            var basket = new List<BasketItem>();
            SetSession("Basket", basket);

            // Act
            var result = _controller.AddToBasket(1, "Product 1", 10.0);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            dynamic data = jsonResult.Value;

            Assert.That(data.success, Is.True);
            Assert.That(data.message, Is.EqualTo("Product added to basket!"));

            var updatedBasket = GetSession<List<BasketItem>>("Basket");
            Assert.That(updatedBasket.Count, Is.EqualTo(1));
            Assert.That(updatedBasket[0].ProductId, Is.EqualTo(1));
            Assert.That(updatedBasket[0].Name, Is.EqualTo("Product 1"));
            Assert.That(updatedBasket[0].Price, Is.EqualTo(10.0));
            Assert.That(updatedBasket[0].Quantity, Is.EqualTo(1));
        }

        [Test]
        public void AddToBasket_IncrementsQuantity_WhenItemExists()
        {
            // Arrange
            var basket = new List<BasketItem>
            {
                new BasketItem { ProductId = 1, Name = "Product 1", Price = 10.0, Quantity = 1 }
            };
            SetSession("Basket", basket);

            // Act
            var result = _controller.AddToBasket(1, "Product 1", 10.0);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            dynamic data = jsonResult.Value;

            Assert.That(data.success, Is.True);
            Assert.That(data.message, Is.EqualTo("Product added to basket!"));

            var updatedBasket = GetSession<List<BasketItem>>("Basket");
            Assert.That(updatedBasket.Count, Is.EqualTo(1));
            Assert.That(updatedBasket[0].Quantity, Is.EqualTo(2));
        }

        [Test]
        public void RemoveFromBasket_RemovesItem_WhenItemExists()
        {
            // Arrange
            var basket = new List<BasketItem>
            {
                new BasketItem { ProductId = 1, Name = "Product 1", Price = 10.0, Quantity = 1 }
            };
            SetSession("Basket", basket);

            // Act
            var result = _controller.RemoveFromBasket(1);

            // Assert
            Assert.That(result, Is.InstanceOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            dynamic data = jsonResult.Value;

            Assert.That(data.success, Is.True);
            Assert.That(data.message, Is.EqualTo("Product removed from basket!"));

            var updatedBasket = GetSession<List<BasketItem>>("Basket");
            Assert.That(updatedBasket.Count, Is.EqualTo(0));
        }
    }
}