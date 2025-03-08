using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using UndergroundBank.LoanService.Application.Interfaces;

namespace UndergroundBank.LoanService.Infrastructure.BackgroundJob
{
    public class TopUpLoanJob : IJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<TopUpLoanJob> _logger;

        public TopUpLoanJob(IServiceScopeFactory serviceScopeFactory, ILogger<TopUpLoanJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            Console.WriteLine("FUCKING STAAAAAAAAAARTED");
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("Starting job execution");
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    Console.WriteLine("Service scope created");

                    _logger.LogInformation($"Executing job {context.JobDetail.Key.Name} at {DateTime.UtcNow}");
                    var loanService = scope.ServiceProvider.GetRequiredService<ILoanService>();

                    var dataMap = context.JobDetail.JobDataMap;

                    var loanId = dataMap.GetString("LoanId");
                    var bankAccountNumber = dataMap.GetString("BankAccountNumber");
                    var userId = dataMap.GetString("UserId");

                    Console.WriteLine($"LoanId: {loanId}, BankAccountNumber: {bankAccountNumber}");

                    await loanService.AutoTopUpLoan(Guid.Parse(loanId), bankAccountNumber, Guid.Parse(userId));

                    Console.WriteLine("Job completed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                _logger.LogError(ex, "Job failed");
            }
        }
    }
}