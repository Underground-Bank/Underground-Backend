using Polly;
using Polly.Wrap;
using UndergroundBank.Common.Data.Models;

namespace UndergroundBank.Common.Helpers.MessageBroker
{
    public static class ResiliencePolicyProvider
    {
        public static AsyncPolicyWrap GetDefaultPolicy()
        {
            var retryPolicy = Policy
                .Handle<RetryableException>()
                .WaitAndRetryAsync(
                    retryCount: 2,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        Console.WriteLine($"[Retry] Attempt {retryCount}: {exception.Message}");
                    }
                );

            var circuitBreakerPolicy = Policy
                .Handle<RetryableException>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 1,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, timespan) =>
                    {
                        Console.WriteLine(
                            $"[Circuit] Opened for {timespan.TotalSeconds}s due to: {exception.Message}"
                        );
                    },
                    onReset: () =>
                    {
                        Console.WriteLine("[Circuit] Closed. Calls will be allowed again.");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine("[Circuit] Half-open: test call is allowed.");
                    }
                );

            return Policy.WrapAsync(circuitBreakerPolicy, retryPolicy);
        }
    }
}
