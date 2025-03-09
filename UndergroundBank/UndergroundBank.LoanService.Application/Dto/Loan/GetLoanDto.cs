using UndergroundBank.LoanService.Domain.Entities;

namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class GetLoanDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly StartDate { get; set; }
        public int LoanDurationInMonth { get; set; }
        public LoanStatus Status { get; set; }
        public Guid TariffId { get; set; }
        public decimal RemainingPayment { get; set; }
    }
}
