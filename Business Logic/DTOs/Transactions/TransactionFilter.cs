using System;
using Data_Access.Enums;

namespace Business_Logic.DTOs.Transactions
{
    public class TransactionFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public TransactionType? Type { get; set; }
        public TransactionStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
