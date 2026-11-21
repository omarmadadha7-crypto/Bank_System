namespace Data_Access.Enums
{
    // byte => stored as TINYINT
    public enum AccountStatus : byte
    {
        Active = 1,
        Frozen = 2,
        Closed = 3
    }
}
