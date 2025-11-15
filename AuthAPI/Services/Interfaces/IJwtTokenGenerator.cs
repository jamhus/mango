using AuthAPI.Models;

namespace Mango.Services.AuthAPI.Services.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string CreateToken(ApplicationUser user);
    }
}
