using System.ComponentModel.DataAnnotations;
using Business_Logic.DTOs.People;

namespace Business_Logic.DTOs.Customers
{
    public class CreateCustomerRequest
    {
        // Provide EITHER PersonId (an existing person) OR Person (data of a new person), not both.
        public long? PersonId { get; set; }
        public PersonRequest? Person { get; set; }

        [Range(1, long.MaxValue)]
        public long BranchId { get; set; }
    }
}
