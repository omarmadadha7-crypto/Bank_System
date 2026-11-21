using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Branches;

namespace Business_Logic.Interfaces
{
    public interface IBranchService
    {
        // isActive = null returns all branches
        Task<IReadOnlyList<BranchResponse>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default);

        Task<BranchResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);

        Task<BranchResponse> UpdateAsync(long id, UpdateBranchRequest request, CancellationToken cancellationToken = default);

        Task<BranchResponse> SetActiveAsync(long id, bool isActive, CancellationToken cancellationToken = default);

        Task DeleteAsync(long id, CancellationToken cancellationToken = default);
    }
}
