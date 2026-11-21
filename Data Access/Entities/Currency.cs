using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Currencies")]
    [Index(nameof(Code), IsUnique = true)]
    public class Currency
    {
        public long Id { get; set; }

        // ISO code: USD, JOD, EUR ...
        [Required, StringLength(3, MinimumLength = 3)]
        [Column(TypeName = "char(3)")]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(5)]
        public string Symbol { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,6)")]
        public decimal RateToUsd { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
