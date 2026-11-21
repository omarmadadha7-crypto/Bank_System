using System;

namespace Business_Logic.DTOs.Currencies
{
    public class CurrencyResponse
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public decimal RateToUsd { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
