using System.ComponentModel.DataAnnotations;

namespace UndergroundBank.LoanService.Application.Dto.Tariff
{
    public class CreateTariffDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MinLoanDuration { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MaxLoanDuration { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MinAmount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public int MaxAmount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Сумма должна быть положительным числом.")]
        public double InterestRate { get; set; }
    }
}
