using System.ComponentModel.DataAnnotations;
using Business_Logic.Common;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Users
{
    public class CreateUserRequest
    {
        // Provide EITHER PersonId (an existing person) OR Person (data of a new person), not both.
        public long? PersonId { get; set; }
        public PersonRequest? Person { get; set; }

        [Required]
        [RegularExpression(ValidationPatterns.UserName, ErrorMessage = ValidationPatterns.UserNameMessage)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(ValidationPatterns.Password, ErrorMessage = ValidationPatterns.PasswordMessage)]
        public string Password { get; set; } = string.Empty;

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long BranchId { get; set; }
    }
}
