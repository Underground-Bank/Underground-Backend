using MediatR;
using UndergroundBank.Common.Dto.BankAccountService;

namespace UndergroundBank.BankAccountService.Application.Communication.Queries.GetAllAccountNumbers
{
    public record GetAllAccountNumbersQuery() : IRequest<List<BankAccountDto>>;
}
