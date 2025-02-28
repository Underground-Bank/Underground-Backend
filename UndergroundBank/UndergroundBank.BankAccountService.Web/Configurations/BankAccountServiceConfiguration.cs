using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Infrastructure;
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

            return services;
        }
    }
}
