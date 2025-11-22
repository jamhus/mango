using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dtos;


namespace Mango.Services.OrderAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDto, OrderHeader>().ReverseMap();
            CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();

            CreateMap<OrderHeaderDto,CartHeaderDto>()
                .ForMember(dest => dest.CartTotal , u=> u.MapFrom(src => src.OrderTotal)).ReverseMap();

            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

        }
    }
}
