namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface ITopUpLoanJob
    {
        Task ScheduleCreditJobAsync(Guid creditId);
        Task RemoveCreditJobAsync(Guid creditId);
    }
}
