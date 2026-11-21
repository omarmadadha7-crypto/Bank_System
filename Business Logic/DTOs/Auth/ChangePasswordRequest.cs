using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;

namespace Business_Logic.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required, MaxLength(100)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ValidationPatterns.Password, ErrorMessage = ValidationPatterns.PasswordMessage)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
