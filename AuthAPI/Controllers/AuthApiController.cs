using AuthAPI.Models.Dtos;
using AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ResponseDto _response;

        public AuthApiController(IAuthService authService)
        {
            _authService = authService;
            _response = new ResponseDto();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.RegisterAsync(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.LoginAsync(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Invalid email or password.";
                return BadRequest(_response);
            }

            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole(RegistrationRequestDto model)
        {
            var successfull = await _authService.AssignRoleAsync(model.Email, model.Role);
            if (!successfull)
            {
                _response.IsSuccess = false;
                _response.DisplayMessage = "Invalid email or role";
                return BadRequest(_response);
            }

            return Ok(_response);
        }
    }
}
