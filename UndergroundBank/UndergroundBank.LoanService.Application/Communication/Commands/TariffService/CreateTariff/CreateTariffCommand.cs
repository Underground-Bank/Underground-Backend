using MediatR;
using UndergroundBank.LoanService.Application.Dto.Tariff;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod
{
    public record CreateTariffCommand(CreateTariffDto createTariffDto) : IRequest;
}
