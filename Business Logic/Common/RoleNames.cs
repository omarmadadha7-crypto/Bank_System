namespace Business_Logic.Common
{
    // Must match the role names stored in the Roles table (see DatabaseSeeder)
    public static class RoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Teller = "Teller";

        // Comma separated lists, used by [Authorize(Roles = ...)]
        public const string AdminOrManager = Admin + "," + Manager;
        public const string AllStaff = Admin + "," + Manager + "," + Teller;
    }
}
