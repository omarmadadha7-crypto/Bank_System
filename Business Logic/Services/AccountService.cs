using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Accounts;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AccountService(BankContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private IQueryable<Account> AccountsWithDetails =>
            _context.Accounts
                .Include(a => a.Customer).ThenInclude(c => c.Person)
                .Include(a => a.Currency);

        public async Task<AccountResponse> OpenAsync(OpenAccountRequest request, long openedByUserId, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers
                .Include(c => c.Person)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken)
                ?? throw new NotFoundException($"Customer with id {request.CustomerId} was not found.");

            if (!customer.IsActive)
                throw new BusinessRuleException("Cannot open an account for an inactive customer.");

            var currency = await _context.Currencies.FirstOrDefaultAsync(c => c.Id == request.CurrencyId, cancellationToken)
                ?? throw new NotFoundException($"Currency with id {request.CurrencyId} was not found.");

            var account = new Account
            {
                AccountNumber = await GenerateAccountNumberAsync(customer.Branch.Code, cancellationToken),
                Type = request.Type,
                Balance = 0m,
                Status = AccountStatus.Active,
                PinHash = _passwordHasher.Hash(request.Pin),
                OpenedAt = DateTime.UtcNow,
                OpenedByUserId = openedByUserId,
                Customer = customer,
                Currency = currency
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(account);
        }

        public async Task<AccountResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var account = await AccountsWithDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Account with id {id} was not found.");

            return ToResponse(account);
        }

        public async Task<AccountResponse> GetByNumberAsync(string accountNumber, CancellationToken cancellationToken = default)
        {
            var number = accountNumber.Trim();

            var account = await AccountsWithDetails
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountNumber == number, cancellationToken)
                ?? throw new NotFoundException($"Account '{number}' was not found.");

            return ToResponse(account);
        }

        public async Task<IReadOnlyList<AccountResponse>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default)
        {
            if (!await _context.Customers.AnyAsync(c => c.Id == customerId, cancellationToken))
                throw new NotFoundException($"Customer with id {customerId} was not found.");

            var accounts = await AccountsWithDetails
                .AsNoTracking()
                .Where(a => a.CustomerId == customerId)
                .OrderBy(a => a.Id)
                .ToListAsync(cancellationToken);

            return accounts.Select(ToResponse).ToList();
        }

        public async Task<AccountBalanceResponse> GetBalanceAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _context.Accounts
                .AsNoTracking()
                .Where(a => a.Id == id)
                .Select(a => new AccountBalanceResponse
                {
                    AccountId = a.Id,
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance,
                    CurrencyCode = a.Currency.Code,
                    AsOf = DateTime.UtcNow
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException($"Account with id {id} was not found.");
        }

        public Task<AccountResponse> FreezeAsync(long id, CancellationToken cancellationToken = default)
            => ChangeStatusAsync(id, AccountStatus.Frozen, cancellationToken);

        public Task<AccountResponse> UnfreezeAsync(long id, CancellationToken cancellationToken = default)
            => ChangeStatusAsync(id, AccountStatus.Active, cancellationToken);

        public Task<AccountResponse> CloseAsync(long id, CancellationToken cancellationToken = default)
            => ChangeStatusAsync(id, AccountStatus.Closed, cancellationToken);

        public async Task ChangePinAsync(long id, ChangePinRequest request, CancellationToken cancellationToken = default)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Account with id {id} was not found.");

            if (account.Status == AccountStatus.Closed)
                throw new BusinessRuleException("The account is closed.");

            if (!_passwordHasher.Verify(request.CurrentPin, account.PinHash))
                throw new BusinessRuleException("The current PIN is incorrect.");

            if (request.CurrentPin == request.NewPin)
                throw new BusinessRuleException("The new PIN must be different from the current PIN.");

            account.PinHash = _passwordHasher.Hash(request.NewPin);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ---------- helpers ----------

        private async Task<AccountResponse> ChangeStatusAsync(long id, AccountStatus target, CancellationToken cancellationToken)
        {
            // tracked: the RowVersion concurrency token protects against two simultaneous changes
            var account = await AccountsWithDetails.FirstOrDefaultAsync(a => a.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Account with id {id} was not found.");

            EnsureTransitionAllowed(account, target);

            account.Status = target;
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(account);
        }

        private static void EnsureTransitionAllowed(Account account, AccountStatus target)
        {
            var allowed = (account.Status, target) switch
            {
                (AccountStatus.Active, AccountStatus.Frozen) => true,
                (AccountStatus.Frozen, AccountStatus.Active) => true,
                (AccountStatus.Active, AccountStatus.Closed) => true,
                (AccountStatus.Frozen, AccountStatus.Closed) => true,
                _ => false
            };

            if (!allowed)
                throw new BusinessRuleException($"An account cannot change from {account.Status} to {target}.");

            if (target == AccountStatus.Closed && account.Balance != 0m)
                throw new BusinessRuleException("An account can only be closed when its balance is zero.");
        }

        // 16 digits: 3 digits of the branch code + 13 random digits (checked for uniqueness)
        private async Task<string> GenerateAccountNumberAsync(int branchCode, CancellationToken cancellationToken)
        {
            var prefix = (branchCode % 1000).ToString("D3");

            for (var attempt = 0; attempt < 10; attempt++)
            {
                var number = prefix + Random.Shared.NextInt64(1_000_000_000_000L, 10_000_000_000_000L);

                if (!await _context.Accounts.AnyAsync(a => a.AccountNumber == number, cancellationToken))
                    return number;
            }

            throw new BusinessRuleException("Could not generate a unique account number. Please try again.");
        }

        private static AccountResponse ToResponse(Account a) => new()
        {
            Id = a.Id,
            AccountNumber = a.AccountNumber,
            Type = a.Type,
            Balance = a.Balance,
            Status = a.Status,
            OpenedAt = a.OpenedAt,
            CustomerId = a.CustomerId,
            CustomerName = $"{a.Customer.Person.FirstName} {a.Customer.Person.LastName}",
            CurrencyId = a.CurrencyId,
            CurrencyCode = a.Currency.Code,
            OpenedByUserId = a.OpenedByUserId
        };
    }
}
