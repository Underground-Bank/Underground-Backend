namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class AutoTopUpLoanDto
    {
        public string BankAccountId { get; set; }
        public Guid LoanId { get; set; }
    }
}
