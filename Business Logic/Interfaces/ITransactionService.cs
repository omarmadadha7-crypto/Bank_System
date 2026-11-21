using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Transactions;

namespace Business_Logic.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse> DepositAsync(DepositRequest request, long performedByUserId, CancellationToken cancellationToken = default);

        Task<TransactionResponse> WithdrawAsync(WithdrawRequest request, long performedByUserId, CancellationToken cancellationToken = default);

        Task<TransactionResponse> TransferAsync(TransferRequest request, long performedByUserId, CancellationToken cancellationToken = default);

        Task<TransactionResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<TransactionResponse> GetByReferenceAsync(long referenceNo, CancellationToken cancellationToken = default);

        // Statement of one account (as source or destination), newest first
        Task<PagedResult<TransactionResponse>> GetByAccountAsync(long accountId, TransactionFilter filter, CancellationToken cancellationToken = default);
    }
}
