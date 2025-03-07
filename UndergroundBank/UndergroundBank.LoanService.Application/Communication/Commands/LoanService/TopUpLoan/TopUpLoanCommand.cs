using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.TopUpLoan
{
    public record TopUpLoanCommand(TopUpLoanDto topUpLoanDto) : IRequest;
}
