namespace Business_Logic.Interfaces
{
    // Used for both user passwords and account PINs
    public interface IPasswordHasher
    {
        string Hash(string secret);
        bool Verify(string secret, string hash);
    }
}
