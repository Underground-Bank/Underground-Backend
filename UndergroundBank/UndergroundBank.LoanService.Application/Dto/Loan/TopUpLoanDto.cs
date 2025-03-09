namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class TopUpLoanDto
    {
        public decimal Amount { get; set; }
        public string BankAccountNumber { get; set; }
        public Guid LoanId { get; set; }
    }
}
