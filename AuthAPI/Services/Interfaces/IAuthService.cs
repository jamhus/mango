using AuthAPI.Models.Dtos;

namespace AuthAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<string> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<bool> AssignRoleAsync(string email, string role);
    }
}
