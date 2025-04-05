using UndergroundBank.Common.Data.Enums;
using UndergroundBank.Common.Dto.Transaction;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ILoanService
    {
        public Task TakeLoan(TakeLoanDto takeLoan, Guid userId);
        public Task<GetLoanDto> GetLoan(Guid loanId);
        public Task StartTopUpLoan(
            decimal amount,
            string BankAccountNumber,
            Guid loanId,
            Guid userId
        );
        public Task EndTopUpLoanTransaction(TransactionResponseDto transaction);
        public Task AutoTopUpLoan(Guid loanId, string accountNumber, Guid userId);
        public Task<GetLoansDto> GetAllLoans(Guid? userId);
        public Task<CreditRatingDto> GetCreditRating(Guid userId);
        public Task CreateAutoTopUp(string bankAccountId, Guid loanId, Guid userId);
        public Task DeleteAutoTopUp(string bankAccountId, Guid loanId, Guid userId);
        public Task<GetLoansDto> GetMyLoans(Guid userId);
        public Task<GetAutoTopUpLoanJobsListDto> GetMyAutoTopUpJobs(Guid userId);
    }
}
