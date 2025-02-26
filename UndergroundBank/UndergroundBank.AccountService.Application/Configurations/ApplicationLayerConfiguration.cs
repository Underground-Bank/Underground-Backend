using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.AccountService.Application.Helpers.AutoMapper;

namespace UndergroundBank.AccountService.Application.Configurations
{
    public static class Configure
    {
        public static void ConfigureApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(config =>
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
            services.AddAutoMapper(typeof(UserMapper));
        }
    }
}
