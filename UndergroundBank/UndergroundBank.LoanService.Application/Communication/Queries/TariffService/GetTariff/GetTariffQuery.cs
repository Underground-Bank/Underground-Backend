using MediatR;
using UndergroundBank.LoanService.Application.Dto.Tariff;

namespace UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariff
{
    public record GetTariffQuery(Guid tariffId) : IRequest<GetTariffDto>;
}
