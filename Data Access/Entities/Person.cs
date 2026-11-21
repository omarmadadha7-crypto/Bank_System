using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.Entities
{
    [Table("People")]
    [Index(nameof(NationalId), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    public class Person
    {
        public long Id { get; set; }

        // string, not long: keeps leading zeros
        [Required, MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string NationalId { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string LastName { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(60), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(30), Phone]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        // One-to-one navigation properties (a person may be a user, a customer, both, or neither)
        public User? User { get; set; }
        public Customer? Customer { get; set; }
    }
}
