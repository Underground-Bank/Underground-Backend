using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Queries.LoanService.GetLoans
{
    public record GetLoansQuery(Guid? userId) : IRequest<GetLoansDto>;
}
