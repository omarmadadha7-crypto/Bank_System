using System.ComponentModel.DataAnnotations;

namespace Business_Logic.DTOs.Currencies
{
    public class UpdateRateRequest
    {
        [Range(0.000001, 1000000000)]
        public decimal RateToUsd { get; set; }
    }
}
