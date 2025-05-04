using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UndergroundBank.Common.Helpers.Logging;

public static class LoggingHelpers
{
    private static readonly ActivitySource _activitySource = new("LoanService");

    public static async Task<T> WithLogging<T>(
        Func<Task<T>> action,
        [CallerMemberName] string callerName = "")
    {
        using var activity = _activitySource.StartActivity(callerName);
        var sw = Stopwatch.StartNew();

        try
        {
            var result = await action();
            LoggingStatistics.IncrementTotal();

            Console.WriteLine($"[{activity?.TraceId}] {callerName} succeeded in {sw.ElapsedMilliseconds}ms");
            LoggingStatistics.LogErrorPercentage();

            return result;
        }
        catch (Exception ex)
        {
            LoggingStatistics.IncrementFailed();

            Console.WriteLine($"[{activity?.TraceId}] {callerName} failed in {sw.ElapsedMilliseconds}ms: {ex.Message}");
            LoggingStatistics.LogErrorPercentage();

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public static async Task WithLogging(
        Func<Task> action,
        [CallerMemberName] string callerName = "")
    {
        using var activity = _activitySource.StartActivity(callerName);
        var sw = Stopwatch.StartNew();

        try
        {
            await action();
            LoggingStatistics.IncrementTotal();

            Console.WriteLine($"[{activity?.TraceId}] {callerName} succeeded in {sw.ElapsedMilliseconds}ms");
            LoggingStatistics.LogErrorPercentage();
        }
        catch (Exception ex)
        {
            LoggingStatistics.IncrementFailed();

            Console.WriteLine($"[{activity?.TraceId}] {callerName} failed in {sw.ElapsedMilliseconds}ms: {ex.Message}");
            LoggingStatistics.LogErrorPercentage();

            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
