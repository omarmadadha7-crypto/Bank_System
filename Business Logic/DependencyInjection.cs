using Business_Logic.Interfaces;
using Business_Logic.Seed;
using Business_Logic.Services;
using Business_Logic.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Business_Logic
{
    public static class DependencyInjection
    {
        // Registers every service of the Business layer. Call it once from Program.cs.
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, JwtSettings jwtSettings)
        {
            // configuration + stateless services
            services.AddSingleton(jwtSettings);
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<ITokenService, TokenService>();

            // services that use the scoped BankContext
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<DatabaseSeeder>();

            return services;
        }
    }
}
