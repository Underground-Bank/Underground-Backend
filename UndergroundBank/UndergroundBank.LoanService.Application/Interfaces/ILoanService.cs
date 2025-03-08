using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ILoanService
    {
        public Task TakeLoan(TakeLoanDto takeLoan, Guid userId);
        public Task<GetLoanDto> GetLoan(Guid loanId);
        public Task StartTopUpLoan(decimal amount, string BankAccountNumber, Guid loanId);
        public Task EndTopUpLoanTransaction(TransactionResponseDto transaction);
        public Task AutoTopUpLoan(Guid loanId, string accountNumber);
        public Task<GetLoansDto> GetAllLoans(Guid? userId);
        public Task CreateAutoTopUp(Guid bankAccountId, Guid loanId);
        public Task<GetLoansDto> GetMyLoans();
    }
}
