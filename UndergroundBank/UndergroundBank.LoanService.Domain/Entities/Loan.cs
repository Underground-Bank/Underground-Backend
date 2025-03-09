namespace UndergroundBank.LoanService.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly StartDate { get; set; }
        public int LoanDurationInMonth { get; set; }
        public LoanStatus Status { get; set; }
        public Guid TariffId { get; set; }
        public Tariff Tariff { get; set; }
        public decimal RemainingPayment { get; set; }
    }

    public enum LoanStatus
    {
        NotStarted,
        Active,
        Closed,
        Overdue,
    }
}
