using MediatR;
using UndergroundBank.LoanService.Application.Dto.Tariff;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.TariffService.GetTariff
{
    public class GetTariffQueryHandler : IRequestHandler<GetTariffQuery, GetTariffDto>
    {
        private readonly ITariffService _tariffService;

        public GetTariffQueryHandler(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        public async Task<GetTariffDto> Handle(
            GetTariffQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _tariffService.GetTariff(request.tariffId);
        }
    }
}
