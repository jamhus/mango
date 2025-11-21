using Mango.Services.EmailAPI.Models.Dtos;

namespace Mango.Services.EmailAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
