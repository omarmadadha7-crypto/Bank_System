using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Currencies
{
    // Code is intentionally not editable
    public class UpdateCurrencyRequest
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(5)]
        public string Symbol { get; set; } = string.Empty;
    }
}
