using MediatR;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod
{
    public class TopUpLoanCommandHandler : IRequestHandler<TakeLoanCommand>
    {
        private readonly ILoanService _loanService;

        public TopUpLoanCommandHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task Handle(
            TakeLoanCommand request,
            CancellationToken cancellationToken
        )
        {
            await _loanService.TakeLoan(request.takeLoanCreds, request.userId);
        }

    }
}
