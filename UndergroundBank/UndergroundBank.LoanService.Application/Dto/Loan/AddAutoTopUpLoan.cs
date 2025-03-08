namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class AddAutoTopUpLoanDto
    {
        public string BankAccountId { get; set; }
        public Guid LoanId { get; set; }
    }
}
