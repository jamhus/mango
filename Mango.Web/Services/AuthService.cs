using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;

namespace Mango.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto roleAssignRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = roleAssignRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assignRole",

            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login",
            }, false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registerRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register",
            }, false);
        }
    }
}
