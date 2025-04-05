using MediatR;
using UndergroundBank.Common.DTO.Transaction;
using UndergroundBank.HistoryService.Application.Interfaces;

namespace UndergroundBank.HistoryService.Application.Communication.Queries.GetOverduePayments
{
    public class GetOverduePaymentQueryHandler
        : IRequestHandler<GetOverduePaymentsQuery, List<GetOverduePaymentDto>>
    {
        private readonly IHistoryService _historyService;

        public GetOverduePaymentQueryHandler(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async Task<List<GetOverduePaymentDto>> Handle(
            GetOverduePaymentsQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _historyService.GetOverduedPayments(request.loanId, request.userId);
        }
    }
}
