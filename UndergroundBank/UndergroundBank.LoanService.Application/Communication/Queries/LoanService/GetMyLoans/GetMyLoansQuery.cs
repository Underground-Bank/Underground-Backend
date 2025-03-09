using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetMyLoans
{
    public record GetMyLoansQuery(Guid userId) : IRequest<GetLoansDto>;
}
