using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI.Services.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByCodeAsync(string couponCode);
    }
}
