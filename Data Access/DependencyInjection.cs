using Data_Access.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data_Access
{
    public static class DependencyInjection
    {
        // Registers BankContext (SQL Server). Call it once from Program.cs.
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BankContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
