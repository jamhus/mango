
using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Models
{
    public class RegistrationRequestDto : LoginRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = "Customer";
    }
}
