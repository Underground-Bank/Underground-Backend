using MediatR;
using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetMyCorrespondingAccountNumber
{
    public record GetMyCorrespondingAccountNumberQuery(
        string accountNumber,
        Guid userId,
        List<Role> userRoles
    ) : IRequest<BankAccountDto>;
}
