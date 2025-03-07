using System.ComponentModel.DataAnnotations;

namespace UndergroundBank.LoanService.Domain.Entities
{
    public class Tariff
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MinLoanDuration { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MaxLoanDuration { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public double MinAmount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public double MaxAmount { get; set; }
        public decimal InterestRate { get; set; }
    }
}
