using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Infrastructure.Repositories;
using UndergroundBank.Common.Data;
using UndergroundBank.LoanService.Application.Interfaces;
using UndergroundBank.LoanService.Infrastructure;
using UndergroundBank.LoanService.Infrastructure.Services;

namespace UndergroundBank.LoanService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddLoanBlServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<LoanDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("LoanDatabasePostgres"))
            );
            services.AddSingleton<RedisDbContext>(provider =>
            {
                var connectionString = configuration.GetConnectionString("RedisDBContext");
                return new RedisDbContext(connectionString);
            });

            services.AddScoped<ILoanService, LoansService>();
            services.AddScoped<ITariffService, TariffService>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddSingleton<IUserContext, UserContext>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}
