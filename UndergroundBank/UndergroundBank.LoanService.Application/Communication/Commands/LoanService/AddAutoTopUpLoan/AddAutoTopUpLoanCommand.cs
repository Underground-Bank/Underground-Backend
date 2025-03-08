using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.AddAutoTopUpLoan
{
    public record AddAutoTopUpLoanCommand(AddAutoTopUpLoanDto addTopUpLoanDto, Guid userId) : IRequest;
}
