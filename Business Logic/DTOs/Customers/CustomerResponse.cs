using System;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Customers
{
    public class CustomerResponse
    {
        public long Id { get; set; }
        public long CustomerNumber { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public PersonResponse Person { get; set; } = null!;
    }
}
