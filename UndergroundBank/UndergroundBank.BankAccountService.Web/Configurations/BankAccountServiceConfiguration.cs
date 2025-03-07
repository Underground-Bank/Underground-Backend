using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.BankAccountService.Infrastructure;
using UndergroundBank.BankAccountService.Infrastructure.Services;
using UndergroundBank.Common.Data;

namespace UndergroundBank.BankAccountService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddBankAccountServiceConfiguration(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<BankAccountDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("BankAccountDatabasePostgres"))
            );

            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });

            services.AddHttpContextAccessor();
            services.AddScoped<IBankService, BankService>();

            return services;
        }
    }
}
