using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice)
        {
            var products = await _lowestPriceService.GetLowestPriceAsync();

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            return View(products);
        }
    }
}