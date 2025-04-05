using UndergroundBank.Common.Dto.Transaction;

namespace UndergroundBank.HistoryService.Application.DTO
{
    public class OverduePaymentDto
    {
        public Guid LoanId { get; set; }
        public OperationHistoryDto OperationHistoryDto { get; set; }
    }
}
