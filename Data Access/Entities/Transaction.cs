using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Transactions")]
    [Index(nameof(ReferenceNo), IsUnique = true)]
    public class Transaction
    {
        public long Id { get; set; }

        public long ReferenceNo { get; set; }

        public TransactionType Type { get; set; }

        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

        // Amount is expressed in the currency of CurrencyId
        [Column(TypeName = "decimal(19,4)")]
        public decimal Amount { get; set; }

        // Rate applied at the time of the transaction (1 when no conversion is needed)
        [Column(TypeName = "decimal(18,6)")]
        public decimal ExchangeRate { get; set; } = 1m;

        // Null when the account is not involved (e.g. no source for a Deposit)
        [Column(TypeName = "decimal(19,4)")]
        public decimal? SourceBalanceAfter { get; set; }

        [Column(TypeName = "decimal(19,4)")]
        public decimal? DestinationBalanceAfter { get; set; }

        // Only filled when Status = Failed
        [MaxLength(200)]
        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public long PerformedByUserId { get; set; }
        public long? SourceAccountId { get; set; }
        public long? DestinationAccountId { get; set; }
        public long CurrencyId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PerformedByUserId))]
        public User PerformedByUser { get; set; } = null!;

        [ForeignKey(nameof(SourceAccountId))]
        public Account? SourceAccount { get; set; }

        [ForeignKey(nameof(DestinationAccountId))]
        public Account? DestinationAccount { get; set; }

        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; } = null!;
    }
}
