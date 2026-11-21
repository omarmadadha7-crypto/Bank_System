using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Roles;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class RoleService : IRoleService
    {
        private readonly BankContext _context;

        public RoleService(BankContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RoleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .Select(r => new RoleResponse { Id = r.Id, Name = r.Name })
                .ToListAsync(cancellationToken);
        }

        public async Task<RoleResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoleResponse { Id = r.Id, Name = r.Name })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Role with id {id} was not found.");
        }
    }
}
