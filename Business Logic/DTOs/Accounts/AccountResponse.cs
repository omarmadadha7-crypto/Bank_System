using System;
using Data_Access.Enums;

namespace Business_Logic.DTOs.Accounts
{
    public class AccountResponse
    {
        public long Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public DateTime OpenedAt { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public long OpenedByUserId { get; set; }
    }
}
