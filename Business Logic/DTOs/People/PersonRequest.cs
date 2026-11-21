using System;
using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.People
{
    public class PersonRequest
    {
        [Required, MaxLength(20)]
        public string NationalId { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string LastName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [Required, MaxLength(60), EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(30), Phone]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }
    }
}
