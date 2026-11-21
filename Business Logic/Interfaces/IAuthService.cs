using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Auth;

namespace Business_Logic.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

        Task ChangePasswordAsync(long userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    }
}
