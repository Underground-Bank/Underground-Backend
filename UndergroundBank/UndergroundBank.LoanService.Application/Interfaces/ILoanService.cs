

using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ILoanService
    {
        public Task TakeLoan(TakeLoanDto takeLoan, Guid userId);
        public Task GetLoan();
        public Task GetAllLoans();
        public Task GetMyLoans();
    }

}
