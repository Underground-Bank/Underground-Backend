using MediatR;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.DeleteAutoTopUpLoan
{
    public class DeleteAutoTopUpLoanCommandHandler : IRequestHandler<DeleteAutoTopUpLoanCommand>
    {
        private readonly ILoanService _loanService;

        public DeleteAutoTopUpLoanCommandHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task Handle(
            DeleteAutoTopUpLoanCommand request,
            CancellationToken cancellationToken
        )
        {
            await _loanService.DeleteAutoTopUp(request.deleteAutoTopUpLoanDto.BankAccountId, request.deleteAutoTopUpLoanDto.LoanId, request.userId);
        }

    }
}
