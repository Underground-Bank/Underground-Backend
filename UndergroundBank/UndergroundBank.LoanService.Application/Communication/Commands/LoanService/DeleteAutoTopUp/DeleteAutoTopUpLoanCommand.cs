using MediatR;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Communication.Commands.LoanService.DeleteAutoTopUpLoan
{
    public record DeleteAutoTopUpLoanCommand(AutoTopUpLoanDto deleteAutoTopUpLoanDto, Guid userId) : IRequest;
}
