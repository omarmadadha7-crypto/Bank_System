using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;

namespace Business_Logic.DTOs.Transactions
{
    public class WithdrawRequest
    {
        [Range(1, long.MaxValue)]
        public long AccountId { get; set; }

        // In the currency of the account
        [Range(0.0001, 1000000000000)]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.Pin, ErrorMessage = ValidationPatterns.PinMessage)]
        public string Pin { get; set; } = string.Empty;
    }
}
