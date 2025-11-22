using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseDto?> CheckPaymentStatus(int orderHeaderId);
        Task<ResponseDto?> CreateOrderAsync(CartDto cartDto);
        Task<ResponseDto?> CreateStripeSesstion(StripeRequestDto requestDto);
    }
}
