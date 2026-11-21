using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;

namespace Business_Logic.DTOs.Transactions
{
    public class TransferRequest
    {
        [Range(1, long.MaxValue)]
        public long SourceAccountId { get; set; }

        [Range(1, long.MaxValue)]
        public long DestinationAccountId { get; set; }

        // In the currency of the SOURCE account
        [Range(0.0001, 1000000000000)]
        public decimal Amount { get; set; }

        // PIN of the source account
        [Required]
        [RegularExpression(ValidationPatterns.Pin, ErrorMessage = ValidationPatterns.PinMessage)]
        public string Pin { get; set; } = string.Empty;
    }
}
