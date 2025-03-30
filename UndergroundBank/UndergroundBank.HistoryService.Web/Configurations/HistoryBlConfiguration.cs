using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data;
using UndergroundBank.HistoryService.Application.Interfaces;
using UndergroundBank.HistoryService.Infrastructure;
using UndergroundBank.HistoryService.Infrastructure.Services;
using UndergroundBank.HistoryService.Infrastructure.Services.HistoryQueue;

namespace UndergroundBank.LoanService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddHistoryBlServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHttpContextAccessor();

            services.AddDbContext<HistoryDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("HistoryDatabasePostgres"))
            );
            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });
            services.AddScoped<OperationHistoryHub>();
            services.AddSingleton<QueueSender>();
            services.AddScoped<IHistoryService, OperationHistoryService>();
            return services;
        }
    }
}
