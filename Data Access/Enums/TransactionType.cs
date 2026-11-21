namespace Data_Access.Enums
{
    // byte => stored as TINYINT
    public enum TransactionType : byte
    {
        Deposit = 1,
        Withdraw = 2,
        Transfer = 3
    }
}
