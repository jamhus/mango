using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> CreateProductAsync(ProductDto product);
        Task<ResponseDto?> UpdateProductAsync(ProductDto product);
        Task<ResponseDto?> DeleteProductAsync(int id);
    }
}
