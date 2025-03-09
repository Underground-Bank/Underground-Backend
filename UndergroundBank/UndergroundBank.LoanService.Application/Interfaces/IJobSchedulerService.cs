namespace UndergroundBank.LoanService.Application.Interfaces
{
    public interface IJobSchedulerService
    {
        Task StartActiveJobsAsync();
    }
}
