
using Mango.Services.OrderAPI.Models.Dtos;

namespace Mango.Services.OrderAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
