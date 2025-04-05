using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetCreditRating;

public class GetCreditRatingQueryHandler : IRequestHandler<GetCreditRatingQuery, CreditRatingDto>
{
    private readonly ILoanService _loanService;

    public GetCreditRatingQueryHandler(ILoanService loanService)
    {
        _loanService = loanService;
    }

    public async Task<CreditRatingDto> Handle(
        GetCreditRatingQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _loanService.GetCreditRating(request.userId);
    }
}

