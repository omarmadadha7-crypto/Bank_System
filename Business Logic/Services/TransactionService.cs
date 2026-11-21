using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Transactions;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class TransactionService : ITransactionService
    {
        private const int MaxPageSize = 100;
        private const int AmountScale = 4;   // matches DECIMAL(19,4)
        private const int RateScale = 6;     // matches DECIMAL(18,6)

        private readonly BankContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public TransactionService(BankContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private IQueryable<Transaction> TransactionsWithDetails =>
            _context.Transactions
                .AsNoTracking()
                .Include(t => t.Currency)
                .Include(t => t.SourceAccount)
                .Include(t => t.DestinationAccount);

        // =============================================================
        // Deposit: money enters the destination account
        // =============================================================
        public async Task<TransactionResponse> DepositAsync(DepositRequest request, long performedByUserId, CancellationToken cancellationToken = default)
        {
            var amount = ValidateAmount(request.Amount);
            var account = await LoadAccountAsync(request.AccountId, cancellationToken);

            string? failure = null;
            if (account.Status != AccountStatus.Active)
                failure = $"The account is {account.Status}.";

            if (failure is not null)
                throw await RecordFailureAsync(TransactionType.Deposit, performedByUserId, null, account, account.Currency, amount, 1m, failure, cancellationToken);

            account.Balance += amount;

            var transaction = new Transaction
            {
                ReferenceNo = await NextReferenceNoAsync(cancellationToken),
                Type = TransactionType.Deposit,
                Status = TransactionStatus.Success,
                Amount = amount,
                ExchangeRate = 1m,
                DestinationAccount = account,
                DestinationBalanceAfter = account.Balance,
                Currency = account.Currency,
                PerformedByUserId = performedByUserId
            };

            // the balance change and the transaction row are saved atomically
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(transaction);
        }

        // =============================================================
        // Withdraw: money leaves the source account (PIN required)
        // =============================================================
        public async Task<TransactionResponse> WithdrawAsync(WithdrawRequest request, long performedByUserId, CancellationToken cancellationToken = default)
        {
            var amount = ValidateAmount(request.Amount);
            var account = await LoadAccountAsync(request.AccountId, cancellationToken);

            string? failure = null;
            if (account.Status != AccountStatus.Active)
                failure = $"The account is {account.Status}.";
            else if (!_passwordHasher.Verify(request.Pin, account.PinHash))
                failure = "Invalid PIN.";
            else if (account.Balance < amount)
                failure = "Insufficient funds.";

            if (failure is not null)
                throw await RecordFailureAsync(TransactionType.Withdraw, performedByUserId, account, null, account.Currency, amount, 1m, failure, cancellationToken);

            account.Balance -= amount;

            var transaction = new Transaction
            {
                ReferenceNo = await NextReferenceNoAsync(cancellationToken),
                Type = TransactionType.Withdraw,
                Status = TransactionStatus.Success,
                Amount = amount,
                ExchangeRate = 1m,
                SourceAccount = account,
                SourceBalanceAfter = account.Balance,
                Currency = account.Currency,
                PerformedByUserId = performedByUserId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(transaction);
        }

        // =============================================================
        // Transfer: Amount is in the SOURCE currency, converted for the destination
        // =============================================================
        public async Task<TransactionResponse> TransferAsync(TransferRequest request, long performedByUserId, CancellationToken cancellationToken = default)
        {
            if (request.SourceAccountId == request.DestinationAccountId)
                throw new BusinessRuleException("The source and destination accounts must be different.");

            var amount = ValidateAmount(request.Amount);

            var accounts = await _context.Accounts
                .Include(a => a.Currency)
                .Where(a => a.Id == request.SourceAccountId || a.Id == request.DestinationAccountId)
                .ToListAsync(cancellationToken);

            var source = accounts.FirstOrDefault(a => a.Id == request.SourceAccountId)
                ?? throw new NotFoundException($"Account with id {request.SourceAccountId} was not found.");

            var destination = accounts.FirstOrDefault(a => a.Id == request.DestinationAccountId)
                ?? throw new NotFoundException($"Account with id {request.DestinationAccountId} was not found.");

            if (source.Currency.RateToUsd <= 0 || destination.Currency.RateToUsd <= 0)
                throw new BusinessRuleException("An exchange rate is not configured for one of the account currencies.");

            // RateToUsd = value of ONE unit in USD  =>  1 source unit = (srcRate / dstRate) destination units
            var rate = source.CurrencyId == destination.CurrencyId
                ? 1m
                : decimal.Round(source.Currency.RateToUsd / destination.Currency.RateToUsd, RateScale);

            var credited = decimal.Round(amount * rate, AmountScale, MidpointRounding.AwayFromZero);

            string? failure = null;
            if (source.Status != AccountStatus.Active)
                failure = $"The source account is {source.Status}.";
            else if (destination.Status != AccountStatus.Active)
                failure = $"The destination account is {destination.Status}.";
            else if (!_passwordHasher.Verify(request.Pin, source.PinHash))
                failure = "Invalid PIN.";
            else if (source.Balance < amount)
                failure = "Insufficient funds.";
            else if (credited <= 0)
                failure = "The amount is too small after currency conversion.";

            if (failure is not null)
                throw await RecordFailureAsync(TransactionType.Transfer, performedByUserId, source, destination, source.Currency, amount, rate, failure, cancellationToken);

            source.Balance -= amount;
            destination.Balance += credited;

            var transaction = new Transaction
            {
                ReferenceNo = await NextReferenceNoAsync(cancellationToken),
                Type = TransactionType.Transfer,
                Status = TransactionStatus.Success,
                Amount = amount,
                ExchangeRate = rate,
                SourceAccount = source,
                DestinationAccount = destination,
                SourceBalanceAfter = source.Balance,
                DestinationBalanceAfter = destination.Balance,
                Currency = source.Currency,
                PerformedByUserId = performedByUserId
            };

            // both balances + the transaction row: one atomic SaveChanges.
            // If another request changed one of the accounts meanwhile, the RowVersion check
            // throws DbUpdateConcurrencyException (the API answers 409) and nothing is saved.
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(transaction);
        }

        // =============================================================
        // Queries
        // =============================================================
        public async Task<TransactionResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var transaction = await TransactionsWithDetails.FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Transaction with id {id} was not found.");

            return ToResponse(transaction);
        }

        public async Task<TransactionResponse> GetByReferenceAsync(long referenceNo, CancellationToken cancellationToken = default)
        {
            var transaction = await TransactionsWithDetails.FirstOrDefaultAsync(t => t.ReferenceNo == referenceNo, cancellationToken)
                ?? throw new NotFoundException($"Transaction with reference {referenceNo} was not found.");

            return ToResponse(transaction);
        }

        public async Task<PagedResult<TransactionResponse>> GetByAccountAsync(long accountId, TransactionFilter filter, CancellationToken cancellationToken = default)
        {
            if (!await _context.Accounts.AnyAsync(a => a.Id == accountId, cancellationToken))
                throw new NotFoundException($"Account with id {accountId} was not found.");

            var page = Math.Max(1, filter.Page);
            var pageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);

            IQueryable<Transaction> query = TransactionsWithDetails
                .Where(t => t.SourceAccountId == accountId || t.DestinationAccountId == accountId);

            if (filter.From.HasValue)
                query = query.Where(t => t.CreatedAt >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(t => t.CreatedAt <= filter.To.Value);

            if (filter.Type.HasValue)
                query = query.Where(t => t.Type == filter.Type.Value);

            if (filter.Status.HasValue)
                query = query.Where(t => t.Status == filter.Status.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var transactions = await query
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TransactionResponse>
            {
                Items = transactions.Select(ToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // =============================================================
        // helpers
        // =============================================================
        private static decimal ValidateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new BusinessRuleException("The amount must be greater than zero.");

            if (decimal.Round(amount, AmountScale) != amount)
                throw new BusinessRuleException($"The amount cannot have more than {AmountScale} decimal places.");

            return amount;
        }

        // tracked (we change the balance), with its currency
        private async Task<Account> LoadAccountAsync(long accountId, CancellationToken cancellationToken)
        {
            return await _context.Accounts
                .Include(a => a.Currency)
                .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken)
                ?? throw new NotFoundException($"Account with id {accountId} was not found.");
        }

        // Saves a FAILED transaction (audit trail) and returns the exception to throw.
        // Called before any balance is modified, so only the failed row is saved.
        private async Task<BusinessRuleException> RecordFailureAsync(
            TransactionType type, long performedByUserId, Account? source, Account? destination,
            Currency currency, decimal amount, decimal exchangeRate, string reason,
            CancellationToken cancellationToken)
        {
            var transaction = new Transaction
            {
                ReferenceNo = await NextReferenceNoAsync(cancellationToken),
                Type = type,
                Status = TransactionStatus.Failed,
                Amount = amount,
                ExchangeRate = exchangeRate,
                SourceAccount = source,
                DestinationAccount = destination,
                Currency = currency,
                PerformedByUserId = performedByUserId,
                FailureReason = reason.Length > 200 ? reason[..200] : reason
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync(cancellationToken);

            return new BusinessRuleException(reason);
        }

        // 13 digits, random, checked for uniqueness (the unique index is the final safety net)
        private async Task<long> NextReferenceNoAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var referenceNo = Random.Shared.NextInt64(1_000_000_000_000L, 10_000_000_000_000L);

                if (!await _context.Transactions.AnyAsync(t => t.ReferenceNo == referenceNo, cancellationToken))
                    return referenceNo;
            }

            throw new BusinessRuleException("Could not generate a unique reference number. Please try again.");
        }

        private static TransactionResponse ToResponse(Transaction t) => new()
        {
            Id = t.Id,
            ReferenceNo = t.ReferenceNo,
            Type = t.Type,
            Status = t.Status,
            Amount = t.Amount,
            CurrencyId = t.CurrencyId,
            CurrencyCode = t.Currency.Code,
            ExchangeRate = t.ExchangeRate,
            DestinationAmount = t.Type == TransactionType.Transfer && t.Status == TransactionStatus.Success
                ? decimal.Round(t.Amount * t.ExchangeRate, AmountScale, MidpointRounding.AwayFromZero)
                : null,
            SourceAccountId = t.SourceAccountId,
            SourceAccountNumber = t.SourceAccount?.AccountNumber,
            DestinationAccountId = t.DestinationAccountId,
            DestinationAccountNumber = t.DestinationAccount?.AccountNumber,
            SourceBalanceAfter = t.SourceBalanceAfter,
            DestinationBalanceAfter = t.DestinationBalanceAfter,
            FailureReason = t.FailureReason,
            PerformedByUserId = t.PerformedByUserId,
            CreatedAt = t.CreatedAt
        };
    }
}
