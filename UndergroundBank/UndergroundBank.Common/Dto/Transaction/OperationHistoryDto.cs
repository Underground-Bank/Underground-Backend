using UndergroundBank.Common.Data.Enums;

namespace UndergroundBank.Common.Dto.Transaction
{
    /// <summary>
    /// This DTO is needed for exchange between the history service and other services
    /// </summary>
    ///
    ///TODO: Refactor it to be more uniform and work with account transfers
    public class OperationHistoryDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public decimal MoneyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public TransactionType TransactionType { get; set; }
        public Guid? DestinationId { get; set; }
    }

    public enum TransactionType
    {
        LoanPayment,
        Withdraw,
        TopUp,
    }
}
