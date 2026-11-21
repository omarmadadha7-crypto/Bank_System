using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.DTOs.Customers;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class CustomerService : ICustomerService
    {
        private const int MaxPageSize = 100;

        private readonly BankContext _context;
        private readonly IPersonService _personService;

        public CustomerService(BankContext context, IPersonService personService)
        {
            _context = context;
            _personService = personService;
        }

        public async Task<PagedResult<CustomerResponse>> SearchAsync(
            string? search, long? branchId, bool? isActive, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            IQueryable<Customer> query = _context.Customers.AsNoTracking();

            if (branchId.HasValue)
                query = query.Where(c => c.BranchId == branchId.Value);

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();

                query = query.Where(c =>
                    c.Person.NationalId.Contains(term) ||
                    c.Person.FirstName.Contains(term) ||
                    c.Person.LastName.Contains(term) ||
                    c.Person.Phone.Contains(term) ||
                    c.Person.Email.Contains(term) ||
                    c.CustomerNumber.ToString().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var customers = await query
                .Include(c => c.Person)
                .Include(c => c.Branch)
                .OrderByDescending(c => c.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<CustomerResponse>
            {
                Items = customers.Select(ToResponse).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<CustomerResponse> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .Include(c => c.Person)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Customer with id {id} was not found.");

            return ToResponse(customer);
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var branch = await GetActiveBranchAsync(request.BranchId, cancellationToken);

            var person = await _personService.ResolveAsync(request.PersonId, request.Person, cancellationToken);

            if (person.Id != 0 && await _context.Customers.AnyAsync(c => c.PersonId == person.Id, cancellationToken))
                throw new ConflictException("This person is already a customer.");

            var customer = new Customer
            {
                CustomerNumber = await GenerateCustomerNumberAsync(cancellationToken),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Person = person,
                Branch = branch
            };

            // Person (if new) and Customer are saved together in one SaveChanges
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(customer);
        }

        public async Task<CustomerResponse> UpdateAsync(long id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
        {
            var customer = await FindTrackedAsync(id, cancellationToken);

            await _personService.UpdateAsync(customer.Person, request.Person, cancellationToken);

            if (customer.BranchId != request.BranchId)
            {
                customer.Branch = await GetActiveBranchAsync(request.BranchId, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(customer);
        }

        public async Task<CustomerResponse> SetActiveAsync(long id, bool isActive, CancellationToken cancellationToken = default)
        {
            var customer = await FindTrackedAsync(id, cancellationToken);

            if (!isActive)
            {
                var hasOpenAccounts = await _context.Accounts
                    .AnyAsync(a => a.CustomerId == id && a.Status != AccountStatus.Closed, cancellationToken);

                if (hasOpenAccounts)
                    throw new BusinessRuleException("Close all accounts of this customer before deactivating the customer.");
            }

            customer.IsActive = isActive;
            await _context.SaveChangesAsync(cancellationToken);

            return ToResponse(customer);
        }

        // ---------- helpers ----------

        private async Task<Customer> FindTrackedAsync(long id, CancellationToken cancellationToken)
        {
            return await _context.Customers
                .Include(c => c.Person)
                .Include(c => c.Branch)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                ?? throw new NotFoundException($"Customer with id {id} was not found.");
        }

        private async Task<Branch> GetActiveBranchAsync(long branchId, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId, cancellationToken)
                ?? throw new NotFoundException($"Branch with id {branchId} was not found.");

            if (!branch.IsActive)
                throw new BusinessRuleException("The selected branch is not active.");

            return branch;
        }

        // 10 digits, random, checked for uniqueness (the unique index is the final safety net)
        private async Task<long> GenerateCustomerNumberAsync(CancellationToken cancellationToken)
        {
            for (var attempt = 0; attempt < 10; attempt++)
            {
                var number = Random.Shared.NextInt64(1_000_000_000L, 10_000_000_000L);

                if (!await _context.Customers.AnyAsync(c => c.CustomerNumber == number, cancellationToken))
                    return number;
            }

            throw new BusinessRuleException("Could not generate a unique customer number. Please try again.");
        }

        private CustomerResponse ToResponse(Customer c) => new()
        {
            Id = c.Id,
            CustomerNumber = c.CustomerNumber,
            IsActive = c.IsActive,
            CreatedAt = c.CreatedAt,
            BranchId = c.BranchId,
            BranchName = c.Branch.Name,
            Person = _personService.ToResponse(c.Person)
        };
    }
}
