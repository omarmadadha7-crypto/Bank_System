using Data_Access.Entities;
using Data_Access.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data_Access.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Transactions_Amount_Positive", "[Amount] > 0");

                // Deposit  => destination only
                // Withdraw => source only
                // Transfer => both, and they must be different accounts
                t.HasCheckConstraint("CK_Transactions_AccountsMatchType",
                    $"([Type] = {(byte)TransactionType.Deposit} AND [SourceAccountId] IS NULL AND [DestinationAccountId] IS NOT NULL) OR " +
                    $"([Type] = {(byte)TransactionType.Withdraw} AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NULL) OR " +
                    $"([Type] = {(byte)TransactionType.Transfer} AND [SourceAccountId] IS NOT NULL AND [DestinationAccountId] IS NOT NULL AND [SourceAccountId] <> [DestinationAccountId])");

                // a failure reason exists if, and only if, the transaction failed
                t.HasCheckConstraint("CK_Transactions_FailureReason",
                    $"([Status] = {(byte)TransactionStatus.Failed} AND [FailureReason] IS NOT NULL) OR " +
                    $"([Status] <> {(byte)TransactionStatus.Failed} AND [FailureReason] IS NULL)");
            });

            // account statements: WHERE account = X ORDER BY CreatedAt DESC
            builder.HasIndex(t => new { t.SourceAccountId, t.CreatedAt });
            builder.HasIndex(t => new { t.DestinationAccountId, t.CreatedAt });
        }
    }
}
