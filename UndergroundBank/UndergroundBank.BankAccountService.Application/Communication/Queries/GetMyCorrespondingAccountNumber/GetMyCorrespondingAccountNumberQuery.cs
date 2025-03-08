using MediatR;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyCorrespondingAccountNumber
{
    public record GetMyCorrespondingAccountNumberQuery(string accountNumber, Guid userId)
        : IRequest<BankAccountDto>;
}
