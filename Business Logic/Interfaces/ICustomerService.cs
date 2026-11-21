using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Customers;

namespace Business_Logic.Interfaces
{
    public interface ICustomerService
    {
        // search matches national id, name, phone, email or customer number
        Task<PagedResult<CustomerResponse>> SearchAsync(
            string? search, long? branchId, bool? isActive, int page, int pageSize,
            CancellationToken cancellationToken = default);

        Task<CustomerResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        // Creates the Person too (or uses an existing one) and the Customer, atomically
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);

        Task<CustomerResponse> UpdateAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);

        Task<CustomerResponse> SetActiveAsync(long id, bool isActive, CancellationToken cancellationToken = default);
    }
}
