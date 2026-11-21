using System.Linq;
using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Access.DbContexts
{
    public class BankContext : DbContext
    {
        public BankContext(DbContextOptions<BankContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Person> People => Set<Person>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rules that attributes cannot express (check constraints, composite indexes).
            // Picks up every IEntityTypeConfiguration<T> in the Configurations folder.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankContext).Assembly);

            // Financial data is never cascade-deleted (also avoids "multiple cascade paths" in SQL Server)
            foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
