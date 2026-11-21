using System.ComponentModel.DataAnnotations;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Users
{
    // UserName and password are not changed here
    public class UpdateUserRequest
    {
        [Required]
        public PersonRequest Person { get; set; } = null!;

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long BranchId { get; set; }
    }
}
