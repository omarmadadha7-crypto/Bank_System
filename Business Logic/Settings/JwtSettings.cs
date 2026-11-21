namespace Business_Logic.Settings
{
    public class JwtSettings
    {
        // At least 32 characters. Keep it out of source control (user-secrets / environment variable).
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; } = 60;
    }
}
