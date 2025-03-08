using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoans
{
    public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, GetLoansDto>
    {
        private readonly ILoanService _loanService;

        public GetLoansQueryHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<GetLoansDto> Handle(
            GetLoansQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _loanService.GetAllLoans(request.userId);
        }
    }
}
