using System.ComponentModel.DataAnnotations;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Customers
{
    public class UpdateCustomerRequest
    {
        [Required]
        public PersonRequest Person { get; set; } = null!;

        [Range(1, long.MaxValue)]
        public long BranchId { get; set; }
    }
}
