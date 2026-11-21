namespace Business_Logic.Common
{
    public static class ValidationPatterns
    {
        // 8-100 chars, at least one lowercase, one uppercase and one digit
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$";
        public const string PasswordMessage = "Password must be 8-100 characters and contain a lowercase letter, an uppercase letter and a digit.";

        // 4 to 6 digits
        public const string Pin = @"^\d{4,6}$";
        public const string PinMessage = "PIN must be 4 to 6 digits.";

        // letters, digits, dot, underscore, hyphen
        public const string UserName = @"^[A-Za-z0-9._-]{3,50}$";
        public const string UserNameMessage = "User name must be 3-50 characters: letters, digits, '.', '_' or '-'.";
    }
}
