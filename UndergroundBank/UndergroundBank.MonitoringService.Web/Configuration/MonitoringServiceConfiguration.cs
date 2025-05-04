using Microsoft.EntityFrameworkCore;
using UndergroundBank.Common.Data;
using UndergroundBank.HistoryService.Infrastructure.Services;
using UndergroundBank.MonitoringService.Application.Interfaces;
using UndergroundBank.MonitoringService.Infrastructure;

namespace UndergroundBank.MonitoringService.Web.Configuration
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddMonitoringServiceConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<MonitoringDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("MonitoringDatabasePostgres"))
            );

            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IMonitoringService, MonitorService>();

            return services;
        }
    }
}
