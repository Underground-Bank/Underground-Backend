using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.SomeMethod
{
    public record TakeLoanCommand(TakeLoanDto takeLoanCreds, Guid userId) : IRequest;
}
