using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ILoanService
    {
        public Task TakeLoan(TakeLoanDto takeLoan, Guid userId);
        public Task<GetLoanDto> GetLoan(Guid loanId);
        public Task TopUpLoan(decimal amount, Guid loanId);
        public Task AutoTopUpLoan(Guid loanId);
        public Task<GetLoansDto> GetAllLoans();
        public Task CreateAutoTopUp(Guid bankAccountId, Guid loanId);
        public Task<GetLoansDto> GetMyLoans();
    }

}
