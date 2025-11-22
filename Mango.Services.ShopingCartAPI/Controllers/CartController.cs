using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;
using Mango.Services.ShoppingCartAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly ResponseDto _response;

        public CartController(
            AppDbContext db, 
            IMapper mapper, 
            IProductService productService, 
            ICouponService couponService,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
            _response = new ResponseDto();
        }
        [HttpPost("UpsertCart")]
        public async Task<ResponseDto> UpsertCart(CartDto cartDto)
        {
            try
            {
                var existingCartHeader = await _db.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

                if (existingCartHeader == null)
                {
                    // create cart header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    await _db.CartHeaders.AddAsync(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else
                {
                    // if header is present
                    // check if details are present and has same product

                    var existingCartDetails = await _db.CartDetails.AsNoTracking()
                        .FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId && u.CartHeaderId == existingCartHeader.CartHeaderId);

                    if (existingCartDetails == null)
                    {
                        // create cart details
                        cartDto.CartDetails.First().CartHeaderId = existingCartHeader.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        // update the count of cart details
                        cartDto.CartDetails.First().Count += existingCartDetails.Count;
                        cartDto.CartDetails.First().CartDetailsId = existingCartDetails.CartDetailsId;
                        cartDto.CartDetails.First().CartHeaderId = existingCartDetails.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost("RemoveFromCart")]
        public async Task<ResponseDto> RemoveFromCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails
                    .FirstAsync(u => u.CartDetailsId == cartDetailsId);

                int totalCountOfCartItems = _db.CartDetails
                    .Where(u => u.CartHeaderId == cartDetails.CartHeaderId)
                    .Count();

                _db.CartDetails.Remove(cartDetails);

                if (totalCountOfCartItems == 1)
                {
                    // remove cart header as well
                    CartHeader cartHeader = await _db.CartHeaders
                        .FirstAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeader);
                }
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int CartHeaderId)
        {
            try
            {
                CartHeader cartHeader = await _db.CartHeaders
                    .FirstAsync(u => u.CartHeaderId == CartHeaderId);

                _db.CartHeaders.Remove(cartHeader);

                _db.CartDetails.RemoveRange(_db.CartDetails.Where(u => u.CartHeaderId == cartHeader.CartHeaderId));

                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new();
                CartHeader cartHeader = await _db.CartHeaders
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                if (cartHeader != null)
                {
                    cartDto.CartHeader = _mapper.Map<CartHeaderDto>(cartHeader);
                    IEnumerable<CartDetails> cartDetails = await _db.CartDetails
                        .AsNoTracking()
                        .Where(u => u.CartHeaderId == cartHeader.CartHeaderId)
                        .ToListAsync();

                    cartDto.CartDetails = _mapper.Map<List<CartDetailsDto>>(cartDetails);
                    IEnumerable<ProductDto> products = await _productService.GetProductsAsync();

                    foreach (var detail in cartDto.CartDetails)
                    {
                        detail.Product = products.FirstOrDefault(p => p.ProductId == detail.ProductId);
                        cartDto.CartHeader.CartTotal += (double)(detail.Count * detail.Product.Price);
                    }

                    // apply coupon if any
                    if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                    {
                        CouponDto coupon = await _couponService.GetCouponByCodeAsync(cartDto.CartHeader.CouponCode);
                        if (coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAmount)
                        {
                            cartDto.CartHeader.Discount = coupon.DiscountAmount;
                            cartDto.CartHeader.CartTotal -= coupon.DiscountAmount;
                        }
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeader = await _db.CartHeaders
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);

                if (cartHeader != null)
                {
                    cartHeader.CouponCode = cartDto.CartHeader.CouponCode;
                    _db.CartHeaders.Update(cartHeader);
                    await _db.SaveChangesAsync();
                    _response.Result = true;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<ResponseDto> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = ex.Message;
            }
            return _response;
        }
    }
}
