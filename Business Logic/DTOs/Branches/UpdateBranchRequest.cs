using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Branches
{
    // Code is intentionally not editable
    public class UpdateBranchRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string City { get; set; } = string.Empty;
    }
}
