using AutoMapper;
using Mango.Services.ShopingCartAPI.Models;
using Mango.Services.ShopingCartAPI.Models.Dtos;

namespace Mango.Services.ShopingCartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartHeaderDto, CartHeader>();
            CreateMap<CartHeader, CartHeaderDto>();

            CreateMap<CartDetailsDto, CartDetails>();
            CreateMap<CartDetails, CartDetailsDto>();
        }
    }
}
