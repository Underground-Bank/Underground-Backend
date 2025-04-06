using Microsoft.EntityFrameworkCore;
using UndergroundBank.BankAccountService.Application.Interfaces;
using UndergroundBank.BankAccountService.Domain.Entities;
using UndergroundBank.BankAccountService.Infrastructure;
using UndergroundBank.BankAccountService.Infrastructure.Services;
using UndergroundBank.Common.Data;
using UndergroundBank.Common.Data.Enums;

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
            services.AddSingleton<QueueSender>();
            services.AddScoped<AdditionalCurrencyService>();
            services.AddScoped<IBankService, BankService>();
            services.AddScoped<ICurrencyService, CurrencyService>();

            return services;
        }

        public static void EnsureMasterBankAccountExists(BankAccountDbContext dbContext)
        {
            var exists = dbContext.BankAccounts.Any(b =>
                b.AccountNumber == BankDefaults.MasterAccountNumber
            );

            if (!exists)
            {
                var masterAccount = new BankAccount
                {
                    AccountNumber = BankDefaults.MasterAccountNumber,
                    UserId = BankDefaults.MasterUserId,
                    Email = "bank@underground.local",
                    Name = "Underground",
                    Surname = "Bank",
                    PhoneNumber = "+70000000000",
                    CreatedDate = DateTime.UtcNow,
                    Balance = 10000000000000000,
                    IsLocked = false,
                    IsHidden = true,
                    Currency = Currency.RUB,
                };

                dbContext.BankAccounts.Add(masterAccount);
                dbContext.SaveChanges();
            }
        }
    }
}
