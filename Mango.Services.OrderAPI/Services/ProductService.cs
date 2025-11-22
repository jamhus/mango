using Mango.Services.OrderAPI.Models.Dtos;
using Mango.Services.OrderAPI.Services.Interfaces;
using Newtonsoft.Json;

namespace Mango.Services.OrderAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync("api/product");
            var content = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseDto>(content);
            if(deserializedResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(deserializedResponse.Result)!)!;
            }
            return new List<ProductDto>();
        }
    }
}
