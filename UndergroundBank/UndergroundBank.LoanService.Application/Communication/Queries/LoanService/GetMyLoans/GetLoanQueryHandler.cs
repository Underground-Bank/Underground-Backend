using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetMyLoans
{
    public class GetMyLoansQueryHandler : IRequestHandler<GetMyLoansQuery, GetLoansDto>
    {
        private readonly ILoanService _loanService;

        public GetMyLoansQueryHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<GetLoansDto> Handle(
            GetMyLoansQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _loanService.GetMyLoans();
        }
    }
}
