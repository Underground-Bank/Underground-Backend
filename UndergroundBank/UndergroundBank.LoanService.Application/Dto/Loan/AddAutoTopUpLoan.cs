namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class AddAutoTopUpLoanDto
    {
        public Guid BankAccountId { get; set; }
        public Guid LoanId { get; set; }
    }
}
