
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.HistoryService.Application.DTO;

namespace UndergroundBank.HistoryService.Application.Interfaces
{
    public interface IHistoryService
    {
        public Task<GetOpeationsHistoryDto> GetOperationsHistory(string BankAccountNumber, Guid? userId);
        public Task AddToHistory(OperationResultDto operationHistoryDto);
    }
}
