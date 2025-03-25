using System;
using System.Text.Json;
using ThAmCo.CheapestProduct.Dtos;
using ThAmCo.CheapestProducts.Services.CheapestProduct;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ThAmCo.CheapestProduct.Services.CheapestProducts
{
    public class LowestProducts : ILowestPriceService
    {
        private readonly HttpClient _httpclient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        
        public LowestProducts(HttpClient httpclient, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpclient = httpclient;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }



        public async Task<IEnumerable<LowestProductDto>> GetLowestPriceAsync()
        {
            var totalClient = _httpClientFactory.CreateClient();
            totalClient.BaseAddress = new Uri(_configuration["TokenAuthority"]);
            var tokenParams = new Dictionary <string, string>
            {
                {"client_id", _configuration["ClientId"]},
                {"client_secret", _configuration["ClientSecret"]},
                {"grant_type", "client_credentials"},
                {"audience", _configuration["Audience"]}
            };

            
            var tokenForm = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await totalClient.PostAsync("oauth/token", tokenForm);
            var contentString = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenDto>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await _httpclient.GetAsync("debug/CheapestProducts");
            
            var productcontextcontentString = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<LowestProductDto>>(productcontextcontentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (products == null)
            {
                throw new InvalidOperationException("Failed to deserialize products.");
            }

            // Dictionary to store grouped results
            Dictionary<string, List<LowestProductDto>> lowestProduct = new Dictionary<string, List<LowestProductDto>>();

            // Iterate over the list and add to the dictionary
            foreach (var lowestProductDto in products)
            {
                if (!lowestProduct.ContainsKey(lowestProductDto.Name))
                {
                    lowestProduct[lowestProductDto.Name] = new List<LowestProductDto>();
                }
                lowestProduct[lowestProductDto.Name].Add(lowestProductDto);
            }
            List<LowestProductDto> lowestProductList = new List<LowestProductDto>();

            foreach (var key in lowestProduct.Keys)
            {
                lowestProductList.Add(lowestProduct[key].OrderBy(p => p.Price).First());
            }

            return lowestProductList;
        }

        async Task<LowestProductDto> GetLowestProductAsync(int id)
        {
            var totalClient = _httpClientFactory.CreateClient();
            totalClient.BaseAddress = new Uri(_configuration["TokenAuthority"]);
            var tokenParams = new Dictionary <string, string>
            {
                {"client_id", _configuration["ClientId"]},
                {"client_secret", _configuration["ClientSecret"]},
                {"grant_type", "client_credentials"},
                {"audience", _configuration["Audience"]}
            };

            
            var tokenForm = new FormUrlEncodedContent(tokenParams);
            var tokenResponse = await totalClient.PostAsync("oauth/token", tokenForm);
            var contentString = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<TokenDto>(contentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var response = await _httpclient.GetAsync("debug/repo");
            var productcontextcontentString = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<IEnumerable<LowestProductDto>>(productcontextcontentString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (products == null)
            {
                throw new InvalidOperationException("Failed to deserialize products.");
            }

            // Dictionary to store grouped results
            Dictionary<string, List<LowestProductDto>> lowestProduct = new Dictionary<string, List<LowestProductDto>>();
            
            List<LowestProductDto> lowestProductList = new List<LowestProductDto>();

            return products.FirstOrDefault(p => p.Id == id);
        }

        Task<LowestProductDto> ILowestPriceService.GetLowestProductAsync(int id)
        {
            return GetLowestProductAsync(id);
        }
    }
}