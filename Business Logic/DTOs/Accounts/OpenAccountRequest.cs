using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;
using Data_Access.Enums;

namespace Business_Logic.DTOs.Accounts
{
    public class OpenAccountRequest
    {
        [Range(1, long.MaxValue)]
        public long CustomerId { get; set; }

        [Range(1, long.MaxValue)]
        public long CurrencyId { get; set; }

        [EnumDataType(typeof(AccountType))]
        public AccountType Type { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.Pin, ErrorMessage = ValidationPatterns.PinMessage)]
        public string Pin { get; set; } = string.Empty;
    }
}
