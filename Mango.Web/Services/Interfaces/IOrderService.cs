using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
    }
}
