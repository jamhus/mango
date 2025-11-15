
namespace AuthAPI.Models.Dtos
{
    public class RegistrationRequestDto : LoginRequestDto
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = "Customer";
    }
}
