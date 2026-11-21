using Data_Access.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Access.Configurations
{
    public class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            // an exchange rate of zero or less would break every conversion (division by it)
            builder.ToTable(t => t.HasCheckConstraint("CK_Currencies_RateToUsd_Positive", "[RateToUsd] > 0"));
        }
    }
}
