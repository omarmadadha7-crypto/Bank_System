using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Access.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // last line of defence: the business layer already checks the balance before a withdrawal
            builder.ToTable(t => t.HasCheckConstraint("CK_Accounts_Balance_NonNegative", "[Balance] >= 0"));
        }
    }
}
