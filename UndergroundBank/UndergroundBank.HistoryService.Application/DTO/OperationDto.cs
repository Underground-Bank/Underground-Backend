using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.Transaction;

namespace UndergroundBank.HistoryService.Application.DTO
{
    public class OperationsHistoryDto
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public string AccountNumber { get; set; }
        public decimal MoneyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public TransactionType transactionType { get; set; }
        public Guid? destinationId { get; set; }
    }
}
