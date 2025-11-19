using Mango.Services.ShopingCartAPI.Models.Dtos;
using Mango.Services.ShopingCartAPI.Services.Interfaces;
using Newtonsoft.Json;

namespace Mango.Services.ShopingCartAPI.Services
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCouponByCodeAsync(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"api/coupon/GetByCode/{couponCode}");
            var content = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseDto>(content);
            if (deserializedResponse.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(deserializedResponse.Result)!)!;
            }
            return new CouponDto();
        }
    }
}
