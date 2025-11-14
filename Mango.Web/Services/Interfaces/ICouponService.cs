using Mango.Web.Models;

namespace Mango.Web.Services.Interfaces
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponByCodeAsync(string couponCode);
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> CreateCouponAsync(CouponDto coupon);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto coupon);
        Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}
