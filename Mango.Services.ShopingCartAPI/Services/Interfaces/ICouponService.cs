using Mango.Services.ShopingCartAPI.Models.Dtos;

namespace Mango.Services.ShopingCartAPI.Services.Interfaces
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponByCodeAsync(string couponCode);
    }
}
