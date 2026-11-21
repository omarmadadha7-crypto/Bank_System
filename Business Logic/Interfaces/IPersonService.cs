using System.Threading;
using System.Threading.Tasks;
using Business_Logic.DTOs.People;
using Data_Access.Entities;

namespace Business_Logic.Interfaces
{
    // Internal helper used by the Customer and User services (no controller uses it).
    // CreateAsync / UpdateAsync only track the changes in the shared (scoped) DbContext:
    // the caller is responsible for SaveChangesAsync, so Person + Customer/User are saved atomically.
    public interface IPersonService
    {
        // Either returns the existing person (personId) or creates a new one (request). Exactly one must be provided.
        Task<Person> ResolveAsync(long? personId, PersonRequest? request, CancellationToken cancellationToken = default);

        Task<Person> CreateAsync(PersonRequest request, CancellationToken cancellationToken = default);

        Task<Person> GetAsync(long personId, CancellationToken cancellationToken = default);

        Task UpdateAsync(Person person, PersonRequest request, CancellationToken cancellationToken = default);

        PersonResponse ToResponse(Person person);
    }
}
