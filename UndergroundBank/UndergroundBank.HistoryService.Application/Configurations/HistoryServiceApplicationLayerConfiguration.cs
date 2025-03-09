using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.BankAccountService.Application.Helpers.AutoMapper;

namespace UndergroundBank.HistoryService.Application.Configurations
{
    public static class Configure
    {
        public static void ConfigureHistoryApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
            services.AddAutoMapper(typeof(HistoryAutomapper).Assembly);
        }
    }
}
