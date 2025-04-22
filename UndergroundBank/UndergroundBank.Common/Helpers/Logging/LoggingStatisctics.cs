namespace UndergroundBank.Common.Helpers.Logging;

public static class LoggingStatistics
{
    private static int _totalRequests = 0;
    private static int _failedRequests = 0;

    public static void IncrementTotal() => Interlocked.Increment(ref _totalRequests);
    public static void IncrementFailed() => Interlocked.Increment(ref _failedRequests);

    public static void LogErrorPercentage()
    {
        if (_totalRequests == 0)
            return;

        var errorPercentage = ((double)_failedRequests / _totalRequests) * 100;
        Console.WriteLine($"[GLOBAL] Error Percentage: {errorPercentage}%");
    }
}