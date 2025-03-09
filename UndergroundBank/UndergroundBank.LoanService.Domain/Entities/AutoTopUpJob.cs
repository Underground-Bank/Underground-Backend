namespace UndergroundBank.LoanService.Domain.Entities
{
    public class AutoTopUpJob
    {
        public string BankAccountNumber { get; set; }
        public Guid LoanId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public JobStatus Status { get; set; }
    }

    public enum JobStatus
    {
        Active,
        Closed
    }
}
