using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();
            ResponseDto? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else TempData["Error"] = response.DisplayMessage;
            return View(list);
        }
        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            ProductDto? product = new();
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);

            if (response != null && response.IsSuccess)
            {
                product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(product);
            }
            else TempData["Error"] = response.DisplayMessage;
            return View(product);
        }


    }
}
