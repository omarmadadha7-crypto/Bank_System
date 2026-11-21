using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.Currencies;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly BankContext _context;

        public CurrencyService(BankContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CurrencyResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var currencies = await _context.Currencies
                .AsNoTracking()
                .OrderBy(c => c.Code)
                .ToListAsync(cancellationToken);

            return currencies.Select(ToResponse).ToList();
        }

        public async Task<CurrencyResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var currency = await _context.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Currency with id {id} was not found.");

            return ToResponse(currency);
        }

        public async Task<CurrencyResponse> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var normalized = NormalizeCode(code);

            var currency = await _context.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == normalized, cancellationToken)
                ?? throw new NotFoundException($"Currency with code '{normalized}' was not found.");

            return ToResponse(currency);
        }

        public async Task<CurrencyResponse> CreateAsync(CreateCurrencyRequest request, CancellationToken cancellationToken = default)
        {
            var code = NormalizeCode(request.Code);

            if (await _context.Currencies.AnyAsync(c => c.Code == code, cancellationToken))
                throw new ConflictException($"Currency with code '{code}' already exists.");

            var currency = new Currency
            {
                Code = code,
                Name = request.Name.Trim(),
                Symbol = request.Symbol.Trim(),
                RateToUsd = request.RateToUsd
            };

            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(currency);
        }

        public async Task<CurrencyResponse> UpdateAsync(long id, UpdateCurrencyRequest request, CancellationToken cancellationToken = default)
        {
            var currency = await FindTrackedAsync(id, cancellationToken);

            currency.Name = request.Name.Trim();
            currency.Symbol = request.Symbol.Trim();

            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(currency);
        }

        public async Task<CurrencyResponse> UpdateRateAsync(long id, UpdateRateRequest request, CancellationToken cancellationToken = default)
        {
            var currency = await FindTrackedAsync(id, cancellationToken);

            currency.RateToUsd = request.RateToUsd;
            currency.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(currency);
        }

        public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var currency = await FindTrackedAsync(id, cancellationToken);

            var inUse = await _context.Accounts.AnyAsync(a => a.CurrencyId == id, cancellationToken)
                     || await _context.Transactions.AnyAsync(t => t.CurrencyId == id, cancellationToken);

            if (inUse)
                throw new ConflictException("This currency is used by existing accounts or transactions and cannot be deleted.");

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync(cancellationToken);
        }

        // ---------- helpers ----------

        private async Task<Currency> FindTrackedAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Currencies.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Currency with id {id} was not found.");
        }

        private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();

        private static CurrencyResponse ToResponse(Currency c) => new()
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Symbol = c.Symbol,
            RateToUsd = c.RateToUsd,
            UpdatedAt = c.UpdatedAt
        };
    }
}
