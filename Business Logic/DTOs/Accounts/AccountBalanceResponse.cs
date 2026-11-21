using System;

namespace Business_Logic.DTOs.Accounts
{
    public class AccountBalanceResponse
    {
        public long AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public DateTime AsOf { get; set; }
    }
}
