using AuthAPI.Data;
using AuthAPI.Models;
using AuthAPI.Models.Dtos;
using AuthAPI.Services.Interfaces;
using Mango.MessageBus;
using Mango.Services.AuthAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext db,
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IJwtTokenGenerator tokenGenerator,
            IMessageBus messageBus,
            IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenGenerator = tokenGenerator;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        public async Task<bool> AssignRoleAsync(string email, string role)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                var roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
                await _userManager.AddToRoleAsync(user, role);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var User = _db.Users.FirstOrDefault(u => u.Email.ToLower() == loginRequestDto.Email.ToLower());

            bool isvalid = await _userManager.CheckPasswordAsync(User, loginRequestDto.Password);

            if (User == null || !isvalid)
            {
                return new LoginResponseDto
                {
                    User = null,
                    Token = "",

                };
            }
            var roles = await _userManager.GetRolesAsync(User);
            var response = new LoginResponseDto
            {
                User = new UserDto
                {
                    Id = User.Id,
                    Name = User.Name,
                    Email = User.Email,
                    PhoneNumber = User.PhoneNumber,
                },
                Token = _tokenGenerator.CreateToken(User, roles),
            };


            return response;
        }

        public async Task<string> RegisterAsync(RegistrationRequestDto model)
        {
            ApplicationUser user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                NormalizedEmail = model.Email.ToUpper(),

            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.Users.FirstOrDefault(u => u.Email == model.Email);
                    var roleExist = await _roleManager.RoleExistsAsync(model.Role);
                    if (!roleExist)
                    {
                        await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    }
                    await _userManager.AddToRoleAsync(userToReturn, model.Role);
                    _messageBus.PublishMessage(userToReturn.Email, _configuration.GetValue<string>("TopicAndQueueNames:EmailUserRegisteredQueue"));
                    return string.Empty;

                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
    }
}
