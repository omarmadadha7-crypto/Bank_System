using System;
using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.People;
using Business_Logic.Exceptions;
using Business_Logic.Interfaces;
using Data_Access.DbContexts;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business_Logic.Services
{
    public class PersonService : IPersonService
    {
        private const int MinimumAge = 18;

        private readonly BankContext _context;

        public PersonService(BankContext context)
        {
            _context = context;
        }

        public async Task<Person> ResolveAsync(long? personId, PersonRequest? request, CancellationToken cancellationToken = default)
        {
            // both provided, or none provided
            if (personId.HasValue == (request is not null))
                throw new BusinessRuleException("Provide either PersonId (an existing person) or Person (data of a new person), but not both.");

            return personId.HasValue
                ? await GetAsync(personId.Value, cancellationToken)
                : await CreateAsync(request!, cancellationToken);
        }

        public async Task<Person> CreateAsync(PersonRequest request, CancellationToken cancellationToken = default)
        {
            ValidateAge(request.DateOfBirth);

            var nationalId = request.NationalId.Trim();
            var email = request.Email.Trim().ToLowerInvariant();
            var phone = request.Phone.Trim();

            await EnsureUniqueAsync(0, nationalId, email, phone, cancellationToken);

            var person = new Person
            {
                NationalId = nationalId,
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DateOfBirth = request.DateOfBirth.Date,
                Email = email,
                Phone = phone,
                Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim()
            };

            _context.People.Add(person);   // saved by the caller
            return person;
        }

        public async Task<Person> GetAsync(long personId, CancellationToken cancellationToken = default)
        {
            return await _context.People.FirstOrDefaultAsync(p => p.Id == personId, cancellationToken)
                ?? throw new NotFoundException($"Person with id {personId} was not found.");
        }

        public async Task UpdateAsync(Person person, PersonRequest request, CancellationToken cancellationToken = default)
        {
            ValidateAge(request.DateOfBirth);

            var nationalId = request.NationalId.Trim();
            var email = request.Email.Trim().ToLowerInvariant();
            var phone = request.Phone.Trim();

            await EnsureUniqueAsync(person.Id, nationalId, email, phone, cancellationToken);

            person.NationalId = nationalId;
            person.FirstName = request.FirstName.Trim();
            person.LastName = request.LastName.Trim();
            person.DateOfBirth = request.DateOfBirth.Date;
            person.Email = email;
            person.Phone = phone;
            person.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();
        }

        public PersonResponse ToResponse(Person person) => new()
        {
            Id = person.Id,
            NationalId = person.NationalId,
            FirstName = person.FirstName,
            LastName = person.LastName,
            FullName = $"{person.FirstName} {person.LastName}",
            DateOfBirth = person.DateOfBirth,
            Email = person.Email,
            Phone = person.Phone,
            Address = person.Address
        };

        // ---------- helpers ----------

        // excludeId = 0 when creating (ids start at 1, so nothing is excluded)
        private async Task EnsureUniqueAsync(long excludeId, string nationalId, string email, string phone, CancellationToken cancellationToken)
        {
            if (await _context.People.AnyAsync(p => p.Id != excludeId && p.NationalId == nationalId, cancellationToken))
                throw new ConflictException("A person with this national id already exists.");

            if (await _context.People.AnyAsync(p => p.Id != excludeId && p.Email == email, cancellationToken))
                throw new ConflictException("A person with this email already exists.");

            if (await _context.People.AnyAsync(p => p.Id != excludeId && p.Phone == phone, cancellationToken))
                throw new ConflictException("A person with this phone number already exists.");
        }

        private static void ValidateAge(DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow.Date;
            var birth = dateOfBirth.Date;

            var age = today.Year - birth.Year;
            if (birth > today.AddYears(-age))
                age--;

            if (age < MinimumAge)
                throw new BusinessRuleException($"A person must be at least {MinimumAge} years old.");
        }
    }
}
