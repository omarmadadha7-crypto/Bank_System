using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;

namespace Business_Logic.DTOs.Accounts
{
    public class ChangePinRequest
    {
        [Required]
        [RegularExpression(ValidationPatterns.Pin, ErrorMessage = ValidationPatterns.PinMessage)]
        public string CurrentPin { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ValidationPatterns.Pin, ErrorMessage = ValidationPatterns.PinMessage)]
        public string NewPin { get; set; } = string.Empty;
    }
}
