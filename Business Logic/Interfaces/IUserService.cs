using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Users;

namespace Business_Logic.Interfaces
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<UserResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

        Task<UserResponse> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default);

        // currentUserId = the admin performing the action (an admin cannot deactivate himself)
        Task<UserResponse> SetActiveAsync(long id, bool isActive, long currentUserId, CancellationToken cancellationToken = default);

        // Clears the failed-attempts counter and the lockout
        Task<UserResponse> UnlockAsync(long id, CancellationToken cancellationToken = default);
    }
}
