using MediatR;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.TopUpLoan
{
    public class TopUpLoanCommandHandler : IRequestHandler<TopUpLoanCommand>
    {
        private readonly ILoanService _loanService;

        public TopUpLoanCommandHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task Handle(
            TopUpLoanCommand request,
            CancellationToken cancellationToken
        )
        {
            await _loanService.TopUpLoan(request.topUpLoanDto.Amount, request.topUpLoanDto.LoanId);
        }

    }
}
