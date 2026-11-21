using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Transactions
{
    public class DepositRequest
    {
        [Range(1, long.MaxValue)]
        public long AccountId { get; set; }

        // In the currency of the account
        [Range(0.0001, 1000000000000)]
        public decimal Amount { get; set; }
    }
}
