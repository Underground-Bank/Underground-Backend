using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UndergroundBank.LoanService.Application.Helpers.Automapper;

//using UndergroundBank.AccountService.Application.Helpers.AutoMapper;

namespace UndergroundBank.LoanService.Application.Configurations
{
    public static class Configure
    {
        public static void ConfigureApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
            services.AddAutoMapper(typeof(LoanServiceMapper));
        }
    }
}
