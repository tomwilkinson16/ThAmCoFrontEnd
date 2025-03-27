using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ThAmCoFrontEnd.Extensions;

namespace ThAmCoFrontEnd.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        [HttpGet("basket")]
        public IActionResult Index()
        {
            // Retrieve basket items from the session
            var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

            // Calculate total price
            ViewBag.BasketItems = basket;
            ViewBag.TotalPrice = basket.Sum(item => item.Quantity * item.Price);

            return View();
        }

        [HttpPost]
        public IActionResult AddToBasket(int productId, string productName, double productPrice)
        {
            // Retrieve the basket from the session or create a new one
            var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

            // Check if the product already exists in the basket
            var existingItem = basket.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                // Increment the quantity if the product already exists
                existingItem.Quantity++;
            }
            else
            {
                // Add a new product to the basket
                basket.Add(new BasketItem
                {
                    ProductId = productId,
                    Name = productName,
                    Price = productPrice,
                    Quantity = 1
                });
            }

            // Save the updated basket back to the session
            HttpContext.Session.Set("Basket", basket);

            return Json(new { success = true, message = "Product added to basket!" });
        }

        [HttpPost]
        public IActionResult RemoveFromBasket(int productId)
        {
            // Retrieve the basket from the session
            var basket = HttpContext.Session.Get<List<BasketItem>>("Basket") ?? new List<BasketItem>();

            // Remove the product from the basket
            var itemToRemove = basket.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                basket.Remove(itemToRemove);
            }

            // Save the updated basket back to the session
            HttpContext.Session.Set("Basket", basket);

            return Json(new { success = true, message = "Product removed from basket!" });
        }
    }

    // Basket item model
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}