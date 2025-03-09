using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.Common.Data;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Infrastructure;
using UndergroundBank.LoanService.Infrastructure.Repositories;
using UndergroundBank.LoanService.Infrastructure.Services;
using UndergroundBank.LoanService.Infrastructure.Services.LoanQueue;

namespace UndergroundBank.LoanService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddLoanBlServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<LoanDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("LoanDatabasePostgres"))
            );
            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });
            services.AddSingleton<QueueSender>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<ILoanService, LoansService>();
            services.AddScoped<ITariffService, TariffService>();
            services.AddScoped<IJobSchedulerService, JobSchedulerService>();

            return services;
        }

        public static IServiceCollection AddQuartzDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddQuartz();
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            services.AddScoped<Infrastructure.BackgroundJob.TopUpLoanJob>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            return services;
        }
    }
}
