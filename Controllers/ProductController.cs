using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ThAmCo.CheapestProducts.Services.CheapestProduct;

namespace ThAmCoFrontEnd.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILowestPriceService _lowestPriceService;

        public ProductController(ILowestPriceService lowestPriceService)
        {
            _lowestPriceService = lowestPriceService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _lowestPriceService.GetLowestPriceAsync();
            return View(products);
        }
    }
}