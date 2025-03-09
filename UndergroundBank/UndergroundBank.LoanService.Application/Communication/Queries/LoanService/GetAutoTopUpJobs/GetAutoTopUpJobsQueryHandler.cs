using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetAutoTopUpJobs
{
    public class GetAutoTopUpJobsQueryHandler : IRequestHandler<GetAutoTopUpJobsQuery, GetAutoTopUpLoanJobsListDto>
    {
        private readonly ILoanService _loanService;

        public GetAutoTopUpJobsQueryHandler(ILoanService loanService)
        {
            _loanService = loanService;
        }

        public async Task<GetAutoTopUpLoanJobsListDto> Handle(
            GetAutoTopUpJobsQuery request,
            CancellationToken cancellationToken
        )
        {
            return await _loanService.GetMyAutoTopUpJobs(request.userId);
        }
    }
}
