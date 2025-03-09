using MediatR;
using UndergroundBank.HistoryService.Application.DTO;

namespace UndergroundBank.HistoryService.Application.Communication.Queries.GetAccountNumbersWithUserId
{
    public record GetHistoryByBankAccountNumberQuery(string BankAccountNumber, Guid? userId) : IRequest<GetOpeationsHistoryDto>;
}
