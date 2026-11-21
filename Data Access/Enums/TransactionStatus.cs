namespace Data_Access.Enums
{
    // byte => stored as TINYINT
    public enum TransactionStatus : byte
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }
}
