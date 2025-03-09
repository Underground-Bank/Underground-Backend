using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoan
{
    public class GetLoanQueryHandler : IRequestHandler<GetLoanQuery, GetLoanDto>
    {
        private readonly ILoanService _loanService;

        public GetLoanQueryHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<GetLoanDto> Handle(
            GetLoanQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _loanService.GetLoan(request.loanId);
        }
    }
}
