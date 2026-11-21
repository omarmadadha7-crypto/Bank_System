using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Users")]
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(PersonId), IsUnique = true)]
    public class User
    {
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public int AccessFailedCount { get; set; }

        // null = the user is not locked out
        public DateTime? LockoutEnd { get; set; }

        // Foreign keys
        public long PersonId { get; set; }
        public long RoleId { get; set; }
        public long BranchId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; } = null!;

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; } = null!;

        public ICollection<Account> OpenedAccounts { get; set; } = new List<Account>();
        public ICollection<Transaction> PerformedTransactions { get; set; } = new List<Transaction>();
    }
}
