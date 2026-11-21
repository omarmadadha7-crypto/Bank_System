using System;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Auth;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class AuthService : IAuthService
    {
        private const int MaxFailedAttempts = 5;
        private const string InvalidCredentials = "Invalid username or password.";
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

        private readonly BankContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(BankContext context, IPasswordHasher passwordHasher, ITokenService tokenService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            var userName = request.UserName.Trim();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken)
                ?? throw new UnauthorizedException(InvalidCredentials);

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                throw new UnauthorizedException("This account is temporarily locked. Please try again later.");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                user.AccessFailedCount++;

                if (user.AccessFailedCount >= MaxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.Add(LockoutDuration);
                    user.AccessFailedCount = 0;
                }

                await _context.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedException(InvalidCredentials);
            }

            // checked after the password so a disabled account does not leak information to guessers
            if (!user.IsActive)
                throw new UnauthorizedException("This account is disabled.");

            user.AccessFailedCount = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync(cancellationToken);

            var (token, expiresAt) = _tokenService.CreateToken(user, user.Role.Name);

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserId = user.Id,
                UserName = user.UserName,
                Role = user.Role.Name,
                BranchId = user.BranchId
            };
        }

        public async Task ChangePasswordAsync(long userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
                ?? throw new NotFoundException($"User with id {userId} was not found.");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                throw new BusinessRuleException("The current password is incorrect.");

            if (request.CurrentPassword == request.NewPassword)
                throw new BusinessRuleException("The new password must be different from the current password.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
