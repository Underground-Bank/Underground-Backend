using Microsoft.Extensions.DependencyInjection;
using Quartz;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Infrastructure.BackgroundJob
{
    public class TopUpLoanJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TopUpLoanJob(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();

                var dataMap = context.JobDetail.JobDataMap;

                var loanId = dataMap.GetString("LoanId");
                var bankAccountNumber = dataMap.GetString("BankAccountNumber");
                await loanService.AutoTopUpLoan(Guid.Parse(loanId), bankAccountNumber);
            }
        }
    }
}