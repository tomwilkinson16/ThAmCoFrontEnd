using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ThAmCo.CheapestProducts.Services.CheapestProduct;
using ThAmCoFrontEnd.Models;

namespace ThAmCoFrontEnd.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILowestPriceService _lowestPriceService;

    public HomeController(ILogger<HomeController> logger, ILowestPriceService lowestPriceService)
    {
        _logger = logger;
        _lowestPriceService = lowestPriceService;
    }

    public async Task<IActionResult> Index(decimal? minPrice, decimal? maxPrice, string searchQuery)
    {
        // Fetch products using the service
        var products = await _lowestPriceService.GetLowestPriceAsync();

        // Apply filtering based on minPrice and maxPrice
        if (minPrice.HasValue)
        {
            products = products.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= maxPrice.Value);
        }

        // Apply search filtering
        if (!string.IsNullOrEmpty(searchQuery))
        {
            products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }

        // Pass the products to the view
        return View(products);
    }

    [HttpGet]
public async Task<IActionResult> FilterProducts(decimal? minPrice, decimal? maxPrice, string searchQuery)
{
    // Fetch products using the service
    var products = await _lowestPriceService.GetLowestPriceAsync();

    // Apply filtering based on minPrice and maxPrice
    if (minPrice.HasValue)
    {
        products = products.Where(p => p.Price >= minPrice.Value);
    }

    if (maxPrice.HasValue)
    {
        products = products.Where(p => p.Price <= maxPrice.Value);
    }

    // Apply search filtering
    if (!string.IsNullOrEmpty(searchQuery))
    {
        products = products.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    // Return the filtered products as a partial view
    return PartialView("_ProductList", products);
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}