using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Currencies
{
    public class CreateCurrencyRequest
    {
        [Required, StringLength(3, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(5)]
        public string Symbol { get; set; } = string.Empty;

        // Value of ONE unit of this currency in USD (USD = 1, JOD = 1.41 ...)
        [Range(0.000001, 1000000000)]
        public decimal RateToUsd { get; set; }
    }
}
