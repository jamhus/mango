using AutoMapper;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dtos;

namespace Mango.Services.ShoppingCartAPI
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
