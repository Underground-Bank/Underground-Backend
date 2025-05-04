using Microsoft.Extensions.DependencyInjection;
using UndergroundBank.MonitoringService.Application.Helpers.Automapper;

namespace UndergroundBank.MonitoringService.Application.Configurations;

public static class Configure
{
    public static void ConfigureApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MonitoringServiceMapper));
    }
}

