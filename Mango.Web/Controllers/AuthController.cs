using Mango.Web.Models;
using Mango.Web.Services.Interfaces;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService,ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }


        public IActionResult Login()
        {
            LoginRequestDto model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            ResponseDto responseDto = await _authService.LoginAsync(model);
            if(responseDto.IsSuccess)
            {
                var loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                TempData["success"] = "Login successfull";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.DisplayMessage;
                return View(model);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await SignOutUser();
            _tokenProvider.ClearToken();
            return Redirect("Login");
        }
        public IActionResult Register()
        {
            ViewBag.RoleList = RoleList();
            RegistrationRequestDto model = new();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto model)
        {
            ResponseDto result = await _authService.RegisterAsync(model);
            if(result.IsSuccess)
            {
                TempData["success"] = "Registration successfull";
                return RedirectToAction(nameof(Login));

            }
            TempData["error"] = result.DisplayMessage;

            ViewBag.RoleList = RoleList();
            return View(model);
        }

        private List<SelectListItem> RoleList()
        {
            var roleList = new List<SelectListItem>() {
                new SelectListItem() { Text = SD.AdminRole, Value = SD.AdminRole },
                new SelectListItem() { Text = SD.CustomerRole, Value = SD.CustomerRole }
            };
            return roleList;
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value));
            identity.AddClaim(new Claim(ClaimTypes.Name, jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Name)?.Value));
            identity.AddClaim(new Claim(ClaimTypes.Role, jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value));



            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        private async Task SignOutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
