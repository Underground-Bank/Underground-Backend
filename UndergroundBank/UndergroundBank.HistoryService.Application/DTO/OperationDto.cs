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
        public TransactionType TransactionType { get; set; }
        public string? DestinationAccountNumber { get; set; }
        public Guid? DestinationLoanId { get; set; }

        public string GetDestinationAccountNumber()
        {
            if (!string.IsNullOrEmpty(DestinationAccountNumber))
                return DestinationAccountNumber;
            throw new InvalidOperationException("DestinationAccountNumber отсутствует.");
        }

        public Guid GetDestinationLoanId()
        {
            if (DestinationLoanId.HasValue)
                return DestinationLoanId.Value;
            throw new InvalidOperationException("DestinationLoanId отсутствует.");
        }

        public (Guid? loanId, string? accountNumber) GetDestination()
        {
            return (DestinationLoanId, DestinationAccountNumber);
        }
    }
}