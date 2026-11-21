using System;
using Data_Access.Enums;

namespace Business_Logic.DTOs.Transactions
{
    public class TransactionResponse
    {
        public long Id { get; set; }
        public long ReferenceNo { get; set; }
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }

        // In the currency of CurrencyId (the source account's currency for transfers)
        public decimal Amount { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;

        // Source currency -> destination currency (1 when no conversion is needed)
        public decimal ExchangeRate { get; set; }

        // Amount credited to the destination account (successful transfers only)
        public decimal? DestinationAmount { get; set; }

        public long? SourceAccountId { get; set; }
        public string? SourceAccountNumber { get; set; }
        public long? DestinationAccountId { get; set; }
        public string? DestinationAccountNumber { get; set; }

        public decimal? SourceBalanceAfter { get; set; }
        public decimal? DestinationBalanceAfter { get; set; }

        public string? FailureReason { get; set; }
        public long PerformedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
