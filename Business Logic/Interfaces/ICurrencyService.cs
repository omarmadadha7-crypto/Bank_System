using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Currencies;

namespace Business_Logic.Interfaces
{
    public interface ICurrencyService
    {
        Task<IReadOnlyList<CurrencyResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<CurrencyResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<CurrencyResponse> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<CurrencyResponse> CreateAsync(CreateCurrencyRequest request, CancellationToken cancellationToken = default);

        Task<CurrencyResponse> UpdateAsync(long id, UpdateCurrencyRequest request, CancellationToken cancellationToken = default);

        Task<CurrencyResponse> UpdateRateAsync(long id, UpdateRateRequest request, CancellationToken cancellationToken = default);

        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
}
