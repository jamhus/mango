using Mango.Services.ShopingCartAPI.Models.Dtos;

namespace Mango.Services.ShopingCartAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
