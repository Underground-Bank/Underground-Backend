using Quartz;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Infrastructure.BackgroundJob;

namespace UndergroundBank.LoanService.Application.Dto.Loan
{
    internal class TopUpLoanScheduler : ITopUpLoanJob
    {
        private readonly ISchedulerFactory _schedulerFactory;

        public TopUpLoanScheduler(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory;
        }
        public async Task ScheduleCreditJobAsync(Guid creditId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(creditId.ToString(), "CreditJobs");

            var job = JobBuilder.Create<TopUpLoanJob>()
                .WithIdentity(jobKey)
                .UsingJobData("CreditId", creditId.ToString())
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{creditId}-trigger", "CreditTriggers")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(20)
                    .RepeatForever())
                .Build();

            await scheduler.ScheduleJob(job, trigger);
        }

        public async Task RemoveCreditJobAsync(Guid creditId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey(creditId.ToString(), "CreditJobs");
            await scheduler.DeleteJob(jobKey);
        }
    }
}
