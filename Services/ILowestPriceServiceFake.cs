using System;

namespace ThAmCo.CheapestProducts.Services.CheapestProduct
{
    public class LowestPriceServiceFake : ILowestPriceService
    {
        private readonly LowestProductDto[] _products =
        {
            new LowestProductDto { Id = 1, Name = "Product 1", Description = "Fake Description 1", ImageUrl = "https://tinyurl.com/mshmsux2", Price = 1.99m, StockLevel = 10 },
            new LowestProductDto { Id = 2, Name = "Product 2", Description = "Fake Description 2", ImageUrl = "https://tinyurl.com/mshmsux2" ,Price = 2.99m, StockLevel = 20 },
            new LowestProductDto { Id = 3, Name = "Product 3", Description = "Fake Description 3", ImageUrl = "https://tinyurl.com/mshmsux2", Price = 3.99m, StockLevel = 30 },
            new LowestProductDto { Id = 4, Name = "Product 4", Description = "Fake Description 4", ImageUrl = "https://tinyurl.com/mshmsux2", Price = 4.99m, StockLevel = 40 },
            new LowestProductDto { Id = 5, Name = "Product 1", Description = "Fake Description 5", ImageUrl = "https://tinyurl.com/mshmsux2", Price = 1.33m, StockLevel = 50 }
        };

        public Task<IEnumerable<LowestProductDto>> GetLowestPriceAsync()
        {
            IEnumerable<LowestProductDto> products = _products.ToList();
            return Task.FromResult(products);
        }


        public Task<LowestProductDto> GetLowestProductAsync(int id)
        {
            LowestProductDto product = _products.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(product);
            // throw new NotImplementedException();
        }

        public Task<IEnumerable<LowestProductDto>> SearchProductsAsync(string keyword)
        {
            var result = _products.Where(p => p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(result);
        }
    }
}