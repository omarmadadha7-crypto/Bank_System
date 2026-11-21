using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Branches;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class BranchService : IBranchService
    {
        private readonly BankContext _context;

        public BranchService(BankContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<BranchResponse>> GetAllAsync(bool? isActive, CancellationToken cancellationToken = default)
        {
            var query = _context.Branches.AsNoTracking();

            if (isActive.HasValue)
                query = query.Where(b => b.IsActive == isActive.Value);

            var branches = await query.OrderBy(b => b.Code).ToListAsync(cancellationToken);

            return branches.Select(ToResponse).ToList();
        }

        public async Task<BranchResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var branch = await _context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Branch with id {id} was not found.");

            return ToResponse(branch);
        }

        public async Task<BranchResponse> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default)
        {
            if (await _context.Branches.AnyAsync(b => b.Code == request.Code, cancellationToken))
                throw new ConflictException($"A branch with code {request.Code} already exists.");

            var branch = new Branch
            {
                Code = request.Code,
                Name = request.Name.Trim(),
                City = request.City.Trim(),
                IsActive = true
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(branch);
        }

        public async Task<BranchResponse> UpdateAsync(long id, UpdateBranchRequest request, CancellationToken cancellationToken = default)
        {
            var branch = await FindTrackedAsync(id, cancellationToken);

            branch.Name = request.Name.Trim();
            branch.City = request.City.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(branch);
        }

        public async Task<BranchResponse> SetActiveAsync(long id, bool isActive, CancellationToken cancellationToken = default)
        {
            var branch = await FindTrackedAsync(id, cancellationToken);

            branch.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(branch);
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var branch = await FindTrackedAsync(id, cancellationToken);

            var inUse = await _context.Users.AnyAsync(u => u.BranchId == id, cancellationToken)
                     || await _context.Customers.AnyAsync(c => c.BranchId == id, cancellationToken);

            if (inUse)
                throw new ConflictException("This branch has users or customers and cannot be deleted. Deactivate it instead.");

            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ---------- helpers ----------

        private async Task<Branch> FindTrackedAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Branches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Branch with id {id} was not found.");
        }

        private static BranchResponse ToResponse(Branch b) => new()
        {
            Id = b.Id,
            Code = b.Code,
            Name = b.Name,
            City = b.City,
            IsActive = b.IsActive
        };
    }
}
