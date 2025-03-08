using MediatR;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetAccountNumbersWithUserId
{
    public record GetAccountNumbersWithUserIdQuery(Guid userId) : IRequest<List<BankAccountDto>>;
}
