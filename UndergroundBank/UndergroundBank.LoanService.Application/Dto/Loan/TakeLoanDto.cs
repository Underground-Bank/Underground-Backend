using System.ComponentModel.DataAnnotations;

namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    public class TakeLoanDto
    {
        public Guid TariffId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int LoanAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int LoanDurationInMonths { get; set; }
        public DateOnly StartDate { get; set; }
    }
}
