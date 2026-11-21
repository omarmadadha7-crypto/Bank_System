using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.Common;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Seed
{
    // Inserts the minimum data needed to use the system. Safe to run on every startup (idempotent).
    // Run it AFTER the database exists (Update-Database).
    public class DatabaseSeeder
    {
        private const string AdminUserName = "admin";

        private readonly BankContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(BankContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync(string adminPassword, CancellationToken cancellationToken = default)
        {
            await SeedRolesAsync(cancellationToken);
            await SeedCurrenciesAsync(cancellationToken);
            var branch = await SeedBranchAsync(cancellationToken);
            await SeedAdminAsync(branch, adminPassword, cancellationToken);
        }

        private async Task SeedRolesAsync(CancellationToken cancellationToken)
        {
            foreach (var name in new[] { RoleNames.Admin, RoleNames.Manager, RoleNames.Teller })
            {
                if (!await _context.Roles.AnyAsync(r => r.Name == name, cancellationToken))
                    _context.Roles.Add(new Role { Name = name });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedCurrenciesAsync(CancellationToken cancellationToken)
        {
            var defaults = new[]
            {
                new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", RateToUsd = 1m },
                new Currency { Code = "JOD", Name = "Jordanian Dinar", Symbol = "JD", RateToUsd = 1.41m }
            };

            foreach (var currency in defaults)
            {
                var code = currency.Code;

                if (!await _context.Currencies.AnyAsync(c => c.Code == code, cancellationToken))
                    _context.Currencies.Add(currency);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task<Branch> SeedBranchAsync(CancellationToken cancellationToken)
        {
            var branch = await _context.Branches.OrderBy(b => b.Id).FirstOrDefaultAsync(cancellationToken);
            if (branch is not null)
                return branch;

            branch = new Branch { Code = 1, Name = "Head Office", City = "Amman", IsActive = true };
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return branch;
        }

        private async Task SeedAdminAsync(Branch branch, string adminPassword, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == AdminUserName, cancellationToken))
                return;

            var adminRole = await _context.Roles.FirstAsync(r => r.Name == RoleNames.Admin, cancellationToken);

            var person = new Person
            {
                NationalId = "0000000000",
                FirstName = "System",
                LastName = "Administrator",
                DateOfBirth = new DateTime(1990, 1, 1),
                Email = "admin@bank.local",
                Phone = "0000000000"
            };

            var user = new User
            {
                UserName = AdminUserName,
                PasswordHash = _passwordHasher.Hash(adminPassword),
                IsActive = true,
                Person = person,
                Role = adminRole,
                Branch = branch
            };

            _context.Users.Add(user);   // the person is saved with it
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
