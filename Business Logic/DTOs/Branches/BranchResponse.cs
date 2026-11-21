namespace Business_Logic.DTOs.Branches
{
    public class BranchResponse
    {
        public long Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
