using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Users;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class UserService : IUserService
    {
        private readonly BankContext _context;
        private readonly IPersonService _personService;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(BankContext context, IPersonService personService, IPasswordHasher passwordHasher)
        {
            _context = context;
            _personService = personService;
            _passwordHasher = passwordHasher;
        }

        private IQueryable<User> UsersWithDetails =>
            _context.Users
                .Include(u => u.Person)
                .Include(u => u.Role)
                .Include(u => u.Branch);

        public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await UsersWithDetails
                .AsNoTracking()
                .OrderBy(u => u.UserName)
                .ToListAsync(cancellationToken);

            return users.Select(ToResponse).ToList();
        }

        public async Task<UserResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await UsersWithDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                ?? throw new NotFoundException($"User with id {id} was not found.");

            return ToResponse(user);
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var userName = request.UserName.Trim();

            if (await _context.Users.AnyAsync(u => u.UserName == userName, cancellationToken))
                throw new ConflictException($"User name '{userName}' is already taken.");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken)
                ?? throw new NotFoundException($"Role with id {request.RoleId} was not found.");

            var branch = await GetActiveBranchAsync(request.BranchId, cancellationToken);

            var person = await _personService.ResolveAsync(request.PersonId, request.Person, cancellationToken);

            if (person.Id != 0 && await _context.Users.AnyAsync(u => u.PersonId == person.Id, cancellationToken))
                throw new ConflictException("This person already has a user account.");

            var user = new User
            {
                UserName = userName,
                PasswordHash = _passwordHasher.Hash(request.Password),
                IsActive = true,
                Person = person,
                Role = role,
                Branch = branch
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(user);
        }

        public async Task<UserResponse> UpdateAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await FindTrackedAsync(id, cancellationToken);

            await _personService.UpdateAsync(user.Person, request.Person, cancellationToken);

            if (user.RoleId != request.RoleId)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken)
                    ?? throw new NotFoundException($"Role with id {request.RoleId} was not found.");

                user.Role = role;
            }

            if (user.BranchId != request.BranchId)
            {
                user.Branch = await GetActiveBranchAsync(request.BranchId, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(user);
        }

        public async Task<UserResponse> SetActiveAsync(long id, bool isActive, long currentUserId, CancellationToken cancellationToken = default)
        {
            if (!isActive && id == currentUserId)
                throw new BusinessRuleException("You cannot deactivate your own account.");

            var user = await FindTrackedAsync(id, cancellationToken);

            user.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(user);
        }

        public async Task<UserResponse> UnlockAsync(long id, CancellationToken cancellationToken = default)
        {
            var user = await FindTrackedAsync(id, cancellationToken);

            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(user);
        }

        // ---------- helpers ----------

        private async Task<User> FindTrackedAsync(long id, CancellationToken cancellationToken)
        {
            return await UsersWithDetails.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
                ?? throw new NotFoundException($"User with id {id} was not found.");
        }

        private async Task<Branch> GetActiveBranchAsync(long branchId, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken)
                ?? throw new NotFoundException($"Branch with id {branchId} was not found.");

            if (!branch.IsActive)
                throw new BusinessRuleException("The selected branch is not active.");

            return branch;
        }

        private UserResponse ToResponse(User user) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            IsActive = user.IsActive,
            LockoutEnd = user.LockoutEnd,
            RoleId = user.RoleId,
            RoleName = user.Role.Name,
            BranchId = user.BranchId,
            BranchName = user.Branch.Name,
            Person = _personService.ToResponse(user.Person)
        };
    }
}
