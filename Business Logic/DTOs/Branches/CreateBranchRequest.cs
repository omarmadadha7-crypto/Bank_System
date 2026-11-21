using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Branches
{
    public class CreateBranchRequest
    {
        [Range(1, 99999)]
        public int Code { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string City { get; set; } = string.Empty;
    }
}
