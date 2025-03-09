namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class GetAutoTopUpLoanJobDto
    {
        public string BankAccountNumber { get; set; }
        public Guid LoanId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetAutoTopUpLoanJobsListDto
    {
        public List<GetAutoTopUpLoanJobDto> AutoTopUpLoanJobDtos { get; set; }
    }
}
