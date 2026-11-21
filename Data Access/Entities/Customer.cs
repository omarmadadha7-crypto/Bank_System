using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("Customers")]
    [Index(nameof(PersonId), IsUnique = true)]
    [Index(nameof(CustomerNumber), IsUnique = true)]
    public class Customer
    {
        public long Id { get; set; }

        public long CustomerNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public long PersonId { get; set; }
        public long BranchId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; } = null!;

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; } = null!;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
