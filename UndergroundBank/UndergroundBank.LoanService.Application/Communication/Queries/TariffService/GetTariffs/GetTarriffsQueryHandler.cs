using MediatR;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariffs
{
    public class GetTariffsQueryHandler : IRequestHandler<GetTariffsQuery, GetTariffsDto>
    {
        private readonly ITariffService _tariffService;

        public GetTariffsQueryHandler(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        public async Task<GetTariffsDto> Handle(
            GetTariffsQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _tariffService.GetAllTariffs();
        }
    }
}
