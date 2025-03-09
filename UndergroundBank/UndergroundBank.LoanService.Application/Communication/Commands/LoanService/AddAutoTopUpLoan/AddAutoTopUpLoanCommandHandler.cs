using MediatR;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.AddAutoTopUpLoan
{
    public class AddAutoTopUpLoanCommandHandler : IRequestHandler<AddAutoTopUpLoanCommand>
    {
        private readonly ILoanService _loanService;

        public AddAutoTopUpLoanCommandHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task Handle(
            AddAutoTopUpLoanCommand request,
            CancellationToken cancellationToken
        )
        {
            await _loanService.CreateAutoTopUp(request.addTopUpLoanDto.BankAccountId, request.addTopUpLoanDto.LoanId, request.userId);
        }

    }
}
