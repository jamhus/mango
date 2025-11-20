using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
