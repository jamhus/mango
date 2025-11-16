using Mango.Web.Models;
using Mango.Web.Services;
using Mango.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(model);

                if (response.IsSuccess)
                {
                    TempData["Success"] = "Product created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = response.DisplayMessage;
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if (response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["Error"] = response.DisplayMessage;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductDto model)
        {
            ResponseDto? response = await _productService.UpdateProductAsync(model);
            if (response.IsSuccess)
            {
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response.DisplayMessage;
                return View(model);
            }   

        }

        public async Task<IActionResult> Delete(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);

            }
            else
            {
                TempData["Error"] = response?.DisplayMessage;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDto model)
        {

            ResponseDto? response = await _productService.DeleteProductAsync(model.ProductId);

            if (response.IsSuccess)
            {
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = response.DisplayMessage;
            }

            return View(model);
        }
    }
}
