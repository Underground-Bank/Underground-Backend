using MediatR;
using UndergroundBank.LoanService.Application.Dto.Tariff;

namespace UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariffs
{
    public record GetTariffsQuery() : IRequest<GetTariffsDto>;
}
