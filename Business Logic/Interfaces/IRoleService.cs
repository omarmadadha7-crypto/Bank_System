using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Roles;

namespace Business_Logic.Interfaces
{
    public interface IRoleService
    {
        Task<IReadOnlyList<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<RoleResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    }
}
