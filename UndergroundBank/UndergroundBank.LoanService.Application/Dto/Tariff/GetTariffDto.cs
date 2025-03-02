namespace UndergroundBank.LoanService.Application.Dto.Tariff
{
    public class GetTariffDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MinLoanDuration { get; set; }
        public int MaxLoanDuration { get; set; }
        public int MinAmount { get; set; }
        public int MaxAmount { get; set; }
        public double InterestRate { get; set; }
    }
}
