using Quartz;
using Quartz.Spi;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Domain.Entities;
using UndergroundBank.LoanService.Infrastructure;
using UndergroundBank.LoanService.Infrastructure.BackgroundJob;

public class JobSchedulerService : IJobSchedulerService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly LoanDbContext _dbContext;

    public JobSchedulerService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, LoanDbContext dbContext)
    {
        _schedulerFactory = schedulerFactory;
        _jobFactory = jobFactory;
        _dbContext = dbContext;
    }

    public async Task StartActiveJobsAsync()
    {
        var scheduler = await _schedulerFactory.GetScheduler();
        scheduler.JobFactory = _jobFactory;

        var activeJobs = _dbContext.TopUpJobs
            .Where(j => j.Status == JobStatus.Active)
            .ToList();

        foreach (var job in activeJobs)
        {
            var jobCredsDto = JobHelper.GenerateJobKeyAndTriggerForLoan(job.BankAccountNumber, job.LoanId, job.UserId);
            await scheduler.ScheduleJob(jobCredsDto.Job, jobCredsDto.Trigger);
        }

        await scheduler.Start();
    }
}