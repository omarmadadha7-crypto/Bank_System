using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Auth
{
    public class LoginRequest
    {
        [Required, MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }
}
