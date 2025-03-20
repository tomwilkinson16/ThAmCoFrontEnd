using System.Collections.Generic;
using System.Threading.Tasks;

namespace ThAmCo.CheapestProducts.Services.CheapestProduct
{
    public interface ILowestPriceService
    {
        Task<IEnumerable<LowestProductDto>> GetLowestPriceAsync();
        //get a single product
        Task<LowestProductDto> GetLowestProductAsync(int id);
    }
}