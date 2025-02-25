using Microsoft.EntityFrameworkCore;
using UndergroundBank.AccountService.Application.Interfaces;
using UndergroundBank.AccountService.Infrastructure;
using UndergroundBank.AccountService.Infrastructure.Helpers.TokenHerlpers;
using UndergroundBank.AccountService.Infrastructure.Repositories;
using UndergroundBank.AccountService.Infrastructure.Services;

namespace UndergroundBank.AccountService.Web.Configurations
{
    public static class ServiceDependencyExtension
    {
        public static IServiceCollection AddAccountBlServiceDependencies(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddDbContext<AccountDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AuthDatabasePostgres"))
            );
            //services.AddSingleton<RedisDBContext>(provider =>
            //{
            //    var connectionString = configuration.GetConnectionString("RedisDBContext");
            //    return new RedisDBContext(connectionString);
            //});

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<TokenHelper>();
            return services;
        }
    }
}
