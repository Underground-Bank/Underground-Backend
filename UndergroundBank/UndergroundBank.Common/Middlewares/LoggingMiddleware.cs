using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using UndergroundBank.Common.Helpers.Logging;

namespace UndergroundBank.Common.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace";

        try
        {
            await _next(context);

            LoggingStatistics.IncrementTotal();
            Console.WriteLine(
                $"[{traceId}] {context.Request.Method} {context.Request.Path} succeeded in {sw.ElapsedMilliseconds}ms"
            );

            if (context.Response.StatusCode >= 500)
            {
                LoggingStatistics.IncrementFailed();
                Console.WriteLine(
                    $"[{traceId}] {context.Request.Method} {context.Request.Path} failed with status code {context.Response.StatusCode} in {sw.ElapsedMilliseconds}ms"
                );
            }

            LoggingStatistics.LogErrorPercentage();
        }
        catch (Exception ex)
        {
            LoggingStatistics.IncrementFailed();
            Console.WriteLine(
                $"[{traceId}] {context.Request.Method} {context.Request.Path} failed in {sw.ElapsedMilliseconds}ms: {ex.Message}"
            );

            LoggingStatistics.LogErrorPercentage();
            throw;
        }
    }
}
