using EasyNetQ;
using Polly.Wrap;
using UndergroundBank.Common.Data.Models;
using UndergroundBank.Common.Middlewares;

namespace UndergroundBank.Common.Helpers.MessageBroker
{
    public abstract class ResilientQueueSender
    {
        protected readonly IBus _bus;
        private readonly AsyncPolicyWrap _policyWrap;

        protected ResilientQueueSender()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
            _policyWrap = ResiliencePolicyProvider.GetDefaultPolicy();
        }

        protected async Task<TResult> ExecuteWithPolicies<TResult>(Func<Task<TResult>> action)
        {
            return await _policyWrap.ExecuteAsync(async () =>
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] {ex}");

                    if (IsRetryable(ex))
                    {
                        throw new RetryableException("Retryable 500-level error occurred.", ex);
                    }

                    throw new BadRequestException(ex.Message);
                }
            });
        }

        protected async Task ExecuteWithPolicies(Func<Task> action)
        {
            await _policyWrap.ExecuteAsync(async () =>
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] {ex}");

                    if (IsRetryable(ex))
                    {
                        throw new RetryableException("Retryable 500-level error occurred.", ex);
                    }

                    throw new BadRequestException(ex.Message);
                }
            });
        }

        private bool IsRetryable(Exception ex)
        {
            if (ex is EasyNetQResponderException responderEx)
            {
                return responderEx.Message.Contains("500");
            }

            if (ex is TimeoutException)
            {
                return true;
            }

            return false;
        }
    }
}
