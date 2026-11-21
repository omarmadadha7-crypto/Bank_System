using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Accounts;

namespace Business_Logic.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResponse> OpenAsync(OpenAccountRequest request, long openedByUserId, CancellationToken cancellationToken = default);

        Task<AccountResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<AccountResponse> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AccountResponse>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default);

        Task<AccountBalanceResponse> GetBalanceAsync(long id, CancellationToken cancellationToken = default);

        Task<AccountResponse> FreezeAsync(long id, CancellationToken cancellationToken = default);

        Task<AccountResponse> UnfreezeAsync(long id, CancellationToken cancellationToken = default);

        // Only possible when the balance is zero
        Task<AccountResponse> CloseAsync(long id, CancellationToken cancellationToken = default);

        Task ChangePinAsync(long id, ChangePinRequest request, CancellationToken cancellationToken = default);
    }
}
