using System;

namespace ThAmCo.CheapestProducts.Services.CheapestProduct
{
    public class LowestPriceServiceFake : ILowestPriceService
    {
        private readonly LowestProductDto[] _products =
        {
            new LowestProductDto { Id = 1, Name = "Product 1", Description = "Fake Description 1", Price = 1.99m },
            new LowestProductDto { Id = 2, Name = "Product 2", Description = "Fake Description 2", Price = 2.99m },
            new LowestProductDto { Id = 3, Name = "Product 3", Description = "Fake Description 3", Price = 3.99m },
            new LowestProductDto { Id = 4, Name = "Product 4", Description = "Fake Description 4", Price = 4.99m },
            new LowestProductDto { Id = 5, Name = "Product 5", Description = "Fake Description 5", Price = 5.99m }
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
    }
}