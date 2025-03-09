using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto.Transaction
{
    public class TransactionResponseDto
    {
        public Guid TransactionId { get; set; }
        public Guid LoanId { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public decimal MoneyCount { get; set; }
        public Status Status { get; set; }
    }
}
