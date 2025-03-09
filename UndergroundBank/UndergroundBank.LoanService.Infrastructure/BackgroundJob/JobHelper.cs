using Quartz;
using UndergroundBank.LoanService.Application.Dto.Loan;

namespace UndergroundBank.LoanService.Infrastructure.BackgroundJob
{
    public static class JobHelper
    {
        public static BackgroundJobDto GenerateJobKeyAndTriggerForLoan(
            string bankAccountNumber,
            Guid loanId,
            Guid userId
        )
        {
            var jobKey = new JobKey(
                $"{loanId}-{bankAccountNumber}-{userId}",
                "CreditRepaymentJobs"
            );

            var job = JobBuilder
                .Create<TopUpLoanJob>()
                .WithIdentity(jobKey)
                .UsingJobData("LoanId", loanId.ToString())
                .UsingJobData("BankAccountNumber", bankAccountNumber.ToString())
                .UsingJobData("UserId", userId.ToString())
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity(
                    $"{loanId}-{bankAccountNumber}-{userId}Trigger",
                    "CreditRepaymentJobs"
                )
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(20).RepeatForever())
                .Build();

            return new BackgroundJobDto { Job = job, Trigger = trigger };
        }
    }
}
