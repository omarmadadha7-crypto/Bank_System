using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Accounts")]
    [Index(nameof(AccountNumber), IsUnique = true)]
    public class Account
    {
        public long Id { get; set; }

        // varchar(34) so it can hold an IBAN
        [Required, MaxLength(34)]
        [Column(TypeName = "varchar(34)")]
        public string AccountNumber { get; set; } = string.Empty;

        public AccountType Type { get; set; }

        [Column(TypeName = "decimal(19,4)")]
        public decimal Balance { get; set; }

        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [Required, MaxLength(255)]
        public string PinHash { get; set; } = string.Empty;

        // Optimistic concurrency: prevents two requests from overwriting the same balance
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public long CustomerId { get; set; }
        public long OpenedByUserId { get; set; }
        public long CurrencyId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(OpenedByUserId))]
        public User OpenedByUser { get; set; } = null!;

        [ForeignKey(nameof(CurrencyId))]
        public Currency Currency { get; set; } = null!;

        // Two FKs point to Account from Transaction, so InverseProperty tells EF which is which
        [InverseProperty(nameof(Transaction.SourceAccount))]
        public ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();

        [InverseProperty(nameof(Transaction.DestinationAccount))]
        public ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
    }
}
