using MediatR;
using UndergroundBank.HistoryService.Application.DTO;
using UndergroundBank.HistoryService.Application.Interfaces;

namespace UndergroundBank.HistoryService.Application.Communication.Queries.GetAccountNumbersWithUserId
{
    public class GetHistoryByBankAccountNumberQueryHandler
        : IRequestHandler<GetHistoryByBankAccountNumberQuery, GetOpeationsHistoryDto>
    {
        private readonly IHistoryService _historyService;

        public GetHistoryByBankAccountNumberQueryHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async Task<GetOpeationsHistoryDto> Handle(
            GetHistoryByBankAccountNumberQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _historyService.GetOperationsHistory(request.BankAccountNumber, request.userId);
        }
    }
}
