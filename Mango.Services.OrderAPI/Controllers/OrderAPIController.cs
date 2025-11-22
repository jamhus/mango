using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dtos;
using Mango.Services.OrderAPI.Services.Interfaces;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;

        public OrderAPIController(
            AppDbContext db,
            IProductService productService,
            IMapper mapper)
        {
            _db = db;
            _productService = productService;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();
                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {

                _response.DisplayMessage = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto requestDto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = requestDto.ApprovedUrl,
                    CancelUrl = requestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var Discounts = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = requestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in requestDto.OrderHeader.OrderDetails)
                {
                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(lineItem);
                }

                if(requestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = Discounts;
                }   

                var service = new SessionService();
                Session session = service.Create(options);
                requestDto.StripeSesstionUrl = session.Url;

                OrderHeader orderHeader = await _db.OrderHeaders.FindAsync(requestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync();

                _response.Result = requestDto;

            }
            catch (Exception ex)
            {

                _response.DisplayMessage = "Payment failed";
                _response.IsSuccess = false;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CheckPaymentStatus")]
        public async Task<ResponseDto> CheckPaymentStatus([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = await _db.OrderHeaders.FirstAsync(o => o.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status.ToLower() == "succeeded")
                {
                    orderHeader.Status = SD.Status_Approved;
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    await _db.SaveChangesAsync();
                }

                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);

            }
            catch (Exception ex)
            {

                _response.DisplayMessage = "Payment failed";
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
