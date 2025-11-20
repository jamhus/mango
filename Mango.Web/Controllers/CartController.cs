using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            CartDto cartDto = await LoadCartByUserIdAsync();
            return View(cartDto);
        }

        private async Task<CartDto?> LoadCartByUserIdAsync()
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var response = await _cartService.GetCartByUserIdAsync(userId!);
            if (response != null && response.IsSuccess && response.Result != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result)!);
            }
            return null;
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if(response != null && response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = string.Empty;
            ResponseDto? response = await _cartService.ApplyCouponAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon applied successfully";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {

            CartDto cart = await LoadCartByUserIdAsync();
            cart.CartHeader.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _cartService.EmailCartAsync(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
