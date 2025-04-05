using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.HistoryService.Application.DTO;

namespace UndergroundBank.HistoryService.Application.Interfaces
{
    public interface IHistoryService
    {
        public Task<GetOpeationsHistoryDto> GetOperationsHistory(
            string BankAccountNumber,
            Guid? userId
        );
        public Task AddToHistory(OperationHistoryDto operationHistoryDto);
        public Task AddOverduePayment(OverduePaymentDto overduePaymentDto);
        public Task<List<GetOverduePaymentDto>> GetOverduedPayments(Guid? loanId, Guid userId);
    }
}
