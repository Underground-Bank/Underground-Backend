using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.BankAccountService.Application.Helpers.AutoMapper;

namespace UndergroundBank.BankAccountService.Application.Configurations
{
    public static class Configure
    {
        public static void ConfigureBankAccountApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
            services.AddAutoMapper(typeof(BankAccountMapper));
        }
    }
}
