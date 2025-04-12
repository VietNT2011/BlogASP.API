using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlogASP.API.DTOs
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = null!;
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
