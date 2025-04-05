using MediatR;
using UndergroundBank.Common.DTO.Transaction;

namespace UndergroundBank.HistoryService.Application.Communication.Queries.GetOverduePayments
{
    public record GetOverduePaymentsQuery(Guid? loanId, Guid userId) : IRequest<List<GetOverduePaymentDto>>;
}
