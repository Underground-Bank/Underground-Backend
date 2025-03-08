namespace UndergroundBank.LoanService.Application.Dto.Tariff
{
    public class GetTariffDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinLoanDuration { get; set; }
        public int MaxLoanDuration { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal InterestRate { get; set; }
    }
}
