using MediatR;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod
{
    public class CreateTariffCommandHandler : IRequestHandler<CreateTariffCommand>
    {
        private readonly ITariffService _tariffService;

        public CreateTariffCommandHandler(ITariffService tariffService)
        {
            _tariffService = tariffService;
        }

        public async Task Handle(
            CreateTariffCommand request,
            CancellationToken cancellationToken
        )
        {
            await _tariffService.CreateTariff(request.createTariffDto);
        }

    }
}
