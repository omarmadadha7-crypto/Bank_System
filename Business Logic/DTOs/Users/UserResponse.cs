using System;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Users
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public long BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public PersonResponse Person { get; set; } = null!;
    }
}
