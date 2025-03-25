using System;

namespace ThAmCo.CheapestProducts.Services.CheapestProduct
{
    public class LowestProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockLevel { get; set; }

    }
}