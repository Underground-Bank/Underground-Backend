using Microsoft.AspNetCore.Http;

namespace UndergroundBank.Common.Middlewares;

public class UnstableMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Random _random = new();

    public UnstableMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var now = DateTime.Now;
        var minute = now.Minute;

        double errorChance = minute % 2 == 0 ? 0.9 : 0.5;

        if (_random.NextDouble() < errorChance)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync(
                "Internal Server Error (симулировано нестабильность)"
            );
            return;
        }

        await _next(context);
    }
}
